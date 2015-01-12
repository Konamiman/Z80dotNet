using System;

namespace Konamiman.NestorMSX.Exceptions
{
    /// <summary>
    /// An exception that is thrown when the initialization of the emulation environment fails.
    /// </summary>
    public class EmulationEnvironmentCreationException : Exception
    {
        public EmulationEnvironmentCreationException(string message) : base(message)
        {
        }

        public EmulationEnvironmentCreationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
