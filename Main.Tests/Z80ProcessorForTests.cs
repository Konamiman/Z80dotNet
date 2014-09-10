namespace Konamiman.Z80dotNet.Tests
{
    public class Z80ProcessorForTests : Z80Processor
    {
        public void SetInstructionExecutionContextToNonNull()
        {
            executionContext = new InstructionExecutionContext();
        }

        public void SetInstructionExecutionContextToNull()
        {
            executionContext = null;
        }

        public bool HasInstructionExecutionContext
        {
            get
            {
                return executionContext != null;
            }
        }
    }
}
