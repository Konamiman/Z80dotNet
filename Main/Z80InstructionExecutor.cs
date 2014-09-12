using System;

namespace Konamiman.Z80dotNet
{
    /// <summary>
    /// Default implementation of <see cref="IZ80InstructionExecutor"/>.
    /// </summary>
    public class Z80InstructionExecutor : IZ80InstructionExecutor
    {
        public IZ80ProcessorAgent ProcessorAgent { get; set; }

        public int Execute(byte firstOpcodeByte)
        {
            byte value;

            switch(firstOpcodeByte)
            {
                case 0x3E:  //LD A,n
                    value = ProcessorAgent.FetchNextOpcode();
                    FireFetchFinished();
                    ProcessorAgent.Registers.Main.A = value;
                    return 7;
                case 0xC6:  //ADD A,n
                    value = ProcessorAgent.FetchNextOpcode();
                    FireFetchFinished();
                    ProcessorAgent.Registers.Main.A += value;   //TODO: Check for overflow, set flags
                    return 4;
                case 0x3C:  //INC A
                    FireFetchFinished();
                    ProcessorAgent.Registers.Main.A++;  //TODO: Check for overflow, set flags
                    return 4;
                case 0xC9:  //RET
                    FireFetchFinished(true);
                    //TODO: Update PC and SP
                    return 10;
                default:    //treat as NOP
                    return 4;
            }
        }

        private void FireFetchFinished(bool isRet = false)
        {
            InstructionFetchFinished(this, new InstructionFetchFinishedEventArgs() {IsRetInstruction = isRet});
        }

        public event EventHandler<InstructionFetchFinishedEventArgs> InstructionFetchFinished;
    }
}
