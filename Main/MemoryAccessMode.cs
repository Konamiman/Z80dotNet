namespace Konamiman.Z80dotNet
{
    /// <summary>
    /// Represents the access mode for a certain memory address.
    /// </summary>
    public enum MemoryAccessMode
    {
        /// <summary>
        /// RAM: memory can be read or written.
        /// </summary>
        RAM,

        /// <summary>
        /// ROM: memory is read-only. Writes will appropriately affect timing but will have no effect.
        /// </summary>
        ROM,

        /// <summary>
        /// Not connected: reads and writes will appropriately affect timing, but
        /// reads will aways return FFh and writes will have no effect.
        /// </summary>
        NotConnected
    }
}