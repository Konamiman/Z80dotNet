using System;

namespace Konamiman.Z80dotNet
{
    /// <summary>
    /// Default implementation of <see cref="IPeriodWaiter"/>.
    /// </summary>
    public class PeriodWaiter : IPeriodWaiter
    {
        public object StartPeriod(int effectiveClockSpeedInMHz)
        {
            throw new NotImplementedException();
        }

        public void WaitToEndOfPeriod(object startPeriodState, int periodLengthInCycles)
        {
            throw new NotImplementedException();
        }
    }
}
