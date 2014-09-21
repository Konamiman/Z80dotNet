namespace Konamiman.Z80dotNet
{
    public partial class Z80InstructionExecutor
    {
        /// <summary>
        /// The CCF instruction.
        /// </summary>
        byte CCF()
        {
            FetchFinished();

            var oldCF = Registers.CF;
            Registers.NF = 0;
            Registers.HF = oldCF;
            Registers.CF = !oldCF;
            Registers.Flag3 = Registers.A.GetBit(3);
            Registers.Flag5 = Registers.A.GetBit(5);

            return 4;
        }
    }
}
