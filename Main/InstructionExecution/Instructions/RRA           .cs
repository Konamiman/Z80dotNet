namespace Konamiman.Z80dotNet
{
    public partial class Z80InstructionExecutor
    {
        /// <summary>
        /// The RRA instruction.
        /// </summary>
        byte RRA()
        {
            FetchFinished();

            var oldValue = Registers.A;
            Registers.A = (byte)((oldValue >> 1) | (Registers.CF ? 0x80 : 0));
            Registers.CF = oldValue.GetBit(0);

            Registers.NF = 0;
            Registers.HF = 0;

            return 4;
        }
    }
}
