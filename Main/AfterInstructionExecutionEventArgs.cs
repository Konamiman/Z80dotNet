namespace Konamiman.Z80dotNet
{
    /// <summary>
    /// Event args for the event triggered by the <see cref="IZ80Processor"/> class after an instruction is executed.
    /// </summary>
    public class AfterInstructionExecutionEventArgs : ProcessorEventArgs
    {
        /// <summary>
        /// Contains the full opcode bytes of the instruction that has been executed.
        /// </summary>
        public byte[] Opcode { get; set; }

        /// <summary>
        /// Contains the instance of <see cref="IExecutionStopper"/> that allows the event consumer
        /// to ask termination of the processor execution.
        /// </summary>
        public IExecutionStopper ExecutionStopper { get; private set; }
    }
}