namespace Konamiman.Z80dotNet
{
    public partial class Z80InstructionExecutor
    {
        private const int HF_NF_reset = 0xED;
        private const int CF_set = 1;

        /// <summary>
        /// The SCF instruction.
        /// </summary>
        byte SCF()
        {
            FetchFinished();

            Registers.F = (byte)(Registers.F & HF_NF_reset | CF_set);
            Registers.Flag3 = Registers.A.GetBit(3);
            Registers.Flag5 = Registers.A.GetBit(5);

            return 4;
        }
    }
}
