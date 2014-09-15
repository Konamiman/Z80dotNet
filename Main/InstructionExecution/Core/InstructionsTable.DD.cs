using System;
using System.Collections.Generic;

namespace Konamiman.Z80dotNet
{
    public partial class Z80InstructionExecutor
    {
        private IDictionary<byte, Func<byte>> DD_InstructionExecutors;

        private void Initialize_DD_InstructionsTable()
        {
            DD_InstructionExecutors = new Dictionary<byte, Func<byte>>
            {
                { 0x09, ADD_IX_BC },
                { 0x19, ADD_IX_DE },
            };
        }

        //TODO: Move instructions to their own files and include them in the table when they are implemented.

        /// <summary>
        /// The ADD_IX,BC instruction.
        /// </summary>
        byte ADD_IX_BC()
        {
            FetchFinished();
            return 15;
        }

        /// <summary>
        /// The ADD IX,DE instruction.
        /// </summary>
        byte ADD_IX_DE()
        {
            FetchFinished();
            return 15;
        }
    }
}
