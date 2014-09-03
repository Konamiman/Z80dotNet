namespace Konamiman.Z80dotNet
{
    /// <summary>
    /// Represents the type of an instruction execution event. 
    /// </summary>
    public enum InstructionExecutionEventType
    {
        /// <summary>
        /// Event triggered before an instruction is going to execute.
        /// </summary>
        BeforeInstructionExecution,

        /// <summary>
        /// Event triggered after an instruction has been executed.
        /// </summary>
        AfterInstructionExecution
    }
}