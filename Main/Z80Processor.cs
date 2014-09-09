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

        public Z80Processor()
        {
            ClockFrequencyInMHz = 4;
            ClockSpeedFactor = 1;

            AutoStopOnDiPlusHalt = false;
            AutoStopOnRetWithStackEmpty = false;

            SetMemoryWaitStatesForM1(0, MemorySpaceSize, 0);
            SetMemoryWaitStatesForNonM1(0, MemorySpaceSize, 0);
            SetPortWaitStates(0, MemorySpaceSize, 0);

            Memory = new PlainMemory(MemorySpaceSize);
            PortsSpace = new PlainMemory(PortSpaceSize);

            SetMemoryAccessMode(0, MemorySpaceSize, MemoryAccessMode.ReadAndWrite);
            SetPortsSpaceAccessMode(0, PortSpaceSize, MemoryAccessMode.ReadAndWrite);

            Registers = new Z80Registers()
            {
                Main = new MainZ80Registers(),
                Alternate = new MainZ80Registers()
            };

            InstructionExecutor = new Z80InstructionExecutor()
            {
                ProcessorAgent = this
            };

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

        public IZ80Registers Registers { get; set; }

        public IMemory Memory { get; private set; }

        private MemoryAccessMode[] memoryAccessModes = new MemoryAccessMode[MemorySpaceSize];

        public void SetMemoryAccessMode(ushort startAddress, int length, MemoryAccessMode mode)
        {
            var data = Enumerable.Repeat<MemoryAccessMode>(mode, length).ToArray();
            Array.Copy(data, 0, memoryAccessModes, startAddress, length);
        }

        public MemoryAccessMode GetMemoryAccessMode(ushort address)
        {
            return memoryAccessModes[address];
        }

        public IMemory PortsSpace { get; private set; }

        private MemoryAccessMode[] portsAccessModes = new MemoryAccessMode[PortSpaceSize];

        public void SetPortsSpaceAccessMode(byte startPort, int length, MemoryAccessMode mode)
        {
            var data = Enumerable.Repeat<MemoryAccessMode>(mode, length).ToArray();
            Array.Copy(data, 0, portsAccessModes, startPort, length);
        }

        public MemoryAccessMode GetPortAccessMode(byte portNumber)
        {
            return portsAccessModes[portNumber];
        }

        public decimal ClockFrequencyInMHz { get; set; }

        public decimal ClockSpeedFactor { get; set; }

        public bool AutoStopOnDiPlusHalt { get; set; }

        public bool AutoStopOnRetWithStackEmpty { get; set; }

        public void SetMemoryWaitStatesForM1(ushort startAddress, int length, byte waitStates)
        {
        }

        public byte GetMemoryWaitStatesForM1(ushort address)
        {
            return 0;
        }

        public void SetMemoryWaitStatesForNonM1(ushort startAddress, int length, byte waitStates)
        {
        }

        public byte GetMemoryWaitStatesForNonM1(ushort address)
        {
            return 0;
        }

        public void SetPortWaitStates(ushort startPort, int length, byte waitStates)
        {
        }

        public byte GetPortWaitStates(byte portNumber)
        {
            return 0;
        }

        public IZ80InstructionExecutor InstructionExecutor { get; set; }

        public IClockSynchronizationHelper ClockSynchronizationHelper { get; set; }

        public event EventHandler<MemoryAccessEventArgs> MemoryAccess;

        public event EventHandler<BeforeInstructionExecutionEventArgs> BeforeInstructionExecution;

        public event EventHandler<AfterInstructionExecutionEventArgs> AfterInstructionExecution;

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
