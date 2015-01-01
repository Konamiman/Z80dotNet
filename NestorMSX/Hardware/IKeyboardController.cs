namespace Konamiman.NestorMSX.Hardware
{
    public interface IKeyboardController
    {
        void WriteToKeyboardMatrixRowSelectionRegister(byte value);

        byte ReadFromKeyboardMatrixRowInputRegister();
    }
}
