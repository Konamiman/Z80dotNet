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
            var oldValue = MainRegisters.A;

            MainRegisters.A = (byte)((oldValue << 1) | (oldValue >> 7));

            MainRegisters.CF = oldValue.GetBit(7);
            MainRegisters.HF = 0;
            MainRegisters.NF = 0;

            return 4;
        }
    }
}
