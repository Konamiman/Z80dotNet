namespace Konamiman.Z80dotNet
{
    /// <summary>
    /// Event args for the event triggered by the <see cref="IZ80Processor"/> class after an instruction is executed.
    /// </summary>
    public class AfterInstructionExecutionEventArgs : ProcessorEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="opcode">The opcode bytes of the instruction that has been executed.</param>
        /// <param name="stopper">An instance of <see cref="IExecutionStopper"/> that can be used
        /// by the event listener to request stop of the execution loop.</param>
        /// <param name="localUserState">The state object from the matching <see cref="IZ80Processor.BeforeInstructionExecution"/> event.</param>
        public AfterInstructionExecutionEventArgs(byte[] opcode, IExecutionStopper stopper, object localUserState)
        {
            this.Opcode = opcode;
            this.ExecutionStopper = stopper;
            this.LocalUserState = localUserState;
        }

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