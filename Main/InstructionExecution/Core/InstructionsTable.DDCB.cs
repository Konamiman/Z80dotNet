using System;

namespace Konamiman.Z80dotNet
{
    public partial class Z80InstructionExecutor
    {
        private Func<byte>[] DDCB_InstructionExecutors;

        private void Initialize_DDCB_InstructionsTable()
        {
            DDCB_InstructionExecutors = new Func<byte>[]
            {
                RLC_IX_plus_n_and_copy_to_B,         //00
                RLC_IX_plus_n_and_copy_to_C          //01
            };
        }

        //TODO: Move instructions to their own files and include them in the table when they are implementDDCB.

        /// <summary>
        /// The RLC(IX+n) and copy to B instruction.
        /// </summary>
        byte RLC_IX_plus_n_and_copy_to_B()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RLC(IX+n) and copy to C instruction.
        /// </summary>
        byte RLC_IX_plus_n_and_copy_to_C()
        {
            throw new NotImplementedException();
        }
    }
}
