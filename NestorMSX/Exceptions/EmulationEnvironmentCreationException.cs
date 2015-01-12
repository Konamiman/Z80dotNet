using System;

namespace Konamiman.NestorMSX.Exceptions
{
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
