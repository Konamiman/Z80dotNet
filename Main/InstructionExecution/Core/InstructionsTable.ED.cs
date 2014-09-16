using System;

namespace Konamiman.Z80dotNet
{
    public partial class Z80InstructionExecutor
    {
        private Func<byte>[] ED_InstructionExecutors;
        private Func<byte>[] ED_Block_InstructionExecutors;

        private void Initialize_ED_InstructionsTable()
        {
            ED_InstructionExecutors = new Func<byte>[]
            {
                IN_B_C,    //40
                OUT_C_B,    //41
                SBC_HL_BC,    //42
                LD_aa_BC,    //43
                NEG,    //44
                RETN,    //45
                IM_0,    //46
                LD_I_A,    //47
                IN_C_C,    //48
                OUT_C_C,    //49
                ADC_HL_BC,    //4A
                LD_BC_aa,    //4B
                NEG,    //4C
                RETI,    //4D
                IM_0,    //4E
                LD_R_A,    //4F
                IN_D_C,    //50
                OUT_C_D,    //51
                SBC_HL_DE,    //52
                LD_aa_DE,    //53
                NEG,    //54
                RETN,    //55
                IM_1,    //56
                LD_A_I,    //57
                IN_E_C,    //58
                OUT_C_E,    //59
                ADC_HL_DE,    //5A
                LD_DE_aa,    //5B
                NEG,    //5C
                RETI,    //5D
                IM_2,    //5E
                LD_A_R,    //5F
                IN_H_C,    //60
                OUT_C_H,    //61
                SBC_HL_HL,    //62
                LD_aa_HL,    //63
                NEG,    //64
                RETN,    //65
                IM_0,    //66
                RRD,    //67
                IN_L_C,    //68
                OUT_C_L,    //69
                ADC_HL_HL,    //6A
                LD_HL_aa,    //6B
                NEG,    //6C
                RETI,    //6D
                IM_0,    //6E
                RLD,    //6F
                IN_C,    //70
                OUT_C_0,    //71
                SBC_HL_SP,    //72
                LD_aa_SP,    //73
                NEG,    //74
                RETN,    //75
                IM_1,    //76
                NOP,    //77
                IN_A_C,    //78
                OUT_C_A,    //79
                ADC_HL_SP,    //7A
                LD_SP_aa,    //7B
                NEG,    //7C
                RETI,    //7D
                IM_2,    //7E
                NOP,    //7F
            };

            ED_Block_InstructionExecutors = new Func<byte>[]
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
            FetchFinished();
            return 12;
        }

        /// <summary>
        /// The OUT (C),B instruction.
        /// </summary>
        byte OUT_C_B()
        {
            FetchFinished();
            return 12;
        }

        /// <summary>
        /// The SBC HL,BC instruction.
        /// </summary>
        byte SBC_HL_BC()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD (nn),BC instruction.
        /// </summary>
        byte LD_aa_BC()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD I,A instruction.
        /// </summary>
        byte LD_I_A()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The IN C,(C) instruction.
        /// </summary>
        byte IN_C_C()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The OUT (C),C instruction.
        /// </summary>
        byte OUT_C_C()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The ADC HL,BC instruction.
        /// </summary>
        byte ADC_HL_BC()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD BC,(nn) instruction.
        /// </summary>
        byte LD_BC_aa()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD R,A instruction.
        /// </summary>
        byte LD_R_A()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The IN D,(C) instruction.
        /// </summary>
        byte IN_D_C()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The OUT (C),D instruction.
        /// </summary>
        byte OUT_C_D()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SBC HL,DE instruction.
        /// </summary>
        byte SBC_HL_DE()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD (nn),DE instruction.
        /// </summary>
        byte LD_aa_DE()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD A,I instruction.
        /// </summary>
        byte LD_A_I()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The IN E,(C) instruction.
        /// </summary>
        byte IN_E_C()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The OUT (C),E instruction.
        /// </summary>
        byte OUT_C_E()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The ADC HL,DE instruction.
        /// </summary>
        byte ADC_HL_DE()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD DE,(nn) instruction.
        /// </summary>
        byte LD_DE_aa()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD A,R instruction.
        /// </summary>
        byte LD_A_R()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The IN H,(C) instruction.
        /// </summary>
        byte IN_H_C()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The OUT (C),H instruction.
        /// </summary>
        byte OUT_C_H()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SBC HL,HL instruction.
        /// </summary>
        byte SBC_HL_HL()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RRD instruction.
        /// </summary>
        byte RRD()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The IN L,(C) instruction.
        /// </summary>
        byte IN_L_C()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The OUT (C),L instruction.
        /// </summary>
        byte OUT_C_L()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The ADC HL,HL instruction.
        /// </summary>
        byte ADC_HL_HL()
        {
            throw new NotImplementedException();
        }

        /// The RLD instruction.
        /// </summary>
        byte RLD()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The IN (C) instruction.
        /// </summary>
        byte IN_C()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The OUT (C),0 instruction.
        /// </summary>
        byte OUT_C_0()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SBC HL,SP instruction.
        /// </summary>
        byte SBC_HL_SP()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD (nn),SP instruction.
        /// </summary>
        byte LD_aa_SP()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The IN A,(C) instruction.
        /// </summary>
        byte IN_A_C()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The OUT (C),A instruction.
        /// </summary>
        byte OUT_C_A()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The ADC HL,SP instruction.
        /// </summary>
        byte ADC_HL_SP()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD SP,(nn) instruction.
        /// </summary>
        byte LD_SP_aa()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The NEG instruction.
        /// </summary>
        byte NEG()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RETI instruction.
        /// </summary>
        byte RETI()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The IM 2 instruction.
        /// </summary>
        byte IM_2()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RETN instruction.
        /// </summary>
        byte RETN()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The IM 0 instruction.
        /// </summary>
        byte IM_0()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The IM 1 instruction.
        /// </summary>
        byte IM_1()
        {
            throw new NotImplementedException();
        }
    }
}
