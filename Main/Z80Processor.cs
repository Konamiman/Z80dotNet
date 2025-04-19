using Konamiman.Z80dotNet.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Konamiman.Z80dotNet
{
    /// <summary>
    /// The implementation of the <see cref="IZ80Processor"/> interface.
    /// </summary>
    public class Z80Processor : IZ80Processor, IZ80ProcessorInterruptEvents, IZ80ProcessorExtendedPortsSpace, IZ80ProcessorAgent, IZ80ProcessorAgentExtendedPorts
    {
        private const int MemorySpaceSize = 65536;
        private int PortSpaceSize = 256;

        private const decimal MaxEffectiveClockSpeed = 100M;
        private const decimal MinEffectiveClockSpeed = 0.001M;

        private const ushort NmiServiceRoutine = 0x66;
        private const byte NOP_opcode = 0x00;
        private const byte RST38h_opcode = 0xFF;
        private const byte RETI_RETN_prefix = 0xED;
        private const byte RETI_opcode = 0x4D;
        private const byte RETN_opcode = 0x45;

        public Z80Processor()
        {
            ClockSynchronizer = new ClockSynchronizer();

            ClockFrequencyInMHz = 4;
            ClockSpeedFactor = 1;
            
            AutoStopOnDiPlusHalt = true;
            AutoStopOnRetWithStackEmpty = false;
            unchecked { StartOfStack =  (short)0xFFFF; }

            Memory = new PlainMemory(MemorySpaceSize);
            _PortsSpace = new PlainMemory(PortSpaceSize);
            portsAccessModes = new MemoryAccessMode[PortSpaceSize];
            portWaitStates = new byte[PortSpaceSize];

            SetMemoryWaitStatesForM1(0, MemorySpaceSize, 0);
            SetMemoryWaitStatesForNonM1(0, MemorySpaceSize, 0);
            SetPortWaitStates(0, PortSpaceSize, 0);

            SetMemoryAccessMode(0, MemorySpaceSize, MemoryAccessMode.ReadAndWrite);
            SetPortsSpaceAccessMode(0, PortSpaceSize, MemoryAccessMode.ReadAndWrite);

            Registers = new Z80Registers();
            InterruptSources = new List<IZ80InterruptSource>();

            InstructionExecutor = new Z80InstructionExecutor();
            InstructionExecutorExtendedPorts = (IZ80InstructionExecutorExtendedPorts)InstructionExecutor;

            StopReason = StopReason.NeverRan;
            State = ProcessorState.Stopped;
        }

        #region Processor control

        public void Start(object userState = null)
        {
            if(userState != null)
                this.UserState = userState;

            Reset();
            TStatesElapsedSinceStart = 0;

            InstructionExecutionLoop();
        }

        public void Continue()
        {
            InstructionExecutionLoop();
        }

        private int InstructionExecutionLoop(bool isSingleInstruction = false)
        {
            try
            {
                return InstructionExecutionLoopCore(isSingleInstruction);
            }
            catch
            {
                State = ProcessorState.Stopped;
                StopReason = StopReason.ExceptionThrown;

                throw;
            }
        }

        private int InstructionExecutionLoopCore(bool isSingleInstruction)
        {
            if(clockSynchronizer != null) clockSynchronizer.Start();
            executionContext = new InstructionExecutionContext();
            StopReason = StopReason.NotApplicable;
            State = ProcessorState.Running;
            var totalTStates = 0;

            while(!executionContext.MustStop)
            {
                executionContext.StartNewInstruction();

                FireBeforeInstructionFetchEvent();
                if(executionContext.MustStop)
                    break;

                var executionTStates = ExecuteNextOpcode();
                
                totalTStates = executionTStates + executionContext.AccummulatedMemoryWaitStates;
                TStatesElapsedSinceStart += (ulong)totalTStates;
                TStatesElapsedSinceReset += (ulong)totalTStates;

                ThrowIfNoFetchFinishedEventFired();

                if(!isSingleInstruction)
                {
                    CheckAutoStopForHaltOnDi();
                    CheckForAutoStopForRetWithStackEmpty();
                    CheckForLdSpInstruction();
                }

                FireAfterInstructionExecutionEvent(totalTStates);

                if(!IsHalted)
                    IsHalted = executionContext.IsHaltInstruction;

                var interruptTStates = AcceptPendingInterrupt();
                totalTStates += interruptTStates;
                TStatesElapsedSinceStart += (ulong)interruptTStates;
                TStatesElapsedSinceReset += (ulong)interruptTStates;

                if(isSingleInstruction)
                    executionContext.StopReason = StopReason.ExecuteNextInstructionInvoked;
                else if(clockSynchronizer != null)
                    clockSynchronizer.TryWait(totalTStates);
            }

            if(clockSynchronizer != null)
                clockSynchronizer.Stop();
            this.StopReason = executionContext.StopReason;
            this.State =
                StopReason == StopReason.PauseInvoked
                    ? ProcessorState.Paused
                    : ProcessorState.Stopped;

            executionContext = null;

            return totalTStates;
        }

        private int ExecuteNextOpcode()
        {
            if(IsHalted) {
                executionContext.OpcodeBytes.Add(NOP_opcode);
                return InstructionExecutor.Execute(NOP_opcode);
            }

            return InstructionExecutor.Execute(FetchNextOpcode());
        }

        private int AcceptPendingInterrupt()
        {
            if(executionContext.IsEiOrDiInstruction)
                return 0;

            if(NmiInterruptPending) {
                IsHalted = false;
                Registers.IFF1 = 0;
                ExecuteCall(NmiServiceRoutine);
                TriggerInterruptEvent(InterruptType.NonMaskable);
                return 11;
            }

            if(!InterruptsEnabled)
                return 0;

            var activeIntSource = InterruptSources.FirstOrDefault(s => s.IntLineIsActive);
            if(activeIntSource == null)
                return 0;

            Registers.IFF1 = 0;
            Registers.IFF2 = 0;
            IsHalted = false;

            switch(InterruptMode) {
                case 0:
                    var opcode = activeIntSource.ValueOnDataBus.GetValueOrDefault(0xFF);
                    TriggerInterruptEvent(InterruptType.Maskable);
                    InstructionExecutor.Execute(opcode);
                    return 13;
                case 1:
                    InstructionExecutor.Execute(RST38h_opcode);
                    TriggerInterruptEvent(InterruptType.Maskable);
                    return 13;
                case 2:
                    var pointerAddress = NumberUtils.CreateUshort(
                        lowByte: activeIntSource.ValueOnDataBus.GetValueOrDefault(0xFF),
                        highByte: Registers.I);
                    var callAddress = NumberUtils.CreateUshort(
                        lowByte: ReadFromMemoryInternal(pointerAddress),
                        highByte: ReadFromMemoryInternal((ushort)(pointerAddress + 1)));
                    ExecuteCall(callAddress);
                    TriggerInterruptEvent(InterruptType.Maskable);
                    return 19;
            }

            return 0;
        }

        public void ExecuteCall(ushort address)
        {
            var oldAddress = (short)Registers.PC;
            var sp = (ushort)(Registers.SP - 1);
            WriteToMemoryInternal(sp, oldAddress.GetHighByte());
            sp = (ushort)(sp - 1);
            WriteToMemoryInternal(sp, oldAddress.GetLowByte());

            Registers.SP = (short)sp;
            Registers.PC = address;
        }

        private void TriggerInterruptEvent(InterruptType interruptType)
        {
            switch (interruptType)
            {
                case InterruptType.Maskable:
                    MaskableInterruptServicingStart?.Invoke(this, EventArgs.Empty);
                    break;

                case InterruptType.NonMaskable:
                    NonMaskableInterruptServicingStart?.Invoke(this, EventArgs.Empty);
                    break;

                default:
                    throw new InvalidOperationException($"Unknown interrupt type: {interruptType}");
            }
        }

        public void ExecuteRet()
        {
            var sp = (ushort)Registers.SP;
            var newPC = NumberUtils.CreateShort(ReadFromMemoryInternal(sp), ReadFromMemoryInternal((ushort)(sp + 1)));

            Registers.PC = (ushort)newPC;
            Registers.SP += 2;
        }

        private void ThrowIfNoFetchFinishedEventFired()
        {
            if (executionContext.FetchComplete)
                return;

            throw new InstructionFetchFinishedEventNotFiredException(
                instructionAddress: (ushort)(Registers.PC - executionContext.OpcodeBytes.Count),
                fetchedBytes: executionContext.OpcodeBytes.ToArray());
        }

        private void CheckAutoStopForHaltOnDi()
        {
            if(AutoStopOnDiPlusHalt && executionContext.IsHaltInstruction && !InterruptsEnabled)
                executionContext.StopReason = StopReason.DiPlusHalt;
        }

        private void CheckForAutoStopForRetWithStackEmpty()
        {
            if(AutoStopOnRetWithStackEmpty && executionContext.IsRetInstruction && StackIsEmpty())
                executionContext.StopReason = StopReason.RetWithStackEmpty;
        }

        private void CheckForLdSpInstruction()
        {
            if(executionContext.IsLdSpInstruction)
                StartOfStack = Registers.SP;
        }

        private bool StackIsEmpty()
        {
            return executionContext.SpAfterInstructionFetch == StartOfStack;
        }

        private bool InterruptsEnabled
        {
            get
            {
                return Registers.IFF1 == 1;
            }
        }
        
        void FireAfterInstructionExecutionEvent(int tStates)
        {
            var opcodeBytes = executionContext.OpcodeBytes.ToArray();

            AfterInstructionExecution?.Invoke(this, new AfterInstructionExecutionEventArgs(
                opcodeBytes,
                stopper: this,
                localUserState: executionContext.LocalUserStateFromPreviousEvent,
                tStates: tStates));

            if (opcodeBytes[0] == RETI_RETN_prefix)
            {
                opcodeBytes[1] &= 0xCF; //To account for mirrored variants
                if (opcodeBytes[1] == RETI_opcode)
                    AfterRetiInstructionExecution?.Invoke(this, EventArgs.Empty);
                else if (opcodeBytes[1] == RETN_opcode)
                    AfterRetnInstructionExecution?.Invoke(this, EventArgs.Empty);
            }
        }

        void InstructionExecutor_InstructionFetchFinished(object sender, InstructionFetchFinishedEventArgs e)
        {
            if(executionContext.FetchComplete)
                return;

            executionContext.FetchComplete = true;

            executionContext.IsRetInstruction = e.IsRetInstruction;
            executionContext.IsLdSpInstruction = e.IsLdSpInstruction;
            executionContext.IsHaltInstruction = e.IsHaltInstruction;
            executionContext.IsEiOrDiInstruction = e.IsEiOrDiInstruction;

            executionContext.SpAfterInstructionFetch = Registers.SP;

            var eventArgs = FireBeforeInstructionExecutionEvent();
            executionContext.LocalUserStateFromPreviousEvent = eventArgs.LocalUserState;
        }

        void FireBeforeInstructionFetchEvent()
        {
            var eventArgs = new BeforeInstructionFetchEventArgs(stopper: this);

            if(BeforeInstructionFetch != null) {
                executionContext.ExecutingBeforeInstructionEvent = true;
                try {
                    BeforeInstructionFetch(this, eventArgs);
                }
                finally {
                    executionContext.ExecutingBeforeInstructionEvent = false;
                }
            }

            executionContext.LocalUserStateFromPreviousEvent = eventArgs.LocalUserState;
        }

        BeforeInstructionExecutionEventArgs FireBeforeInstructionExecutionEvent()
        {
            var opcodeBytes = executionContext.OpcodeBytes.ToArray();

            var eventArgs = new BeforeInstructionExecutionEventArgs(
                opcodeBytes,
                executionContext.LocalUserStateFromPreviousEvent);

            BeforeInstructionExecution?.Invoke(this, eventArgs);

            if (opcodeBytes[0] == RETI_RETN_prefix)
            {
                opcodeBytes[1] &= 0xCF; //To account for mirrored variants
                if(opcodeBytes[1] == RETI_opcode)
                    BeforeRetiInstructionExecution?.Invoke(this, EventArgs.Empty);
                else if (opcodeBytes[1] == RETN_opcode)
                    BeforeRetnInstructionExecution?.Invoke(this, EventArgs.Empty);
            }

            return eventArgs;
        }

        public void Reset()
        {
            Registers.IFF1 = 0;
            Registers.IFF2 = 0;
            Registers.PC = 0;
            unchecked { Registers.AF = (short)0xFFFF; }
            unchecked { Registers.SP = (short)0xFFFF; }
            InterruptMode = 0;

            NmiInterruptPending = false;
            IsHalted = false;

            TStatesElapsedSinceReset = 0;
            StartOfStack = Registers.SP;
        }

        public int ExecuteNextInstruction()
        {
            return InstructionExecutionLoop(isSingleInstruction: true);
        }

        #endregion

        #region Information and state

        public ulong TStatesElapsedSinceStart { get; private set; }

        public ulong TStatesElapsedSinceReset { get; private set; }

        public StopReason StopReason { get; private set; }

        public ProcessorState State { get; private set; }

        public object UserState { get; set; }

        public bool IsHalted { get; protected set; }

        private byte _InterruptMode;
        public byte InterruptMode
        {
            get
            {
                return _InterruptMode;
            }
            set
            {
                if(value > 2)
                    throw new ArgumentException("Interrupt mode can be set to 0, 1 or 2 only");

                _InterruptMode = value;
            }
        }

        public short StartOfStack { get; protected set; }

        #endregion

        #region Inside and outside world

        private IZ80Registers _Registers;
        public IZ80Registers Registers
        {
            get
            {
                return _Registers;
            }
            set
            {
                if(value == null)
                    throw new ArgumentNullException(nameof(Registers));

                _Registers = value;
            }
        }

        private IMemory _Memory;
        public IMemory Memory
        {
            get
            {
                return _Memory;
            }
            set
            {
                if(value == null)
                    throw new ArgumentNullException(nameof(Memory));

                _Memory = value;
            }
        }

        private MemoryAccessMode[] memoryAccessModes = new MemoryAccessMode[MemorySpaceSize];

        public void SetMemoryAccessMode(ushort startAddress, int length, MemoryAccessMode mode)
        {
             SetArrayContents(memoryAccessModes, startAddress, length, mode);
        }

        private void SetArrayContents<T>(T[] array, ushort startIndex, int length, T value)
        {
            if(length < 0)
                throw new ArgumentException($"{nameof(length)} can't be negative");
            if(startIndex + length > array.Length)
                throw new ArgumentException($"{nameof(startIndex)} + {nameof(length)} go beyond " + (array.Length - 1));

            var data = Enumerable.Repeat(value, length).ToArray();
            Array.Copy(data, 0, array, startIndex, length);
        }

        public MemoryAccessMode GetMemoryAccessMode(ushort address)
        {
            return memoryAccessModes[address];
        }

        private IMemory _PortsSpace;
        public IMemory PortsSpace
        {
            get
            {
                return _PortsSpace;
            }
            set
            {
                if(value == null)
                    throw new ArgumentNullException(nameof(PortsSpace));

                if(value.Size < PortSpaceSize) {
                    throw new InvalidOperationException($"{nameof(PortsSpace)} must be set to an instance of {nameof(IMemory)} with a size of at least {PortSpaceSize} bytes when {nameof(UseExtendedPortsSpace)} is {UseExtendedPortsSpace}");
                }

                _PortsSpace = value;
            }
        }

        private MemoryAccessMode[] portsAccessModes;

        public void SetPortsSpaceAccessMode(byte startPort, int length, MemoryAccessMode mode) => SetExtendedPortsSpaceAccessMode(startPort, length, mode);

        public void SetExtendedPortsSpaceAccessMode(ushort startPort, int length, MemoryAccessMode mode)
        {
            SetArrayContents(portsAccessModes, startPort, length, mode);
        }

        public MemoryAccessMode GetPortAccessMode(byte portNumber) => GetExtendedPortAccessMode(portNumber);

        public MemoryAccessMode GetExtendedPortAccessMode(ushort portNumber)
        {
            return portsAccessModes[portNumber];
        }

        private IList<IZ80InterruptSource> InterruptSources { get; set; }

        public void RegisterInterruptSource(IZ80InterruptSource source)
        {
            if(InterruptSources.Contains(source))
                return;

            InterruptSources.Add(source);
            source.NmiInterruptPulse += (sender, args) => NmiInterruptPending = true;
        }

        private readonly object nmiInterruptPendingSync = new object();
        private bool _nmiInterruptPending;
        private bool NmiInterruptPending
        {
            get
            {
                lock(nmiInterruptPendingSync) {
                    var value = _nmiInterruptPending;
                    _nmiInterruptPending = false;
                    return value;
                }
            }
            set
            {
                lock(nmiInterruptPendingSync) {
                    _nmiInterruptPending = value;
                }
            }
        }

        public IEnumerable<IZ80InterruptSource> GetRegisteredInterruptSources()
        {
            return InterruptSources.ToArray();
        }

        public void UnregisterAllInterruptSources()
        {
            foreach(var source in InterruptSources) {
                source.NmiInterruptPulse -= (sender, args) => NmiInterruptPending = true;
            }

            InterruptSources.Clear();
        }

        #endregion

        #region Configuration

        private decimal effectiveClockFrequency;

        private decimal _ClockFrequencyInMHz = 1;
        public decimal ClockFrequencyInMHz
        {
            get
            {
                return _ClockFrequencyInMHz;
            }
            set
            {
                SetEffectiveClockFrequency(value, ClockSpeedFactor);
                _ClockFrequencyInMHz = value;
            }
        }

        private void SetEffectiveClockFrequency(decimal clockFrequency, decimal clockSpeedFactor)
        {
            decimal effectiveClockFrequency = clockFrequency * clockSpeedFactor;
            if((effectiveClockFrequency > MaxEffectiveClockSpeed) ||
                (effectiveClockFrequency < MinEffectiveClockSpeed))
                throw new ArgumentException(string.Format("Clock frequency multiplied by clock speed factor must be a number between {0} and {1}", MinEffectiveClockSpeed, MaxEffectiveClockSpeed));

            this.effectiveClockFrequency = effectiveClockFrequency;
            if(clockSynchronizer != null)
                clockSynchronizer.EffectiveClockFrequencyInMHz = effectiveClockFrequency;
        }

        private decimal _ClockSpeedFactor = 1;
        public decimal ClockSpeedFactor
        {
            get
            {
                return _ClockSpeedFactor;
            }
            set
            {
                SetEffectiveClockFrequency(ClockFrequencyInMHz, value);
                _ClockSpeedFactor = value;
            }
        }

        public bool AutoStopOnDiPlusHalt { get; set; }

        public bool AutoStopOnRetWithStackEmpty { get; set; }

        private byte[] memoryWaitStatesForM1 = new byte[MemorySpaceSize];

        public void SetMemoryWaitStatesForM1(ushort startAddress, int length, byte waitStates)
        {
            SetArrayContents(memoryWaitStatesForM1, startAddress, length, waitStates);
        }

        public byte GetMemoryWaitStatesForM1(ushort address)
        {
            return memoryWaitStatesForM1[address];
        }

        private byte[] memoryWaitStatesForNonM1 = new byte[MemorySpaceSize];

        public void SetMemoryWaitStatesForNonM1(ushort startAddress, int length, byte waitStates)
        {
            SetArrayContents(memoryWaitStatesForNonM1, startAddress, length, waitStates);
        }

        public byte GetMemoryWaitStatesForNonM1(ushort address)
        {
            return memoryWaitStatesForNonM1[address];
        }

        private byte[] portWaitStates;

        public void SetPortWaitStates(ushort startPort, int length, byte waitStates) => SetExtendedPortWaitStates(startPort, length, waitStates);

        public void SetExtendedPortWaitStates(ushort startPort, int length, byte waitStates)
        {
            SetArrayContents(portWaitStates, startPort, length, waitStates);
        }

        public byte GetPortWaitStates(byte portNumber) => GetExtendedPortWaitStates(portNumber);

        public byte GetExtendedPortWaitStates(ushort portNumber)
        {
            return portWaitStates[portNumber];
        }

        private IZ80InstructionExecutor _InstructionExecutor;
        public IZ80InstructionExecutor InstructionExecutor
        {
            get
            {
                return _InstructionExecutor;
            }
            set
            {
                if(value == null)
                    throw new ArgumentNullException(nameof(InstructionExecutor));

                if(_InstructionExecutor != null)
                    _InstructionExecutor.InstructionFetchFinished -= InstructionExecutor_InstructionFetchFinished;

                _InstructionExecutor = value;
                _InstructionExecutor.ProcessorAgent = this;
                _InstructionExecutor.InstructionFetchFinished += InstructionExecutor_InstructionFetchFinished;
            }
        }

        private IZ80InstructionExecutorExtendedPorts _InstructionExecutorExtendedPorts;
        public IZ80InstructionExecutorExtendedPorts InstructionExecutorExtendedPorts
        {
            get
            {
                return _InstructionExecutorExtendedPorts;
            }
            set
            {
                if(value == null)
                    throw new ArgumentNullException(nameof(InstructionExecutorExtendedPorts));

                _InstructionExecutorExtendedPorts = value;
                _InstructionExecutorExtendedPorts.ProcessorAgentExtendedPorts = this;
            }
        }

        private IClockSynchronizer clockSynchronizer;
        public IClockSynchronizer ClockSynchronizer
        {
            get
            {
                return clockSynchronizer;
            }
            set
            {
                clockSynchronizer = value;
                if (value == null)
                    return;

                clockSynchronizer.EffectiveClockFrequencyInMHz = effectiveClockFrequency;
            }
        }

        private bool useExtendedPortsSpace = false;

        /// <summary>
        /// Gets or sets a value indicating whether the processor is using extended (16 bits) ports space.
        /// 
        /// The first 256 items in the port access modes and port wait states arrays will be preserved
        /// when modifying the value of this property. When setting the value to true,
        /// ports 256 to 65535 will get read and write access mode and zero wait states.
        /// </summary>
        public bool UseExtendedPortsSpace
        {
            get => useExtendedPortsSpace;

            set
            {
                if(value == useExtendedPortsSpace) {
                    return;
                }

                var newPortsSpaceSize = value ? 65536 : 256;
                if(PortsSpace.Size < newPortsSpaceSize) {
                    throw new InvalidOperationException($"UseExtendedPortsSpace can be set to {value} only if the ports space size is {newPortsSpaceSize} bytes");
                }

                useExtendedPortsSpace = value;
                PortSpaceSize = newPortsSpaceSize;

                var newPortsAccessModes = new MemoryAccessMode[PortSpaceSize];
                Array.Copy(portsAccessModes, newPortsAccessModes, 256);
                portsAccessModes = newPortsAccessModes;

                var newPortWaitStates = new byte[PortSpaceSize];
                Array.Copy(portWaitStates, newPortWaitStates, 256);
                portWaitStates = newPortWaitStates;
            }
        }

        #endregion

        #region Events

        public event EventHandler<MemoryAccessEventArgs> MemoryAccess;

        public event EventHandler<BeforeInstructionFetchEventArgs> BeforeInstructionFetch;

        public event EventHandler<BeforeInstructionExecutionEventArgs> BeforeInstructionExecution;

        public event EventHandler<AfterInstructionExecutionEventArgs> AfterInstructionExecution;

        public event EventHandler MaskableInterruptServicingStart;

        public event EventHandler NonMaskableInterruptServicingStart;

        public event EventHandler BeforeRetiInstructionExecution;

        public event EventHandler AfterRetiInstructionExecution;

        public event EventHandler BeforeRetnInstructionExecution;

        public event EventHandler AfterRetnInstructionExecution;

        #endregion

        #region Members of IZ80ProcessorAgent

        public byte FetchNextOpcode()
        {
            FailIfNoExecutionContext();

            if(executionContext.FetchComplete)
                throw new InvalidOperationException("FetchNextOpcode can be invoked only before the InstructionFetchFinished event has been raised.");

            byte opcode;
            if (executionContext.PeekedOpcode == null)
            {
                var address = Registers.PC;
                opcode = ReadFromMemoryOrPort(
                    address,
                    Memory,
                    GetMemoryAccessMode(address),
                    MemoryAccessEventType.BeforeMemoryRead,
                    MemoryAccessEventType.AfterMemoryRead,
                    GetMemoryWaitStatesForM1(address));
            }
            else
            {
                executionContext.AccummulatedMemoryWaitStates +=
                    GetMemoryWaitStatesForM1(executionContext.AddressOfPeekedOpcode);
                opcode = executionContext.PeekedOpcode.Value;
                executionContext.PeekedOpcode = null;
            }

            executionContext.OpcodeBytes.Add(opcode);
            Registers.PC++;
            return opcode;
        }
        
        public byte PeekNextOpcode()
        {
            FailIfNoExecutionContext();

            if(executionContext.FetchComplete)
                throw new InvalidOperationException("PeekNextOpcode can be invoked only before the InstructionFetchFinished event has been raised.");

            if (executionContext.PeekedOpcode == null)
            {
                var address = Registers.PC;
                var opcode = ReadFromMemoryOrPort(
                    address,
                    Memory,
                    GetMemoryAccessMode(address),
                    MemoryAccessEventType.BeforeMemoryRead,
                    MemoryAccessEventType.AfterMemoryRead,
                    waitStates: 0);

                executionContext.PeekedOpcode = opcode;
                executionContext.AddressOfPeekedOpcode = Registers.PC;
                return opcode;
            }
            else
            {
                return executionContext.PeekedOpcode.Value;
            }
        }

        private void FailIfNoExecutionContext()
        {
            if(executionContext == null)
                throw new InvalidOperationException("This method can be invoked only when an instruction is being executed.");
        }

        public byte ReadFromMemory(ushort address)
        {
            FailIfNoExecutionContext();
            FailIfNoInstructionFetchComplete();

            return ReadFromMemoryInternal(address);
        }

        private byte ReadFromMemoryInternal(ushort address)
        {
            return ReadFromMemoryOrPort(
                address,
                Memory,
                GetMemoryAccessMode(address),
                MemoryAccessEventType.BeforeMemoryRead,
                MemoryAccessEventType.AfterMemoryRead,
                GetMemoryWaitStatesForNonM1(address));
        }

        protected virtual void FailIfNoInstructionFetchComplete()
        {
            if(executionContext != null && !executionContext.FetchComplete)
                throw new InvalidOperationException("IZ80ProcessorAgent members other than FetchNextOpcode can be invoked only after the InstructionFetchFinished event has been raised.");
        }

        private byte ReadFromMemoryOrPort(
            ushort address,
            IMemory memory,
            MemoryAccessMode accessMode,
            MemoryAccessEventType beforeEventType,
            MemoryAccessEventType afterEventType,
            byte waitStates)
        {
            var beforeEventArgs = FireMemoryAccessEvent(beforeEventType, address, 0xFF);

            byte value;
            if(!beforeEventArgs.CancelMemoryAccess && 
                (accessMode == MemoryAccessMode.ReadAndWrite || accessMode == MemoryAccessMode.ReadOnly))
                value = memory[address];
            else
                value = beforeEventArgs.Value;

            if(executionContext != null)
                executionContext.AccummulatedMemoryWaitStates += waitStates;

            var afterEventArgs = FireMemoryAccessEvent(
                afterEventType, 
                address,
                value,
                beforeEventArgs.LocalUserState,
                beforeEventArgs.CancelMemoryAccess);
            return afterEventArgs.Value;
        }

        MemoryAccessEventArgs FireMemoryAccessEvent(
            MemoryAccessEventType eventType,
            ushort address, 
            byte value, 
            object localUserState = null,
            bool cancelMemoryAccess = false)
        {
            var eventArgs = new MemoryAccessEventArgs(eventType, address, value, localUserState, cancelMemoryAccess);
            MemoryAccess?.Invoke(this, eventArgs);
            return eventArgs;
        }

        public void WriteToMemory(ushort address, byte value)
        {
            FailIfNoExecutionContext();
            FailIfNoInstructionFetchComplete();

            WriteToMemoryInternal(address, value);
        }

        private void WriteToMemoryInternal(ushort address, byte value)
        {
            WritetoMemoryOrPort(
                address,
                value,
                Memory,
                GetMemoryAccessMode(address),
                MemoryAccessEventType.BeforeMemoryWrite,
                MemoryAccessEventType.AfterMemoryWrite,
                GetMemoryWaitStatesForNonM1(address));
        }

        private void WritetoMemoryOrPort(
            ushort address,
            byte value,
            IMemory memory,
            MemoryAccessMode accessMode,
            MemoryAccessEventType beforeEventType,
            MemoryAccessEventType afterEventType,
            byte waitStates)
        {
            var beforeEventArgs = FireMemoryAccessEvent(beforeEventType, address, value);

            if(!beforeEventArgs.CancelMemoryAccess &&
                (accessMode == MemoryAccessMode.ReadAndWrite || accessMode == MemoryAccessMode.WriteOnly))
                memory[address] = beforeEventArgs.Value;

            if(executionContext != null)
                executionContext.AccummulatedMemoryWaitStates += waitStates;

            FireMemoryAccessEvent(
                afterEventType, 
                address, 
                beforeEventArgs.Value, 
                beforeEventArgs.LocalUserState,
                beforeEventArgs.CancelMemoryAccess);
        }

        public byte ReadFromPort(byte portNumber) => ReadFromPort(portNumber, 0);

        public byte ReadFromPort(byte portNumberLow, byte portNumberHigh)
        {
            FailIfNoExecutionContext();
            FailIfNoInstructionFetchComplete();

            ushort portNumber = useExtendedPortsSpace ? NumberUtils.CreateUshort(portNumberLow, portNumberHigh) : portNumberLow;

            return ReadFromMemoryOrPort(
                portNumber,
                PortsSpace,
                GetExtendedPortAccessMode(portNumber),
                MemoryAccessEventType.BeforePortRead,
                MemoryAccessEventType.AfterPortRead,
                GetExtendedPortWaitStates(portNumber));
        }

        public void WriteToPort(byte portNumber, byte value) => WriteToPort(portNumber, 0, value);

        public void WriteToPort(byte portNumberLow, byte portNumberHigh, byte value)
        {
            FailIfNoExecutionContext();
            FailIfNoInstructionFetchComplete();

            ushort portNumber = useExtendedPortsSpace ? NumberUtils.CreateUshort(portNumberLow, portNumberHigh) : portNumberLow;

            WritetoMemoryOrPort(
                portNumber,
                value,
                PortsSpace,
                GetExtendedPortAccessMode(portNumber),
                MemoryAccessEventType.BeforePortWrite,
                MemoryAccessEventType.AfterPortWrite,
                GetExtendedPortWaitStates(portNumber));
        }

        public void SetInterruptMode(byte interruptMode)
        {
            FailIfNoExecutionContext();
            FailIfNoInstructionFetchComplete();

            this.InterruptMode = interruptMode;
        }

        public void Stop(bool isPause = false)
        {
            FailIfNoExecutionContext();

            if(!executionContext.ExecutingBeforeInstructionEvent)
                FailIfNoInstructionFetchComplete();

            executionContext.StopReason = 
                isPause ? 
                StopReason.PauseInvoked :
                StopReason.StopInvoked;
        }

        IZ80Registers IZ80ProcessorAgent.Registers
        {
            get
            {
                return _Registers;
            }
        }

        #endregion

        #region Instruction execution context

        protected InstructionExecutionContext executionContext;

        #endregion
    }
}
