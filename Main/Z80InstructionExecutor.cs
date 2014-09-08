using System;

namespace Konamiman.Z80dotNet
{
    public class Z80InstructionExecutor : IZ80InstructionExecutor
    {
        public IZ80ProcessorAgent ProcessorAgent { get; set; }

        public int Execute(byte firstOpcodeByte)
        {
            throw new NotImplementedException();
        }

        public event EventHandler<InstructionFetchFinishedEventArgs> InstructionFetchFinished;
    }
}
