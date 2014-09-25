namespace Konamiman.Z80dotNet
{
    public partial class Z80InstructionExecutor
    {
        /// <summary>
        /// The RLCA instruction
        /// </summary>
        byte RLCA()
        {
            FetchFinished();
            var oldValue = Registers.A;

            Registers.A = (byte)((oldValue << 1) | (oldValue >> 7));

            Registers.CF = oldValue.GetBit(7);
            Registers.HF = 0;
            Registers.NF = 0;
            SetFlags3and5From(Registers.A);

            return 4;
        }
    }
}
