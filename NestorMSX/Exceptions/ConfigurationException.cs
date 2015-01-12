using System;

namespace Konamiman.NestorMSX.Exceptions
{
    /// <summary>
    /// An exception that is thrown when there is a problem with the configuration file.
    /// </summary>
    public class ConfigurationException : Exception
    {
        public ConfigurationException(string message) : base(message)
        {
        }

        public ConfigurationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
