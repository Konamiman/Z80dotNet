using System;
using Konamiman.NestorMSX.Hardware;

namespace Konamiman.NestorMSX.Host
{
    /// <summary>
    /// Represents a source for host keyboard events.
    /// </summary>
    public interface IKeyEventSource
    {
        /// <summary>
        /// Notifies the class that it should starts generating keyboard events.
        /// </summary>
        void StartGeneratingKeyEvents();

        /// <summary>
        /// Notifies the class that it should stop generating keyboard events.
        /// </summary>
        void StopGeneratingKeyEvents();

        /// <summary>
        /// Event triggered when a key is pressed in the host.
        /// </summary>
        event EventHandler<KeyEventArgs> KeyPressed;

        /// <summary>
        /// Event triggered when a key is released in the host.
        /// </summary>
        event EventHandler<KeyEventArgs> KeyReleased;
    }
}
