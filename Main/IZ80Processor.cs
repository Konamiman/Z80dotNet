using System;

namespace Konamiman.Z80dotNet
{
    public interface IZ80Processor
    {
        // Control

        void Start(object globalState);

        void Stop();

        void Pause();

        void Continue();

        void Reset();

        void ExecuteNextInstruction();

        // Info and state

        ulong TStatesElapsedSinceStart { get; }

        ulong TStatesElapsedSinceReset { get; }

        StopReason StopReason { get; }

        ProcessorState State { get; }

        object UserState { get; set; }

        // Inside and outside world

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

        // Events

        event EventHandler<MemoryAccessEventArgs> MemoryAccess;

        event EventHandler<InstructionExecutionEventArgs> InstructionExecution;

    }

    public enum ProcessorState
    {
        Stopped,
        Paused,
        Running,
        ExecutingOneInstruction
    }

    public enum StopReason
    {
        NeverRan,
        StopInvoked,
        PauseInvoked,
        ExecuteNextInstructionInvoked,
        DiPlusHalt
    }

    public enum MemoryMode
    {
        RAM,
        ROM,
        NotConnected
    }

    public class InstructionExecutionEventArgs : ProcessorEventArgs
    {
        public InstructionExecutionOperation Operation { get; private set; }

        public byte[] Opcode { get; set; }

        public bool IsContinuationOfBlockInstructionExecution { get; private set; }
    }

    public enum InstructionExecutionOperation
    {
        BeforeInstructionExecution,
        AfterInstructionExecution
    }

    public class MemoryAccessEventArgs : ProcessorEventArgs
    {
        public MemoryAccessOperation Operation { get; private set; }

        public ushort Address { get; private set; }

        public byte Value { get; set; }
    }

    public enum MemoryAccessOperation
    {
        BeforeMemoryRead,
        AfterMemoryRead,
        BeforeMemoryWrite,
        AfterMemoryWrite,
        BeforePortRead,
        AfterPortRead,
        BeforePortWrite,
        AfterPortWrite
    }

    public class ProcessorEventArgs : EventArgs
    {
        object LocalUserState { get; set; }
    }

    public class Z80Lines
    {
        // Values can be 0 or 1 only.

        // Output

        public byte Halt { get; private set; }

        // Input

        public byte Reset { get; set; }

        public byte Interrupt { get; set; }

        public byte NonMaskableInterrupt { get; set; }
    }

    public class Z80Registers
    {
        public MainZ80Registers Main { get; set; }

        public MainZ80Registers Alternate { get; set; }

        public short IX { get; set; }

        public short IY { get; set; }

        public short PC { get; set; }

        public short SP { get; set; }

        public short IR { get; set; }

        public byte IM { get; set; }    // 0, 1 or 2

        public byte IFF1 { get; set; }  // 0 or 1

        public byte IFF2 { get; set; }  // 0 or 1
    }

    public class MainZ80Registers
    {
        public short AF { get; set; }

        public short BC { get; set; }

        public short DE { get; set; }

        public short HL { get; set; }
    }
}
