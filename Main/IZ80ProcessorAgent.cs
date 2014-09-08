namespace Konamiman.Z80dotNet
{
    /// <summary>
    /// Represents a class that allows to perform a limited set of operations on an <see cref="IZ80Processor"/>.
    /// </summary>
    public interface IZ80ProcessorAgent : IExecutionStopper
    {
        /// <summary>
        /// Reads the next opcode byte from the memory location currently pointed by the PC register,
        /// then increases PC by one.
        /// </summary>
        /// <returns>The byte obtained from memory</returns>
        byte FetchNextOpcode();

        /// <summary>
        /// Reads one byte from memory.
        /// </summary>
        /// <param name="address">Memory address to read from</param>
        /// <returns>Obtained byte</returns>
        byte ReadFromMemory(ushort address);

        /// <summary>
        /// Writes one byte to memory.
        /// </summary>
        /// <param name="address">Memory address to write to</param>
        /// <param name="value">Value to write</param>
        void WriteToMemory(ushort address, byte value);

        /// <summary>
        /// Reads one byte from an I/O port.
        /// </summary>
        /// <param name="portNumber">Port number to read from</param>
        /// <returns>Obtained byte</returns>
        byte ReadFromPort(byte portNumber);

        /// <summary>
        /// Writes one byte to an I/O port.
        /// </summary>
        /// <param name="portNumber">Port number to write to</param>
        /// <param name="value">Value to write</param>
        void WriteToPort(byte portNumber, byte value);

        /// <summary>
        /// Returns the current register set used by the processor.
        /// </summary>
        IZ80Registers Registers { get; }

        /// <summary>
        /// Changes the current interrupt mode.
        /// </summary>
        /// <param name="interruptMode">The new interrupt mode.</param>
        /// <exception cref="System.InvalidOperationException">Attempt to set a value other than 0, 1 or 2</exception>
        void SetInterruptMode(int interruptMode);
    }
}
