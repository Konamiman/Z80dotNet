using System;

namespace Konamiman.Z80dotNet
{
    /// <summary>
    /// Default implementation of <see cref="IPeriodWaiter"/>.
    /// </summary>
    public class PeriodWaiter : IPeriodWaiter
    {
        public int EffecttiveClockSpeedInMHz { get; set; }

        public void PeriodElapsed(int periodLengthInCycles)
        {
            throw new NotImplementedException();
        }
    }
}
