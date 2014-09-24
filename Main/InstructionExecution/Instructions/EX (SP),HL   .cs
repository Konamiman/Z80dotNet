namespace Konamiman.Z80dotNet
{
    public partial class Z80InstructionExecutor
    {
        /// <summary>
        /// The EX (SP),HL instruction.
        /// </summary>
        byte EX_aSP_HL()
        {
            FetchFinished();

            var sp = Registers.SP.ToUShort();
            
            var temp = ReadShortFromMemory(sp);
            WriteShortToMemory(sp, Registers.HL);
            Registers.HL = temp;

            return 19;
        }
    }
}
