namespace Konamiman.Z80dotNet
{
    public partial class Z80InstructionExecutor
    {
        /// <summary>
        /// The JP (HL) instruction.
        /// </summary>
        byte JP_aHL()
        {
            FetchFinished();

            Registers.PC = Registers.HL.ToUShort();

            return 4;
        }
    }
}
