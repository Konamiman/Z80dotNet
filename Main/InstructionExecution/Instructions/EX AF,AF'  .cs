namespace Konamiman.Z80dotNet
{
    public partial class Z80InstructionExecutor
    {
        /// <summary>
        /// The EX AF,AF' instruction
        /// </summary>
        byte EX_AF_AF()
        {
            FetchFinished();

            var temp = MainRegisters.AF;
            MainRegisters.AF = Registers.Alternate.AF;
            Registers.Alternate.AF = temp;

            return 4;
        }
    }
}
