using System;

namespace Konamiman.Z80dotNet
{
    public partial class Z80InstructionExecutor
    {
        private Func<byte>[] FDCB_InstructionExecutors;

        private void Initialize_FDCB_InstructionsTable()
        {
            FDCB_InstructionExecutors = new Func<byte>[]
            {
                RLC_IY_plus_n_and_copy_to_B,         //00
                RLC_IY_plus_n_and_copy_to_C          //01
            };
        }

        //TODO: Move instructions to their own files and include them in the table when they are implementFDCB.

        /// <summary>
        /// The RLC(IY+n) and copy to B instruction.
        /// </summary>
        byte RLC_IY_plus_n_and_copy_to_B()
        {
            FetchFinished();
            return 23;
        }

        /// <summary>
        /// The RLC(IY+n) and copy to C instruction.
        /// </summary>
        byte RLC_IY_plus_n_and_copy_to_C()
        {
            FetchFinished();
            return 23;
        }
    }
}
