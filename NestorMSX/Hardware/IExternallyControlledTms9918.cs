namespace Konamiman.NestorMSX.Hardware
{
    /// <summary>
    /// Represents a TMS9918 video display processor that can be controlled externally,
    /// that is, by explicit screen mode selection and VRAM access methods.
    /// </summary>
    public interface IExternallyControlledTms9918 : ITms9918
    {
        /// <summary>
        /// Writes one byte to VRAM.
        /// </summary>
        /// <param name="address">VRAM address</param>
        /// <param name="value">Value to write</param>
        void WriteVram(int address, byte value);

        /// <summary>
        /// Reads one byte from VRAM.
        /// </summary>
        /// <param name="address">VRAM address</param>
        /// <returns>Value read from VRAM</returns>
        byte ReadVram(int address);

        /// <summary>
        /// Gets the contents of a portion of VRAM
        /// </summary>
        /// <param name="startAddress">First address</param>
        /// <param name="length">Amount of bytes to read</param>
        /// <returns></returns>
        byte[] GetVramContents(int startAddress, int length);

        /// <summary>
        /// Sets the contents of a portion of VRAM
        /// </summary>
        /// <param name="startAddress">First VRAM address that will be set</param>
        /// <param name="contents">New contents of VRAM</param>
        /// <param name="startIndex">Start index for starting copying within the contens array</param>
        /// <param name="length">Length of the contents array that will be copied. If null,
        /// the whole array is copied.</param>
        void SetVramContents(int startAddress, byte[] contents, int startIndex = 0, int? length = null);
    }
}
