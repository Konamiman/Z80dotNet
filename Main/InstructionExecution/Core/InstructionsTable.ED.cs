using System;

namespace Konamiman.Z80dotNet
{
    public partial class Z80InstructionExecutor
    {
        private Func<byte>[] ED_InstructionExecutors;

        private void Initialize_ED_InstructionsTable()
        {
            ED_InstructionExecutors = new Func<byte>[]
            {
                IN_B_C,         //40
                OUT_C_B         //41
            };
        }

        //TODO: Move instructions to their own files and include them in the table when they are implemented.

        /// <summary>
        /// The IN B,(C) instruction.
        /// </summary>
        byte IN_B_C()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The OUT (C),B instruction.
        /// </summary>
        byte OUT_C_B()
        {
            throw new NotImplementedException();
        }
    }
}
