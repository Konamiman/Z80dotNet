using System;
using System.Linq;

namespace Konamiman.Z80dotNet
{
    /// <summary>
    /// The implementation of the <see cref="IZ80Processor"/> class.
    /// </summary>
    public class Z80Processor : IZ80Processor, IZ80ProcessorAgent
    {
        private const int MemorySpaceSize = 65536;
        private const int PortSpaceSize = 256;

        private const decimal MaxEffectiveClockSpeed = 100M;
        private const decimal MinEffectiveClockSpeed = 0.001M;

        public Z80Processor()
        {
            ClockSynchronizationHelper = new ClockSynchronizationHelper();

            ClockFrequencyInMHz = 4;
            ClockSpeedFactor = 1;
            
            AutoStopOnDiPlusHalt = true;
            AutoStopOnRetWithStackEmpty = false;
            StartOfStack = 0xFFFF.ToShort();

            SetMemoryWaitStatesForM1(0, MemorySpaceSize, 0);
            SetMemoryWaitStatesForNonM1(0, MemorySpaceSize, 0);
            SetPortWaitStates(0, PortSpaceSize, 0);

            Memory = new PlainMemory(MemorySpaceSize);
            PortsSpace = new PlainMemory(PortSpaceSize);

            SetMemoryAccessMode(0, MemorySpaceSize, MemoryAccessMode.ReadAndWrite);
            SetPortsSpaceAccessMode(0, PortSpaceSize, MemoryAccessMode.ReadAndWrite);

            Registers = new Z80Registers();

            InstructionExecutor = new Z80InstructionExecutor();

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
                return UnsafeInstructionExecutionLoop(isSingleInstruction);
            }
            catch
            {
                State = ProcessorState.Stopped;
                StopReason = StopReason.ExceptionThrown;

                throw;
            }
        }

        private int UnsafeInstructionExecutionLoop(bool isSingleInstruction)
        {
            ClockSynchronizationHelper.Start();
            executionContext = new InstructionExecutionContext();
            StopReason = StopReason.NotApplicable;
            State = ProcessorState.Running;
            var totalTStates = 0;

            while(!executionContext.MustStop)
            {
                executionContext.StartNewInstruction();

                var executionTStates = InstructionExecutor.Execute(FetchNextOpcode());
                
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

                if(isSingleInstruction)
                    executionContext.StopReason = StopReason.ExecuteNextInstructionInvoked;
                else
                    ClockSynchronizationHelper.TryWait(totalTStates);
            }

            ClockSynchronizationHelper.Stop();
            this.StopReason = executionContext.StopReason;
            this.State =
                StopReason == StopReason.PauseInvoked
                    ? ProcessorState.Paused
                    : ProcessorState.Stopped;

            executionContext = null;

            return totalTStates;
        }

