namespace Konamiman.Z80dotNet
{
    public partial class Z80InstructionExecutor
    {
        /// <summary>
        /// The RLA instruction.
        /// </summary>
        byte RLA()
        {
            FetchFinished();

            var oldValue = Registers.A;
            Registers.A = (byte)((oldValue << 1) | (byte)Registers.CF);
            Registers.CF = oldValue.GetBit(7);

            Registers.NF = 0;
            Registers.HF = 0;

            return 4;
        }
    }
}
