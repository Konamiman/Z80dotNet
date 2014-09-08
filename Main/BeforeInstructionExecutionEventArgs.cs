namespace Konamiman.Z80dotNet
{
    /// <summary>
    /// Event triggered by the <see cref="IZ80Processor"/> class before an instruction is executed.
    /// </summary>
    public class BeforeInstructionExecutionEventArgs : ProcessorEventArgs
    {
        /// <summary>
        /// Contains the full opcode bytes of the instruction that is about to be executed.
        /// </summary>
        public byte[] Opcode { get; set; }
    }
}