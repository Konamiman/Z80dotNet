using System;

namespace Konamiman.Z80dotNet.Tests
{
    public class InterruptSourceForTests : IZ80InterruptSource
    {
        public void FireNmi()
        {
            NmiInterruptPulse?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler NmiInterruptPulse;
        public bool IntLineIsActive { get; set; }
        public byte? ValueOnDataBus { get; set; }
    }
}
