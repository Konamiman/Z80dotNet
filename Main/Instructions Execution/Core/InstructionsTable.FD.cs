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
                { 0x21, LD_IY_nn },
                { 0x22, LD_aa_IY },
                { 0x23, INC_IY },
                { 0x24, INC_IYH },
                { 0x25, DEC_IYH },
                { 0x26, LD_IYH_n },
                { 0x29, ADD_IY_IY },
                { 0x2A, LD_IY_aa },
                { 0x2B, DEC_IY },
                { 0x2C, INC_IYL },
                { 0x2D, DEC_IYL },
                { 0x2E, LD_IYL_n },
                { 0x34, INC_aIY_plus_n },
                { 0x35, DEC_aIY_plus_n },
                { 0x36, LD_aIY_plus_n_N },
                { 0x39, ADD_IY_SP },
                { 0x44, LD_B_IYH },
                { 0x45, LD_B_IYL },
                { 0x46, LD_B_aIY_plus_n },
                { 0x4C, LD_C_IYH },
                { 0x4D, LD_C_IYL },
                { 0x4E, LD_C_aIY_plus_n },
                { 0x54, LD_D_IYH },
                { 0x55, LD_D_IYL },
                { 0x56, LD_D_aIY_plus_n },
                { 0x5C, LD_E_IYH },
                { 0x5D, LD_E_IYL },
                { 0x5E, LD_E_aIY_plus_n },
                { 0x60, LD_IYH_B },
                { 0x61, LD_IYH_C },
                { 0x62, LD_IYH_D },
                { 0x63, LD_IYH_E },
                { 0x64, LD_IYH_IYH },
                { 0x65, LD_IYH_IYL },
                { 0x66, LD_H_aIY_plus_n },
                { 0x67, LD_IYH_A },
                { 0x68, LD_IYL_B },
                { 0x69, LD_IYL_C },
                { 0x6A, LD_IYL_D },
                { 0x6B, LD_IYL_E },
                { 0x6C, LD_IYL_H },
                { 0x6D, LD_IYL_IYL },
                { 0x6E, LD_L_aIY_plus_n },
                { 0x6F, LD_IYL_A },
                { 0x70, LD_aIY_plus_n_B },
                { 0x71, LD_aIY_plus_n_C },
                { 0x72, LD_aIY_plus_n_D },
                { 0x73, LD_aIY_plus_n_E },
                { 0x74, LD_aIY_plus_n_H },
                { 0x75, LD_aIY_plus_n_L },
                { 0x77, LD_aIY_plus_n_A },
                { 0x7C, LD_A_IYH },
                { 0x7D, LD_A_IYL },
                { 0x7E, LD_A_aIY_plus_n },
                { 0x84, ADD_A_IYH },
                { 0x85, ADD_A_IYL },
                { 0x86, ADD_A_aIY_plus_n },
                { 0x8C, ADC_A_IYH },
                { 0x8D, ADC_A_IYL },
                { 0x8E, ADC_A_aIY_plus_n },
                { 0x94, SUB_IYH },
                { 0x95, SUB_IYL },
                { 0x96, SUB_aIY_plus_n },
                { 0x9C, SBC_A_IYH },
                { 0x9D, SBC_A_IYL },
                { 0x9E, SBC_A_aIY_plus_n },
                { 0xA4, AND_IYH },
                { 0xA5, AND_IYL },
                { 0xA6, AND_aIY_plus_n },
                { 0xAC, XOR_IYH },
                { 0xAD, XOR_IYL },
                { 0xAE, XOR_aIY_plus_n },
                { 0xB4, OR_IYH },
                { 0xB5, OR_IYL },
                { 0xB6, OR_aIY_plus_n },
                { 0xBC, CP_IYH },
                { 0xBD, CP_IYL },
                { 0xBE, CP_aIY_plus_n },
                { 0xE1, POP_IY },
                { 0xE3, EX_aSP_IY },
                { 0xE5, PUSH_IY },
                { 0xE9, JP_aIY },
                { 0xF9, LD_SP_IY },
            };
        }

        //TODO: Move instructions to their own files when they are implemented.

        /// <summary>
        /// The LD IY,nn instruction.
        /// </summary>
        byte LD_IY_nn()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The INC (IY+d) instruction.
        /// </summary>
        byte INC_aIY_plus_n()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The DEC (IY+d) instruction.
        /// </summary>
        byte DEC_aIY_plus_n()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD (IY+d),N instruction.
        /// </summary>
        byte LD_aIY_plus_n_N()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD B,(IY+d) instruction.
        /// </summary>
        byte LD_B_aIY_plus_n()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD C,(IY+d) instruction.
        /// </summary>
        byte LD_C_aIY_plus_n()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD D,(IY+d) instruction.
        /// </summary>
        byte LD_D_aIY_plus_n()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD E,(IY+d) instruction.
        /// </summary>
        byte LD_E_aIY_plus_n()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD H,(IY+d) instruction.
        /// </summary>
        byte LD_H_aIY_plus_n()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD L,(IY+d) instruction.
        /// </summary>
        byte LD_L_aIY_plus_n()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD (IY+d),B instruction.
        /// </summary>
        byte LD_aIY_plus_n_B()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD (IY+d),C instruction.
        /// </summary>
        byte LD_aIY_plus_n_C()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD (IY+d),D instruction.
        /// </summary>
        byte LD_aIY_plus_n_D()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD (IY+d),E instruction.
        /// </summary>
        byte LD_aIY_plus_n_E()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD (IY+d),H instruction.
        /// </summary>
        byte LD_aIY_plus_n_H()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD (IY+d),L instruction.
        /// </summary>
        byte LD_aIY_plus_n_L()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD (IY+d),A instruction.
        /// </summary>
        byte LD_aIY_plus_n_A()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD A,(IY+d) instruction.
        /// </summary>
        byte LD_A_aIY_plus_n()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The ADD A,IYH instruction.
        /// </summary>
        byte ADD_A_IYH()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The ADD A,IYL instruction.
        /// </summary>
        byte ADD_A_IYL()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The ADD A,(IY+d) instruction.
        /// </summary>
        byte ADD_A_aIY_plus_n()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The ADC A,IYH instruction.
        /// </summary>
        byte ADC_A_IYH()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The ADC A,IYL instruction.
        /// </summary>
        byte ADC_A_IYL()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The ADC A,(IY+d) instruction.
        /// </summary>
        byte ADC_A_aIY_plus_n()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SUB IYH instruction.
        /// </summary>
        byte SUB_IYH()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SUB IYL instruction.
        /// </summary>
        byte SUB_IYL()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SUB (IY+d) instruction.
        /// </summary>
        byte SUB_aIY_plus_n()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SBC A,IYH instruction.
        /// </summary>
        byte SBC_A_IYH()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SBC A,IYL instruction.
        /// </summary>
        byte SBC_A_IYL()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SBC A,(IY+d) instruction.
        /// </summary>
        byte SBC_A_aIY_plus_n()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The AND IYH instruction.
        /// </summary>
        byte AND_IYH()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The AND IYL instruction.
        /// </summary>
        byte AND_IYL()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The AND (IY+d) instruction.
        /// </summary>
        byte AND_aIY_plus_n()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The XOR IYH instruction.
        /// </summary>
        byte XOR_IYH()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The XOR IYL instruction.
        /// </summary>
        byte XOR_IYL()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The XOR (IY+d) instruction.
        /// </summary>
        byte XOR_aIY_plus_n()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The OR IYH instruction.
        /// </summary>
        byte OR_IYH()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The OR IYL instruction.
        /// </summary>
        byte OR_IYL()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The OR (IY+d) instruction.
        /// </summary>
        byte OR_aIY_plus_n()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The CP IYH instruction.
        /// </summary>
        byte CP_IYH()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The CP IYL instruction.
        /// </summary>
        byte CP_IYL()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The CP (IY+d) instruction.
        /// </summary>
        byte CP_aIY_plus_n()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The POP IY instruction.
        /// </summary>
        byte POP_IY()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The EX (SP),IY instruction.
        /// </summary>
        byte EX_aSP_IY()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The PUSH IY instruction.
        /// </summary>
        byte PUSH_IY()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The JP (IY) instruction.
        /// </summary>
        byte JP_aIY()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD SP,IY instruction.
        /// </summary>
        byte LD_SP_IY()
        {
            throw new NotImplementedException();
        }
    }
}
