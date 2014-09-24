namespace Konamiman.Z80dotNet
{
    public partial class Z80InstructionExecutor
    {
        /// <summary>
        /// The DI instruction.
        /// </summary>
        byte DI()
        {
            FetchFinished();

            Registers.IFF1 = 0;
            Registers.IFF2 = 0;

            return 4;
        }
    }
}
