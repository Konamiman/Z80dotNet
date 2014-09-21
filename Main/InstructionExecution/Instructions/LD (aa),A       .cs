namespace Konamiman.Z80dotNet
{
    public partial class Z80InstructionExecutor
    {
        /// <summary>
        /// The LD (aa),A instruction.
        /// </summary>
        private byte LD_aa_A()
        {
            var address = FetchWord().ToUShort();
            FetchFinished();

            ProcessorAgent.WriteToMemory(address, Registers.A);

            return 13;
        }
    }
}
