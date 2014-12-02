namespace Konamiman.Z80dotNet
{
    public partial class Z80InstructionExecutor
    {
        /// <summary>
        /// The LD I,A instruction.
        /// </summary>
        byte LD_I_A()
        {
            FetchFinished();

            Registers.I = Registers.A;

            return 9;
        }
    }
}
