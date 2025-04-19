namespace Konamiman.Z80dotNet
{
    /// <summary>
    /// Interface for implementations of IZ80Processor that support extended (16 bits) ports space.
    /// </summary>
    public interface IZ80ProcessorExtendedPortsSpace
    {
        /// <summary>
        /// Gets or sets a value indicating whether the processor is using extended (16 bits) ports space.
        /// </summary>
        bool UseExtendedPortsSpace { get; set; }

        /// <summary>
        /// Sets the access mode of a portion of the visible ports space, specifying the initial port as a 16 bit number.
        /// </summary>
        /// <param name="startPort">First port that will be set</param>
        /// <param name="length">Length of the mports space that will be set</param>
        /// <param name="mode">New memory mode</param>
        /// <exception cref="System.ArgumentException"><c>startAddress</c> is less than 0, 
        /// or <c>startAddress</c> + <c>length</c> goes beyond 255.</exception>
        void SetExtendedPortsSpaceAccessMode(ushort startPort, int length, MemoryAccessMode mode);

        /// <summary>
        /// Gets the access mode of a port, specifying the port as a 16 bit number.
        /// </summary>
        /// <param name="portNumber">The port number to check</param>
        /// <returns>The current memory access mode for the port</returns>
        /// <exception cref="System.ArgumentException"><c>portNumber</c> is greater than 255.</exception>
        MemoryAccessMode GetExtendedPortAccessMode(ushort portNumber);

        /// <summary>
        /// Sets the wait states that will be simulated when accessing the I/O ports, specifying the initial port as a 16 bit number.
        /// </summary>
        /// <param name="startPort">First port that will be configured</param>
        /// <param name="length">Length of the port range that will be configured</param>
        /// <param name="waitStates">New wait states</param>
        /// <exception cref="System.InvalidOperationException"><c>startAddress</c> + <c>length</c> goes beyond 255.</exception>
        void SetExtendedPortWaitStates(ushort startPort, int length, byte waitStates);

        /// <summary>
        /// Obtains the wait states that will be simulated when accessing the I/O ports
        /// for a given port, specifying the port as a 16 bit number. 
        /// </summary>
        /// <param name="portNumber">Port number to het the wait states for</param>
        /// <returns>Current wait states for the specified port</returns>
        byte GetExtendedPortWaitStates(ushort portNumber);
    }
}
