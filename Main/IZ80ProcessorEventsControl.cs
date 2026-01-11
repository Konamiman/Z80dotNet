namespace Konamiman.Z80dotNet
{
    /// <summary>
    /// Interface for controlling which memory and port access events are fired by the processor.
    /// </summary>
    /// <remarks>
    /// <para>
    /// By default, <see cref="IZ80Processor.MemoryAccess"/> events are fired for all memory and port accesses.
    /// This interface allows configuring specific address blocks for which these events will be fired,
    /// which can significantly improve emulation performance when event handling is only needed
    /// for certain memory or port regions (e.g., memory-mapped I/O, video RAM).
    /// </para>
    /// <para>
    /// Memory is logically divided into 256 blocks of 256 bytes each (0x0000-0x00FF, 0x0100-0x01FF, etc.).
    /// Each property accepts an array of block base addresses (addresses with the format 0xXX00).
    /// Events are fired only when the accessed address falls within one of the configured blocks.
    /// Setting an empty array disables events entirely for that access type.
    /// </para>
    /// <para>
    /// Input addresses are automatically masked to block boundaries (address &amp; 0xFF00),
    /// and duplicates are removed. For example, input [0x1234, 0x1299, 0x2000] becomes [0x1200, 0x2000].
    /// </para>
    /// </remarks>
    public interface IZ80ProcessorEventsControl
    {
        /// <summary>
        /// Gets or sets the memory block base addresses for which read events will be fired.
        /// </summary>
        /// <remarks>
        /// <para>
        /// When set, <see cref="MemoryAccessEventType.BeforeMemoryRead"/> and
        /// <see cref="MemoryAccessEventType.AfterMemoryRead"/> events will only be fired
        /// for addresses that fall within one of the specified 256-byte blocks.
        /// </para>
        /// <para>
        /// The default value covers all 256 blocks (entire memory space 0x0000-0xFFFF),
        /// which maintains backwards compatibility by firing events for all memory reads.
        /// </para>
        /// <para>
        /// Setting this to an empty array disables all memory read events.
        /// </para>
        /// </remarks>
        ushort[] MemoryAreasWithReadEvents { get; set; }

        /// <summary>
        /// Gets or sets the memory block base addresses for which write events will be fired.
        /// </summary>
        /// <remarks>
        /// <para>
        /// When set, <see cref="MemoryAccessEventType.BeforeMemoryWrite"/> and
        /// <see cref="MemoryAccessEventType.AfterMemoryWrite"/> events will only be fired
        /// for addresses that fall within one of the specified 256-byte blocks.
        /// </para>
        /// <para>
        /// The default value covers all 256 blocks (entire memory space 0x0000-0xFFFF),
        /// which maintains backwards compatibility by firing events for all memory writes.
        /// </para>
        /// <para>
        /// Setting this to an empty array disables all memory write events.
        /// </para>
        /// </remarks>
        ushort[] MemoryAreasWithWriteEvents { get; set; }

        /// <summary>
        /// Gets or sets the port addresses or block base addresses for which read events will be fired.
        /// </summary>
        /// <remarks>
        /// <para>
        /// When set, <see cref="MemoryAccessEventType.BeforePortRead"/> and
        /// <see cref="MemoryAccessEventType.AfterPortRead"/> events will only be fired
        /// for port numbers that match the configured addresses.
        /// </para>
        /// <para>
        /// In standard 256-port mode, each value represents an individual port number (0x00-0xFF).
        /// In extended 64K port mode (<see cref="IZ80ProcessorExtendedPortsSpace.UseExtendedPortsSpace"/>),
        /// values represent 256-port block base addresses, similar to memory blocks.
        /// </para>
        /// <para>
        /// The default value covers all ports. This property is reset to default when
        /// <see cref="IZ80ProcessorExtendedPortsSpace.UseExtendedPortsSpace"/> is changed.
        /// </para>
        /// <para>
        /// Setting this to an empty array disables all port read events.
        /// </para>
        /// </remarks>
        ushort[] PortAreasWithReadEvents { get; set; }

        /// <summary>
        /// Gets or sets the port addresses or block base addresses for which write events will be fired.
        /// </summary>
        /// <remarks>
        /// <para>
        /// When set, <see cref="MemoryAccessEventType.BeforePortWrite"/> and
        /// <see cref="MemoryAccessEventType.AfterPortWrite"/> events will only be fired
        /// for port numbers that match the configured addresses.
        /// </para>
        /// <para>
        /// In standard 256-port mode, each value represents an individual port number (0x00-0xFF).
        /// In extended 64K port mode (<see cref="IZ80ProcessorExtendedPortsSpace.UseExtendedPortsSpace"/>),
        /// values represent 256-port block base addresses, similar to memory blocks.
        /// </para>
        /// <para>
        /// The default value covers all ports. This property is reset to default when
        /// <see cref="IZ80ProcessorExtendedPortsSpace.UseExtendedPortsSpace"/> is changed.
        /// </para>
        /// <para>
        /// Setting this to an empty array disables all port write events.
        /// </para>
        /// </remarks>
        ushort[] PortAreasWithWriteEvents { get; set; }
    }
}
