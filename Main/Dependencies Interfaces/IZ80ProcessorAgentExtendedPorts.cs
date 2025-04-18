namespace Konamiman.Z80dotNet
{
    /// <summary>
    /// Complements <see cref="IZ80ProcessorAgent"/> with methods for accessing the extended (16 bits) ports space.
    /// </summary>
    public interface IZ80ProcessorAgentExtendedPorts
    {
        /// <summary>
        /// Reads one byte from an I/O port, specifying the port as a 16 bit number.
        /// </summary>
        /// <param name="portNumberLow">Port number to read from (low byte)</param>
        /// <param name="portNumberHigh">Port number to read from (high byte)</param>
        /// <returns>Obtained byte</returns>
        byte ReadFromPort(byte portNumberLow, byte portNumberHigh);

        /// <summary>
        /// Writes one byte to an I/O port, specifying the port as a 16 bit number.
        /// </summary>
        /// <param name="portNumberLow">Port number to write to (low byte)</param>
        /// <param name="portNumberHigh">Port number to write to (high byte)</param>
        /// <param name="value">Value to write</param>
        void WriteToPort(byte portNumberLow, byte portNumberHigh, byte value);
    }
}
