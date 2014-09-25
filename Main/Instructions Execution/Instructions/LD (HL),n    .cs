namespace Konamiman.Z80dotNet
{
    public partial class Z80InstructionExecutor
    {
        /// <summary>
        /// The LD (nn),A instruction.
        /// </summary>
        private byte LD_aHL_n()
        {
            var value = ProcessorAgent.FetchNextOpcode();
            FetchFinished();

            ProcessorAgent.WriteToMemory(Registers.HL.ToUShort(), value);

            return 10;
        }
    }
}
