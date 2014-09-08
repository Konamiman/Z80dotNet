using System;

namespace Konamiman.Z80dotNet
{
    /// <summary>
    /// Event args for the <see cref="IZ80InstructionExecutor.InstructionFetchFinished"/> event.
    /// </summary>
    public class InstructionFetchFinishedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="opcode">Value for the <see cref="InstructionFetchFinishedEventArgs.Opcode"/> property.</param>
        public InstructionFetchFinishedEventArgs(params byte[] opcode)
        {
            this.Opcode = opcode;
        }

        /// <summary>
        /// The opcode bytes of the instruction fetched.
        /// </summary>
        public byte[] Opcode { get; private set; }
    }
}