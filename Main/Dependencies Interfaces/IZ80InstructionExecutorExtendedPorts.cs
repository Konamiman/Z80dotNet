namespace Konamiman.Z80dotNet
{
    /// <summary>
    /// Complements <see cref="IZ80InstructionExecutor"/> with a property of type <see cref="IZ80ProcessorAgentExtendedPorts"/>.
    /// </summary>
    public interface IZ80InstructionExecutorExtendedPorts
    {
        /// <summary>
        /// The <see cref="IZ80ProcessorAgentExtendedPorts"/> that the <see cref="Execute"/> method will use in order
        /// to interact with the processor.
        /// </summary>
        IZ80ProcessorAgentExtendedPorts ProcessorAgentExtendedPorts { get; set; }
    }
}
