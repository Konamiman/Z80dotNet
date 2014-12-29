using System;

namespace Konamiman.NestorMSX.Hardware
{
    public class SlotSelectionRegisterWrittenEventArgs : EventArgs
    {
        public byte Value { get; private set; }

        public SlotSelectionRegisterWrittenEventArgs(byte value)
        {
            this.Value = value;
        }
    }
}
