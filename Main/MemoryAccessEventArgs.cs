namespace Konamiman.Z80dotNet
{
    /// <summary>
    /// Event triggered by the <see cref="IZ80Processor"/> class before and after a memory address or port access.
    /// </summary>
    public class MemoryAccessEventArgs : ProcessorEventArgs
    {
        /// <summary>
        /// Gets the type of event being processed. 
        /// </summary>
        public MemoryAccessEventType EventType { get; private set; }

        /// <summary>
        /// Gets the memory or port address being accessed.
        /// </summary>
        public ushort Address { get; private set; }

        /// <summary>
        /// Gets the value that has been read, will be written, or has been written.
        /// Before a read it does not contain any meaningful value.
        /// </summary>
        public byte Value { get; set; }
    }
}