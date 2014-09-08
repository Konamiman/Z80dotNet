using System;
using System.Linq;

namespace Konamiman.Z80dotNet
{
    public class Z80Processor : IZ80Processor
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
            SetPortsSpaceMode(0, PortSpaceSize, MemoryAccessMode.ReadAndWrite);

            Registers = new Z80Registers()
            {
                Main = new MainZ80Registers(),
                Alternate = new MainZ80Registers()
            };
        }

        public void Start(object globalState = null)
        {
            throw new NotImplementedException();
        }

        public void Stop(bool isPause = false)
        {
            throw new NotImplementedException();
        }

        public void Continue()
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
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

        public ulong TStatesElapsedSinceStart
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public ulong TStatesElapsedSinceReset
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public StopReason StopReason
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public ProcessorState State
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public object UserState
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool IsHalted
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public byte InterruptModeM
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public IZ80Registers Registers { get; set; }

        public IMemory Memory { get; private set; }

        public void SetMemoryAccessMode(ushort startAddress, int length, MemoryAccessMode mode)
        {
        }

        public MemoryAccessMode GetMemoryAccessMode(ushort address)
        {
            return MemoryAccessMode.ReadAndWrite;
        }

        public IMemory PortsSpace { get; private set; }

        public void SetPortsSpaceMode(byte startPort, int length, MemoryAccessMode mode)
        {
        }

        public MemoryAccessMode GetPortAccessMode(byte portNumber)
        {
            return MemoryAccessMode.ReadAndWrite;
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

        public event EventHandler<MemoryAccessEventArgs> MemoryAccess;

        public event EventHandler<InstructionExecutionEventArgs> InstructionExecution;
    }
}
