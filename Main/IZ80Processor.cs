using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konamiman.Z80dotNet
{
    public interface IZ80Processor
    {
        // Control

        void Start(object globalState);

        void Stop();

        void Pause();

        void Reset();

        // Info

        ulong TStatesElapsedSinceStart { get; }

        ulong TStatesElapsedSinceReset { get; }

        // Registers, memory, lines

        Z80Registers Registers { get; set; }

        Z80Lines Lines { get; set; }

        byte[] Memory { get; }

        void SetMemoryContents(ushort startAddress, byte[] contents, ushort startIndex = 0, ushort? length = null);

        void SetMemoryMode(ushort startAddress, ushort length, MemoryMode mode);

        // Config

        decimal ClockFrequencyInMHz { get; set; }

        decimal ClockSpeedFactor { get; set; }

        bool AutoStopOnDiPlusHalt { get; set; }

        int MemoryWaitStates { get; set; }

        // Events:
        // MemoryRead -> MemoryAccess -> ProcessorOperation
        // MemoryWrite -> MemoryAccess -> ProcessorOperation
        // PortRead -> MemoryAccess -> ProcessorOperation
        // PortWrite -> MemoryAccess -> ProcessorOperation
        // BeforeInstructionExecution -> ProcessorOperation
        // AfterInstructionExecution -> ProcessorOperation
        
    }

    public class Z80Lines
    {
        // Output

        public byte Halt { get; private set; }

        // Input

        public byte Reset { get; set; }

        public byte Interrupt { get; set; }

        public byte NonMaskableInterrupt { get; set; }
    }

    public class Z80Registers
    {
    }
}
