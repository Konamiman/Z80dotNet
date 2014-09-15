using System;

namespace Konamiman.Z80dotNet
{
    public partial class Z80InstructionExecutor
    {
        private Func<byte>[] CB_InstructionExecutors;

        private void Initialize_CB_InstructionsTable()
        {
            CB_InstructionExecutors = new Func<byte>[]
            {
                RLC_B,        //00
                RLC_C         //01
            };
        }

        //TODO: Move instructions to their own files and include them in the table when they are implemented.

        /// <summary>
        /// The RLC B instruction.
        /// </summary>
        byte RLC_B()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RLC C instruction.
        /// </summary>
        byte RLC_C()
        {
            throw new NotImplementedException();
        }
    }
}
