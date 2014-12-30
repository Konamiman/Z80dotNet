using Konamiman.Z80dotNet;

namespace Konamiman.NestorMSX.Hardware
{
    /// <summary>
    /// Represents a TMS9918 video display processor.
    /// </summary>
    public interface ITms9918 : IZ80InterruptSource
    {
        /// <summary>
        /// Writes a value to the specified port number
        /// </summary>
        /// <param name="portNumber">The port number to write</param>
        /// <param name="value">The value to be written</param>
        void WriteToPort(Bit portNumber, byte value);

        /// <summary>
        /// Reads a value from the specified port number
        /// </summary>
        /// <param name="portNumber">The port number to read</param>
        /// <returns>The value to be read</returns>
        byte ReadFromPort(Bit portNumber);
    }
}
