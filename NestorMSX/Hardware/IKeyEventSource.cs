using System;

namespace Konamiman.NestorMSX.Hardware
{
    public interface IKeyEventSource
    {
        event EventHandler<KeyEventArgs> KeyPressed;

        event EventHandler<KeyEventArgs> KeyReleased;
    }
}
