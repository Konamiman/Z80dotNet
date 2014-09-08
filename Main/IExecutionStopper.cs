namespace Konamiman.Z80dotNet
{
    public interface IExecutionStopper
    {
        /// <summary>
        /// Stops the processor execution, causing the <see cref="IZ80Processor.Start"/> method to return.
        /// </summary>
        /// <param name="isPause">If true, the <see cref="IZ80Processor.StopReason"/> property of the
        /// processor classs will return <see cref="Konamiman.Z80dotNet.StopReason.PauseInvoked"/> after the method returns.
        /// Otherwise, it will return <see cref="Konamiman.Z80dotNet.StopReason.StopInvoked"/>.</param>
        void Stop(bool isPause = false);
    }
}