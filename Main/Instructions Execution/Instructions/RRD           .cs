namespace Konamiman.Z80dotNet
{
    public partial class Z80InstructionExecutor
    {
        /// <summary>
        /// The RRD instruction.
        /// </summary>
        byte RRD()
        {
            FetchFinished();

            var memoryAddress = Registers.HL.ToUShort();

            var Avalue = Registers.A;
            var HLcontents = ProcessorAgent.ReadFromMemory(memoryAddress);

            var newAvalue = (byte)((Avalue & 0xF0) | (HLcontents & 0x0F));
            var newHLcontents = (byte)(((HLcontents >> 4) & 0x0F) | ((Avalue << 4) & 0xF0));

            Registers.A = newAvalue;
            ProcessorAgent.WriteToMemory(memoryAddress, newHLcontents);

            Registers.SF = newAvalue.GetBit(7);
            Registers.ZF = (newAvalue == 0);
            Registers.HF = 0;
            Registers.PF = Parity[newAvalue];
            Registers.NF = 1;
            SetFlags3and5From(newAvalue);

            return 18;
        }
    }
}
