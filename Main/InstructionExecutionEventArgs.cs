namespace Konamiman.Z80dotNet
{
    /// <summary>
    /// Event triggered by the <see cref="IZ80Processor"/> class before and after an instruction is executed.
    /// </summary>
    public class InstructionExecutionEventArgs : ProcessorEventArgs
    {
        /// <summary>
        /// Gets the type of event being processed. 
        /// </summary>
        public InstructionExecutionEventType EventType { get; private set; }

        /// <summary>
        /// Contains the full opcode bytes of the instruction to be executed or having been executed.
        /// </summary>
        public byte[] Opcode { get; set; }

        /// <summary>
        /// If true, this is the execution of a step in a block instruction (such as LDIR) but the first.
        /// So when a block instruction causes N executions, the first one has this property set to false,
        /// and the remaining N-1 have it set to true.
        /// </summary>
        public bool IsContinuationOfBlockInstructionExecution { get; private set; }
    }
}