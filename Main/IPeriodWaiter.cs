namespace Konamiman.Z80dotNet
{
    /// <summary>
    /// Represents a class that helps on introducing delays that last for a specified amount of time.
    /// </summary>
    public interface IPeriodWaiter
    {
        /// <summary>
        /// Signals the start a new time period.
        /// </summary>
        /// <param name="effectiveClockSpeedInMHz">Clock speed of the simulated system, in MHz.</param>
        /// <returns>A status object that must be passed back to <see cref="WaitToEndOfPeriod"/>.</returns>
        object StartPeriod(int effectiveClockSpeedInMHz);

        /// <summary>
        /// Stops the current thread for long enough so that the time passed since the execution of
        /// <see cref="StartPeriod"/> is equivalent to the specified cycles at the specified clock speed.
        /// </summary>
        /// <param name="startPeriodState">State object that was returned by <see cref="StartPeriod"/>.</param>
        /// <param name="periodLengthInCycles">Total period length in clock cycles.</param>
        /// <example>If <see cref="StartPeriod"/> was invoked with a clock speed of 4MHz,
        /// and this method is invoked with a period length of 4, then this method will pause
        /// the current thread until 1 microsecond has elapsed since the invocation of <see cref="StartPeriod"/>.</example>
        void WaitToEndOfPeriod(object startPeriodState, int periodLengthInCycles);
    }
}
