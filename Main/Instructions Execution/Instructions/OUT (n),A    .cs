namespace Konamiman.Z80dotNet
{
    public partial class Z80InstructionExecutor
    {
        /// <summary>
        /// The OUT (n),A instruction.
        /// </summary>
        byte OUT_n_A()
        {
            var portNumber = ProcessorAgent.FetchNextOpcode();
            FetchFinished();

            ProcessorAgentExtendedPorts.WriteToPort(portNumber, Registers.A, Registers.A);

            return 11;
        }
    }
}
