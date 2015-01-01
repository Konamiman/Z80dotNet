using System;
using System.Windows.Forms;

namespace Konamiman.NestorMSX.Hardware
{
    public class KeyEventArgs : EventArgs
    {
        public KeyEventArgs(Keys value)
        {
            this.Value = value;
        }

        public Keys Value { get; set; }
    }
}
