using System;
using System.Collections.Generic;

namespace Konamiman.Z80dotNet
{
    public partial class Z80InstructionExecutor
    {
        private IDictionary<byte, Func<byte>> FD_InstructionExecutors;

        private void Initialize_FD_InstructionsTable()
        {
            FD_InstructionExecutors = new Dictionary<byte, Func<byte>>
            {
                { 0x09, ADD_IY_BC },
                { 0x19, ADD_IY_DE },
            };
        }

        //TODO: Move instructions to their own files and include them in the table when they are implemented.

        /// <summary>
        /// The ADD_IY,BC instruction.
        /// </summary>
        byte ADD_IY_BC()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The ADD IY,DE instruction.
        /// </summary>
        byte ADD_IY_DE()
        {
            throw new NotImplementedException();
        }
    }
}
