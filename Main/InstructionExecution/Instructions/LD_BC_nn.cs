namespace Konamiman.Z80dotNet
{
    public partial class Z80InstructionExecutor
    {
        /// <summary>
        /// The LD BC,nn instruction.
        /// </summary>
        byte LD_BC_nn()
        {
            var value = FetchWord();
            FetchFinished();
            ProcessorAgent.Registers.Main.BC = value;
            return 10;
        }
    }
}
