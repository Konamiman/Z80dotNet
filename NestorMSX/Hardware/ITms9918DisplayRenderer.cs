namespace Konamiman.NestorMSX.Hardware
{
    /// <summary>
    /// Represents a device that can be connected to a TMS9918 processor
    /// and render the video data that it produces.
    /// </summary>
    public interface ITms9918DisplayRenderer
    {
        /// <summary>
        /// Enables the screen, displaying again all the data in the pattern name table.
        /// </summary>
        void ActivateScreen();

        /// <summary>
        /// Disables the screen, showing just the backdrop color.
        /// </summary>
        void BlankScreen();

        /// <summary>
        /// Notifies of a change of the screen mode.
        /// </summary>
        /// <param name="mode">New screen mode, a number from 0 to 3.</param>
        void SetScreenMode(byte mode);

        /// <summary>
        /// Notifies of a byte written in the pattern name table.
        /// </summary>
        /// <param name="position">Offset in the table.</param>
        /// <param name="value">Value written.</param>
        void WriteToNameTable(int position, byte value);

        /// <summary>
        /// Notifies of a byte written in the pattern generator table.
        /// </summary>
        /// <param name="position">Offset in the table.</param>
        /// <param name="value">Value to write.</param>
        void WriteToPatternGeneratorTable(int position, byte value);

        /// <summary>
        /// Notifies of a byte written in the color generator table.
        /// </summary>
        /// <param name="position">Offset in the table.</param>
        /// <param name="value">Value to write.</param>
        void WriteToColourTable(int position, byte value);

        /// <summary>
        /// Notifies of a change of the text color.
        /// </summary>
        /// <param name="colorIndex">Color index of the new text color.</param>
        void SetTextColor(byte colorIndex);
        
        /// <summary>
        /// Notifies of a change of the backdrop color.
        /// </summary>
        /// <param name="colorIndex">Color index of the new backdrop color.</param>
        void SetBackdropColor(byte colorIndex);
    }
}
