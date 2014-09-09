using System;

namespace Konamiman.Z80dotNet
{
    /// <summary>
    /// Default implementation of <see cref="IClockSynchronizationHelper"/>.
    /// </summary>
    public class ClockSynchronizationHelper : IClockSynchronizationHelper
    {
        public decimal EffecttiveClockSpeedInMHz { get; set; }

        public void TryWait(int periodLengthInCycles)
        {
            throw new NotImplementedException();
        }
    }
}
