using System;

namespace Konamiman.Z80dotNet
{
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
