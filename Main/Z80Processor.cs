using System;
using System.Collections.Generic;
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

        private const byte RET_opcode = 0xC9;
        private const byte HALT_opcode = 0x76;

        public Z80Processor()
        {
            ClockFrequencyInMHz = 4;
            ClockSpeedFactor = 1;

            AutoStopOnDiPlusHalt = false;
            AutoStopOnRetWithStackEmpty = false;

            SetMemoryWaitStatesForM1(0, MemorySpaceSize, 0);
            SetMemoryWaitStatesForNonM1(0, MemorySpaceSize, 0);
            SetPortWaitStates(0, PortSpaceSize, 0);

            Memory = new PlainMemory(MemorySpaceSize);
            PortsSpace = new PlainMemory(PortSpaceSize);

            SetMemoryAccessMode(0, MemorySpaceSize, MemoryAccessMode.ReadAndWrite);
            SetPortsSpaceAccessMode(0, PortSpaceSize, MemoryAccessMode.ReadAndWrite);

            Registers = new Z80Registers();

            InstructionExecutor = new Z80InstructionExecutor();

            ClockSynchronizationHelper = new ClockSynchronizationHelper();

            StopReason = StopReason.NeverRan;
            State = ProcessorState.Stopped;
        }

        #region Processor control

        public void Start(object userState = null)
        {
            if(userState != null)
                this.UserState = userState;

            Reset();
            
            InstructionExecutionLoop();
        }

        public void Continue()
        {
            InstructionExecutionLoop();
        }

        private void InstructionExecutionLoop()
        {
            executionContext = new InstructionExecutionContext();
            StopReason = StopReason.NotApplicable;
            State = ProcessorState.Running;

            while(!executionContext.MustStop)
            {
                executionContext.StartNewInstruction();

                var opcode = FetchNextOpcode();
                InstructionExecutor.Execute(opcode);

                CheckAutoStopForHaltOnDi();

                if(opcode == 0xC9)
                    executionContext.StopReason = StopReason.RetWithStackEmpty;
            }

            //TODO: Fire Before and After instruction execution events
            //TODO: Check for RetWithStackEmpty
            //TODO: Count extra T states and wait after instruction execution
            //TODO: Catch InstructionFetchFinished event, prevent further opcode fetches

            this.StopReason = executionContext.StopReason;
            this.State =
                StopReason == StopReason.PauseInvoked
                    ? ProcessorState.Paused
                    : ProcessorState.Stopped;
            executionContext = null;
        }

        private void CheckAutoStopForHaltOnDi()
        {
            if(AutoStopOnDiPlusHalt && executionContext.OpcodeBytes[0] == HALT_opcode && !InterruptsEnabled)
                executionContext.StopReason = StopReason.DiPlusHalt;
        }

        private bool InterruptsEnabled
        {
            get
            {
                return Registers.IFF1 == 1;
            }
        }
        
        void _InstructionExecutor_InstructionFetchFinished(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            Registers.IFF1 = 0;
            Registers.IFF1 = 0;
            Registers.PC = 0;
            Registers.Main.AF = 0xFFFF.ToShort();
            Registers.SP = 0xFFFF.ToShort();
            InterruptMode = 0;

            TStatesElapsedSinceReset = 0;
        }

        public void ExecuteNextInstruction()
        {
            throw new NotImplementedException();
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
                    _InstructionExecutor.InstructionFetchFinished -= _InstructionExecutor_InstructionFetchFinished;

                _InstructionExecutor = value;
                _InstructionExecutor.ProcessorAgent = this;
                _InstructionExecutor.InstructionFetchFinished += _InstructionExecutor_InstructionFetchFinished;
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
                _ClockSynchronizationHelper.EffecttiveClockSpeedInMHz = effectiveClockFrequency;
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
            FailIfNoInstructionExecuting();

            var value = ReadFromMemory(Registers.PC.ToUShort());
            executionContext.OpcodeBytes.Add(value);
            Registers.PC = Registers.PC.Inc();
            return value;
        }

        private void FailIfNoInstructionExecuting()
        {
            if(executionContext == null)
                throw new InvalidOperationException("This method can be invoked only when an instruction is being executed.");
        }

        public byte ReadFromMemory(ushort address)
        {
            FailIfNoInstructionExecuting();

            return ReadFromMemoryOrPort(
                address, 
                Memory, 
                GetMemoryAccessMode(address),
                MemoryAccessEventType.BeforeMemoryRead,
                MemoryAccessEventType.AfterMemoryRead);
        }

        private byte ReadFromMemoryOrPort(
            ushort address,
            IMemory memory,
            MemoryAccessMode accessMode,
            MemoryAccessEventType beforeEventType,
            MemoryAccessEventType afterEventType)
        {
            var beforeEventArgs = FireMemoryAccessEvent(beforeEventType, address, 0xFF);

            byte value;
            if(!beforeEventArgs.CancelMemoryAccess && 
                (accessMode == MemoryAccessMode.ReadAndWrite || accessMode == MemoryAccessMode.ReadOnly))
                value = memory[address];
            else
                value = beforeEventArgs.Value;

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
            FailIfNoInstructionExecuting();

            WritetoMemoryOrPort(
                address,
                value,
                Memory,
                GetMemoryAccessMode(address),
                MemoryAccessEventType.BeforeMemoryWrite,
                MemoryAccessEventType.AfterMemoryWrite);
        }

        private void WritetoMemoryOrPort(
            ushort address,
            byte value,
            IMemory memory,
            MemoryAccessMode accessMode,
            MemoryAccessEventType beforeEventType,
            MemoryAccessEventType afterEventType)
        {
            var beforeEventArgs = FireMemoryAccessEvent(beforeEventType, address, value);

            if(!beforeEventArgs.CancelMemoryAccess &&
                (accessMode == MemoryAccessMode.ReadAndWrite || accessMode == MemoryAccessMode.WriteOnly))
                memory[address] = beforeEventArgs.Value;

            FireMemoryAccessEvent(
                afterEventType, 
                address, 
                beforeEventArgs.Value, 
                beforeEventArgs.LocalUserState,
                beforeEventArgs.CancelMemoryAccess);
        }

        public byte ReadFromPort(byte portNumber)
        {
            FailIfNoInstructionExecuting();

            return ReadFromMemoryOrPort(
                portNumber, 
                PortsSpace, 
                GetPortAccessMode(portNumber),
                MemoryAccessEventType.BeforePortRead,
                MemoryAccessEventType.AfterPortRead);
        }

        public void WriteToPort(byte portNumber, byte value)
        {
            FailIfNoInstructionExecuting();

            WritetoMemoryOrPort(
                portNumber,
                value,
                PortsSpace,
                GetPortAccessMode(portNumber),
                MemoryAccessEventType.BeforePortWrite,
                MemoryAccessEventType.AfterPortWrite);
        }

        public void SetInterruptMode(byte interruptMode)
        {
            FailIfNoInstructionExecuting();

            this.InterruptMode = interruptMode;
        }

        public void Stop(bool isPause = false)
        {
            FailIfNoInstructionExecuting();

            executionContext.StopReason = 
                isPause ? 
                StopReason.PauseInvoked :
                StopReason.StopInvoked;
        }

        #endregion

        #region Instruction execution context

        protected InstructionExecutionContext executionContext;

        /// <summary>
        /// Internal class used to keep track of the current instruction execution.
        /// </summary>
        protected class InstructionExecutionContext
        {
            public InstructionExecutionContext()
            {
                StopReason = StopReason.NotApplicable;
                OpcodeBytes = new List<byte>();
            }

            public StopReason StopReason { get; set; }

            public bool MustStop
            {
                get
                {
                    return StopReason != StopReason.NotApplicable;
                }
            }

            public void StartNewInstruction()
            {
                OpcodeBytes.Clear();
                FetchComplete = false;
            }

            public bool FetchComplete { get; set; }

            public List<byte> OpcodeBytes { get; set; }
        }

        #endregion
    }
}
