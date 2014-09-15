using System;

namespace Konamiman.Z80dotNet
{
    /// <summary>
    /// Default implementation of <see cref="IZ80InstructionExecutor"/>.
    /// </summary>
    public partial class Z80InstructionExecutor : IZ80InstructionExecutor
    {
        public IZ80ProcessorAgent ProcessorAgent { get; set; }

        public Z80InstructionExecutor()
        {
            Initialize_CB_InstructionsTable();
            Initialize_DD_InstructionsTable();
            Initialize_DDCB_InstructionsTable();
            Initialize_ED_InstructionsTable();
            Initialize_FD_InstructionsTable();
            Initialize_FDCB_InstructionsTable();
            Initialize_SingleByte_InstructionsTable();
        }

        public int Execute(byte firstOpcodeByte)
        {
            switch(firstOpcodeByte)
            {
                case 0xCB:
                    return Execute_CB_Instruction();
                case 0xDD:
                    return Execute_DD_Instruction();
                case 0xED:
                    return Execute_ED_Instruction();
                case 0xFD:
                    return Execute_FD_Instruction();
                default:
                    return Execute_SingleByte_Instruction(firstOpcodeByte);
            }
        }

        private int Execute_CB_Instruction()
        {
            Inc_R();
            Inc_R();
            return CB_InstructionExecutors[ProcessorAgent.FetchNextOpcode()]();
        }

        private int Execute_ED_Instruction()
        {
            Inc_R();
            Inc_R();
            var secondOpcodeByte = ProcessorAgent.FetchNextOpcode();
            if (secondOpcodeByte < 0x40 || secondOpcodeByte >= 0xBF)
                return ExecuteUnsopported_ED_Instruction(secondOpcodeByte);
            else
                return ED_InstructionExecutors[secondOpcodeByte - 0x40]();
        }

        /// <summary>
        /// Executes an unsupported ED instruction, that is, an instruction whose opcode is
        /// ED xx, where xx is 00-40 or C0-FF.
        /// </summary>
        /// <param name="secondOpcodeByte">The opcode byte fetched after the 0xED.</param>
        /// <returns>The total amount of T states required for the instruction execution.</returns>
        /// <remarks>You can override this method in derived classes in order to implement a custom
        /// behavior for these unsupported instructions (for example, to implement the multiplication
        /// instructions of the R800 processor).</remarks>
        protected virtual int ExecuteUnsopported_ED_Instruction(byte secondOpcodeByte)
        {
            FetchFinished();
            return 8;
        }

        private int Execute_SingleByte_Instruction(byte firstOpcodeByte)
        {
            Inc_R();
            return SingleByte_InstructionExecutors[firstOpcodeByte]();
        }

        private void FetchFinished(bool isRet = false, bool isHalt = false, bool isLdSp = false)
        {
            InstructionFetchFinished(this, new InstructionFetchFinishedEventArgs()
            {
                IsRetInstruction = isRet,
                IsHaltInstruction = isHalt,
                IsLdSpInstruction = isLdSp
            });
        }

        public event EventHandler<InstructionFetchFinishedEventArgs> InstructionFetchFinished;

        private void Inc_R()
        {
            ProcessorAgent.Registers.R = ProcessorAgent.Registers.R.Inc7Bits();
        }
    }
}
