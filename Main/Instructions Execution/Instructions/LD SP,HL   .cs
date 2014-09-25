namespace Konamiman.Z80dotNet
{
    public partial class Z80InstructionExecutor
    {
        /// <summary>
        /// The LD SP,HL instruction.
        /// </summary>
        byte LD_SP_HL()
        {
            FetchFinished(isLdSp: true);

            Registers.SP = Registers.HL;

            return 6;
        }
    }
}
