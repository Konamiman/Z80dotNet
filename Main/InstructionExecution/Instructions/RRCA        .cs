namespace Konamiman.Z80dotNet
{
    public partial class Z80InstructionExecutor
    {
        /// <summary>
        /// The RRCA instruction
        /// </summary>
        byte RRCA()
        {
            FetchFinished();
            var oldValue = Registers.A;

            Registers.A = (byte)((oldValue >> 1) | (oldValue << 7));

            Registers.CF = oldValue.GetBit(0);
            Registers.HF = 0;
            Registers.NF = 0;

            return 4;
        }
    }
}
