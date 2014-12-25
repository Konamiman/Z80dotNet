using System.Threading;

namespace Konamiman.Z80dotNet.ZexallTest
{
    class DummyClockSynchronizer : IClockSynchronizer
    {
        public decimal EffectiveClockFrequencyInMHz { get; set; }

        public void Start()
        {
        }

        public void Stop()
        {
        }

        public void TryWait(int periodLengthInCycles)
        {
        }
    }
}
