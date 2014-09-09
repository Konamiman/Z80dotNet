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

        public void Start(object globalState = null)
        {
            throw new NotImplementedException();
        }

        public void Continue()
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
                if(value>2)
                    throw new ArgumentException("Interrupt mode can be set to 0, 1 or 2 only");

                _InterruptMode = value;
            }
        }

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

                _InstructionExecutor = value;
                _InstructionExecutor.ProcessorAgent = this;
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

        public event EventHandler<MemoryAccessEventArgs> MemoryAccess;

        public event EventHandler<BeforeInstructionExecutionEventArgs> BeforeInstructionExecution;

        public event EventHandler<AfterInstructionExecutionEventArgs> AfterInstructionExecution;

        private void SetArrayContents<T>(T[] array, ushort startIndex, int length, T value)
        {
            if(length < 0)
                throw new ArgumentException("length can't be negative");
            if(startIndex + length > array.Length)
                throw new ArgumentException("start + length go beyond " + (array.Length - 1));

            var data = Enumerable.Repeat(value, length).ToArray();
            Array.Copy(data, 0, array, startIndex, length);
        }

        #region Members of IZ80ProcessorAgent

        public byte FetchNextOpcode()
        {
            throw new NotImplementedException();
        }

        public byte ReadFromMemory(ushort address)
        {
            throw new NotImplementedException();
        }

        public void WriteToMemory(ushort address, byte value)
        {
            throw new NotImplementedException();
        }

        public byte ReadFromPort(byte portNumber)
        {
            throw new NotImplementedException();
        }

        public void WriteToPort(byte portNumber, byte value)
        {
            throw new NotImplementedException();
        }

        public void SetInterruptMode(int interruptMode)
        {
            throw new NotImplementedException();
        }

        public void Stop(bool isPause = false)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
