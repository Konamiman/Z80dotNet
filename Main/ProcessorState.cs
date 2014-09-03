namespace Konamiman.Z80dotNet
{
    /// <summary>
    /// Represents the current processor state
    /// </summary>
    public enum ProcessorState
    {
        /// <summary>
        /// Not running. Either the processor has never been running since it was instantiated,
        /// or the <see cref="IZ80Processor.Start"/> or <see cref="IZ80Processor.ExecuteNextInstruction"/>
        /// methods were invoked and finished for any reason other than the invocation of <see cref="IZ80Processor.Stop"/>
        /// with <c>isPause=true</c>.
        /// </summary>
        Stopped,

        /// <summary>
        /// Not running. The <see cref="IZ80Processor.Start"/> or <see cref="IZ80Processor.ExecuteNextInstruction"/>
        /// were invoked and finished with an invocation of <see cref="IZ80Processor.Stop"/>
        /// with <c>isPause=true</c>.
        /// </summary>
        Paused,

        /// <summary>
        /// Running. The <see cref="IZ80Processor.Start"/> was invoked and has not returned yet.
        /// </summary>
        Running,

        /// <summary>
        /// Executing only one instruction. The <see cref="IZ80Processor.ExecuteNextInstruction"/> was invoked
        /// and has not returned yet.
        /// </summary>
        ExecutingOneInstruction
    }
}