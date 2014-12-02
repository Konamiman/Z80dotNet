namespace Konamiman.Z80dotNet
{
    public partial class Z80InstructionExecutor
    {
        /// <summary>
        /// The LD A,I instruction.
        /// </summary>
        byte LD_A_I()
        {
            FetchFinished();

            var value = Registers.I;
            Registers.A = value;

            Registers.SF = value.GetBit(7);
            Registers.ZF = (value == 0);
            Registers.HF = 0;
            Registers.NF = 0;
            Registers.PF = Registers.IFF2;
            SetFlags3and5From(value);

            return 9;
        }
    }
}
