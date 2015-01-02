using System;

namespace Konamiman.NestorMSX.Hardware
{
    public interface IKeyEventSource
    {
        void Start();

        void Stop();

        event EventHandler<KeyEventArgs> KeyPressed;

        event EventHandler<KeyEventArgs> KeyReleased;
    }
}
