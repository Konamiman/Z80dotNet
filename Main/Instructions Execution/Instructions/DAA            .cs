namespace Konamiman.Z80dotNet
{
    public partial class Z80InstructionExecutor
    {
        /// <summary>
        /// The DAA instruction.
        /// </summary>
        byte DAA()
        {
            const byte CF_NF = 3;
            
            FetchFinished();

            //Algorithm borrowed from MAME:
            //https://github.com/mamedev/mame/blob/master/src/emu/cpu/z80/z80.c

            var temp = Registers.A;

            if(Registers.HF || (Registers.A & 0x0F) > 9) temp = (byte)temp.Add(Registers.NF ? 0xFA : 0x06);
            if(Registers.CF || Registers.A > 0x99) temp = (byte)temp.Add(Registers.NF ? 0xA0 : 0x60);

            Registers.F = (byte)((Registers.F & CF_NF) | (Registers.A > 0x99 ? 1 : 0) | ((Registers.A ^ temp) & 0x0F) | (temp & 0x80));
            Registers.A = temp;

            SetFlags3and5From(temp);
            Registers.PF = Parity[temp];

            return 4;
        }
    }
}
