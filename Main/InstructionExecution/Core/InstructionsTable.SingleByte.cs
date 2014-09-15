using System;

namespace Konamiman.Z80dotNet
{
    public partial class Z80InstructionExecutor
    {
        private Func<byte>[] SingleByte_InstructionExecutors;

        private void Initialize_SingleByte_InstructionsTable()
        {
            SingleByte_InstructionExecutors = new Func<byte>[]
            {
                NOP,        //00
                LD_BC_nn    //01
            };
        }

        //TODO: Move instructions to their own files and include them in the table when they are implemented.

    }
}
