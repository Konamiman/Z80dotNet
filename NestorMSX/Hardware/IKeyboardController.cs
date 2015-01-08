namespace Konamiman.NestorMSX.Hardware
{
    /// <summary>
    /// Represents a MSX keyboard controller.
    /// </summary>
    public interface IKeyboardController
    {
        /// <summary>
        /// Writes a value to the keyboard matrix row selection register.
        /// </summary>
        /// <param name="value">Value to write</param>
        void WriteToKeyboardMatrixRowSelectionRegister(byte value);

        /// <summary>
        /// Reads the current value from the keyboard matrix row input register.
        /// </summary>
        /// <returns>Current value of the register</returns>
        byte ReadFromKeyboardMatrixRowInputRegister();
    }
}