        private void ThrowIfNoFetchFinishedEventFired()
        {
            if (executionContext.FetchComplete)
                return;

            throw new InstructionFetchFinishedEventNotFiredException(
                instructionAddress: Registers.PC.Sub((ushort)executionContext.OpcodeBytes.Count),
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
            if(AfterInstructionExecution != null)
                AfterInstructionExecution(this, new AfterInstructionExecutionEventArgs(
                    executionContext.OpcodeBytes.ToArray(), 
                    stopper: this,
                    localUserState: executionContext.LocalUserStateFromBeforeExecuteEvent,
                    tStates: tStates));
        }

        void InstructionExecutor_InstructionFetchFinished(object sender, InstructionFetchFinishedEventArgs e)
        {
            if(executionContext.FetchComplete)
                return;

            executionContext.FetchComplete = true;

            executionContext.IsRetInstruction = e.IsRetInstruction;
            executionContext.IsLdSpInstruction = e.IsLdSpInstruction;
            executionContext.IsHaltInstruction = e.IsHaltInstruction;

            executionContext.SpAfterInstructionFetch = Registers.SP;

            var eventArgs = FireBeforeInstructionExecutionEvent();
            executionContext.LocalUserStateFromBeforeExecuteEvent = eventArgs.LocalUserState;
        }

        BeforeInstructionExecutionEventArgs FireBeforeInstructionExecutionEvent()
        {
            var eventArgs = new BeforeInstructionExecutionEventArgs(executionContext.OpcodeBytes.ToArray());

            if(BeforeInstructionExecution != null)
                BeforeInstructionExecution(this, eventArgs);

            return eventArgs;
        }

        public void Reset()
        {
            Registers.IFF1 = 0;
            Registers.IFF2 = 0;
            Registers.PC = 0;
            Registers.AF = 0xFFFF.ToShort();
            Registers.SP = 0xFFFF.ToShort();
            InterruptMode = 0;

            TStatesElapsedSinceReset = 0;
            StartOfStack = Registers.SP;
        }

        public int ExecuteNextInstruction()
        {
            return InstructionExecutionLoop(isSingleInstruction: true);
        }

        public void FireNonMaskableInterrupt()
        {
            throw new NotImplementedException();
        }

        public void FireMaskableInterrupt(byte dataBusValue = 0xFF)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Information and state

        public ulong TStatesElapsedSinceStart { get; private set; }

        public ulong TStatesElapsedSinceReset { get; private set; }

        public StopReason StopReason { get; private set; }

        public ProcessorState State { get; private set; }

        public object UserState { get; set; }

        public bool IsHalted { get; private set; }

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
                    throw new ArgumentNullException("Registers");

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
                    throw new ArgumentNullException("Memory");

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
                throw new ArgumentException("length can't be negative");
            if(startIndex + length > array.Length)
                throw new ArgumentException("start + length go beyond " + (array.Length - 1));

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
                    throw new ArgumentNullException("PortsSpace");

                _PortsSpace = value;
            }
        }

        private MemoryAccessMode[] portsAccessModes = new MemoryAccessMode[PortSpaceSize];

        public void SetPortsSpaceAccessMode(byte startPort, int length, MemoryAccessMode mode)
        {
            SetArrayContents(portsAccessModes, startPort, length, mode);
        }

        public MemoryAccessMode GetPortAccessMode(byte portNumber)
        {
            return portsAccessModes[portNumber];
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
            ClockSynchronizationHelper.EffectiveClockFrequencyInMHz = effectiveClockFrequency;
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

        private byte[] portWaitStates = new byte[PortSpaceSize];

        public void SetPortWaitStates(ushort startPort, int length, byte waitStates)
        {
            SetArrayContents(portWaitStates, startPort, length, waitStates);
        }

        public byte GetPortWaitStates(byte portNumber)
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
                    throw new ArgumentNullException("InstructionExecutor");

                if(_InstructionExecutor != null)
                    _InstructionExecutor.InstructionFetchFinished -= InstructionExecutor_InstructionFetchFinished;

                _InstructionExecutor = value;
                _InstructionExecutor.ProcessorAgent = this;
                _InstructionExecutor.InstructionFetchFinished += InstructionExecutor_InstructionFetchFinished;
            }
        }

        private IClockSynchronizationHelper _ClockSynchronizationHelper;
        public IClockSynchronizationHelper ClockSynchronizationHelper
        {
            get
            {
                return _ClockSynchronizationHelper;
            }
            set
            {
                if(value == null)
                    throw new ArgumentNullException("ClockSynchronizationHelper");

                _ClockSynchronizationHelper = value;
                _ClockSynchronizationHelper.EffectiveClockFrequencyInMHz = effectiveClockFrequency;
            }
        }

        #endregion

        #region Events

        public event EventHandler<MemoryAccessEventArgs> MemoryAccess;

        public event EventHandler<BeforeInstructionExecutionEventArgs> BeforeInstructionExecution;

        public event EventHandler<AfterInstructionExecutionEventArgs> AfterInstructionExecution;

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
            Registers.PC = Registers.PC.Inc();
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
            if(!executionContext.FetchComplete)
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
            if(MemoryAccess != null)
                MemoryAccess(this, eventArgs);
            return eventArgs;
        }

        public void WriteToMemory(ushort address, byte value)
        {
            FailIfNoExecutionContext();
            FailIfNoInstructionFetchComplete();

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

        public byte ReadFromPort(byte portNumber)
        {
            FailIfNoExecutionContext();
            FailIfNoInstructionFetchComplete();

            return ReadFromMemoryOrPort(
                portNumber, 
                PortsSpace, 
                GetPortAccessMode(portNumber),
                MemoryAccessEventType.BeforePortRead,
                MemoryAccessEventType.AfterPortRead,
                GetPortWaitStates(portNumber));
        }

        public void WriteToPort(byte portNumber, byte value)
        {
            FailIfNoExecutionContext();
            FailIfNoInstructionFetchComplete();

            WritetoMemoryOrPort(
                portNumber,
                value,
                PortsSpace,
                GetPortAccessMode(portNumber),
                MemoryAccessEventType.BeforePortWrite,
                MemoryAccessEventType.AfterPortWrite,
                GetPortWaitStates(portNumber));
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
