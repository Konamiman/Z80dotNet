namespace Konamiman.NestorMSX.Hardware
{
    /// <summary>
    /// Represents a device that can be connected to a TMS9918 processor
    /// and render the video data that it produces.
    /// </summary>
    public interface ITms9918DisplayRenderer
    {
        void ActivateScreen();

        void BlankScreen();

        void SetScreenMode(byte mode);

        void WriteToNameTable(int position, byte value);

        void WriteToPatternGeneratorTable(int position, byte value);

        void WriteToColourTable(int position, byte value);

        void SetForegroundColor(byte colorIndex);

        void SetBackgroundColor(byte colorIndex);
    }
}
