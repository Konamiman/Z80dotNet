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
                { 0x21, LD_IX_nn },
                { 0x22, LD_aa_IX },
                { 0x23, INC_IX },
                { 0x24, INC_IXH },
                { 0x25, DEC_IXH },
                { 0x26, LD_IXH_n },
                { 0x29, ADD_IX_IX },
                { 0x2A, LD_IX_aa },
                { 0x2B, DEC_IX },
                { 0x2C, INC_IXL },
                { 0x2D, DEC_IXL },
                { 0x2E, LD_IXL_n },
                { 0x34, INC_aIX_plus_n },
                { 0x35, DEC_aIX_plus_n },
                { 0x36, LD_aIX_plus_n_N },
                { 0x39, ADD_IX_SP },
                { 0x44, LD_B_IXH },
                { 0x45, LD_B_IXL },
                { 0x46, LD_B_aIX_plus_n },
                { 0x4C, LD_C_IXH },
                { 0x4D, LD_C_IXL },
                { 0x4E, LD_C_aIX_plus_n },
                { 0x54, LD_D_IXH },
                { 0x55, LD_D_IXL },
                { 0x56, LD_D_aIX_plus_n },
                { 0x5C, LD_E_IXH },
                { 0x5D, LD_E_IXL },
                { 0x5E, LD_E_aIX_plus_n },
                { 0x60, LD_IXH_B },
                { 0x61, LD_IXH_C },
                { 0x62, LD_IXH_D },
                { 0x63, LD_IXH_E },
                { 0x64, LD_IXH_IXH },
                { 0x65, LD_IXH_IXL },
                { 0x66, LD_H_aIX_plus_n },
                { 0x67, LD_IXH_A },
                { 0x68, LD_IXL_B },
                { 0x69, LD_IXL_C },
                { 0x6A, LD_IXL_D },
                { 0x6B, LD_IXL_E },
                { 0x6C, LD_IXL_H },
                { 0x6D, LD_IXL_IXL },
                { 0x6E, LD_L_aIX_plus_n },
                { 0x6F, LD_IXL_A },
                { 0x70, LD_aIX_plus_n_B },
                { 0x71, LD_aIX_plus_n_C },
                { 0x72, LD_aIX_plus_n_D },
                { 0x73, LD_aIX_plus_n_E },
                { 0x74, LD_aIX_plus_n_H },
                { 0x75, LD_aIX_plus_n_L },
                { 0x77, LD_aIX_plus_n_A },
                { 0x7C, LD_A_IXH },
                { 0x7D, LD_A_IXL },
                { 0x7E, LD_A_aIX_plus_n },
                { 0x84, ADD_A_IXH },
                { 0x85, ADD_A_IXL },
                { 0x86, ADD_A_aIX_plus_n },
                { 0x8C, ADC_A_IXH },
                { 0x8D, ADC_A_IXL },
                { 0x8E, ADC_A_aIX_plus_n },
                { 0x94, SUB_IXH },
                { 0x95, SUB_IXL },
                { 0x96, SUB_aIX_plus_n },
                { 0x9C, SBC_A_IXH },
                { 0x9D, SBC_A_IXL },
                { 0x9E, SBC_A_aIX_plus_n },
                { 0xA4, AND_IXH },
                { 0xA5, AND_IXL },
                { 0xA6, AND_aIX_plus_n },
                { 0xAC, XOR_IXH },
                { 0xAD, XOR_IXL },
                { 0xAE, XOR_aIX_plus_n },
                { 0xB4, OR_IXH },
                { 0xB5, OR_IXL },
                { 0xB6, OR_aIX_plus_n },
                { 0xBC, CP_IXH },
                { 0xBD, CP_IXL },
                { 0xBE, CP_aIX_plus_n },
                { 0xE1, POP_IX },
                { 0xE3, EX_aSP_IX },
                { 0xE5, PUSH_IX },
                { 0xE9, JP_aIX },
                { 0xF9, LD_SP_IX },
            };
        }

        //TODO: Move instructions to their own files and include them in the table when they are implemented.

        /// <summary>
        /// The LD IX,nn instruction.
        /// </summary>
        byte LD_IX_nn()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The INC (IX+d) instruction.
        /// </summary>
        byte INC_aIX_plus_n()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The DEC (IX+d) instruction.
        /// </summary>
        byte DEC_aIX_plus_n()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD (IX+d),N instruction.
        /// </summary>
        byte LD_aIX_plus_n_N()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD B,IXH instruction.
        /// </summary>
        byte LD_B_IXH()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD B,IXL instruction.
        /// </summary>
        byte LD_B_IXL()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD B,(IX+d) instruction.
        /// </summary>
        byte LD_B_aIX_plus_n()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD C,IXH instruction.
        /// </summary>
        byte LD_C_IXH()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD C,IXL instruction.
        /// </summary>
        byte LD_C_IXL()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD C,(IX+d) instruction.
        /// </summary>
        byte LD_C_aIX_plus_n()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD D,IXH instruction.
        /// </summary>
        byte LD_D_IXH()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD D,IXL instruction.
        /// </summary>
        byte LD_D_IXL()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD D,(IX+d) instruction.
        /// </summary>
        byte LD_D_aIX_plus_n()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD E,IXH instruction.
        /// </summary>
        byte LD_E_IXH()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD E,IXL instruction.
        /// </summary>
        byte LD_E_IXL()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD E,(IX+d) instruction.
        /// </summary>
        byte LD_E_aIX_plus_n()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD IXH,B instruction.
        /// </summary>
        byte LD_IXH_B()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD IXH,C instruction.
        /// </summary>
        byte LD_IXH_C()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD IXH,D instruction.
        /// </summary>
        byte LD_IXH_D()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD IXH,E instruction.
        /// </summary>
        byte LD_IXH_E()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD IXH,IXH instruction.
        /// </summary>
        byte LD_IXH_IXH()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD IXH,IXL instruction.
        /// </summary>
        byte LD_IXH_IXL()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD H,(IX+d) instruction.
        /// </summary>
        byte LD_H_aIX_plus_n()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD IXH,A instruction.
        /// </summary>
        byte LD_IXH_A()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD IXL,B instruction.
        /// </summary>
        byte LD_IXL_B()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD IXL,C instruction.
        /// </summary>
        byte LD_IXL_C()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD IXL,D instruction.
        /// </summary>
        byte LD_IXL_D()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD IXL,E instruction.
        /// </summary>
        byte LD_IXL_E()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD IXL,H instruction.
        /// </summary>
        byte LD_IXL_H()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD IXL,IXL instruction.
        /// </summary>
        byte LD_IXL_IXL()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD L,(IX+d) instruction.
        /// </summary>
        byte LD_L_aIX_plus_n()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD IXL,A instruction.
        /// </summary>
        byte LD_IXL_A()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD (IX+d),B instruction.
        /// </summary>
        byte LD_aIX_plus_n_B()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD (IX+d),C instruction.
        /// </summary>
        byte LD_aIX_plus_n_C()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD (IX+d),D instruction.
        /// </summary>
        byte LD_aIX_plus_n_D()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD (IX+d),E instruction.
        /// </summary>
        byte LD_aIX_plus_n_E()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD (IX+d),H instruction.
        /// </summary>
        byte LD_aIX_plus_n_H()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD (IX+d),L instruction.
        /// </summary>
        byte LD_aIX_plus_n_L()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD (IX+d),A instruction.
        /// </summary>
        byte LD_aIX_plus_n_A()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD A,IXH instruction.
        /// </summary>
        byte LD_A_IXH()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD A,IXL instruction.
        /// </summary>
        byte LD_A_IXL()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD A,(IX+d) instruction.
        /// </summary>
        byte LD_A_aIX_plus_n()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The ADD A,IXH instruction.
        /// </summary>
        byte ADD_A_IXH()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The ADD A,IXL instruction.
        /// </summary>
        byte ADD_A_IXL()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The ADD A,(IX+d) instruction.
        /// </summary>
        byte ADD_A_aIX_plus_n()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The ADC A,IXH instruction.
        /// </summary>
        byte ADC_A_IXH()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The ADC A,IXL instruction.
        /// </summary>
        byte ADC_A_IXL()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The ADC A,(IX+d) instruction.
        /// </summary>
        byte ADC_A_aIX_plus_n()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SUB IXH instruction.
        /// </summary>
        byte SUB_IXH()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SUB IXL instruction.
        /// </summary>
        byte SUB_IXL()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SUB (IX+d) instruction.
        /// </summary>
        byte SUB_aIX_plus_n()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SBC A,IXH instruction.
        /// </summary>
        byte SBC_A_IXH()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SBC A,IXL instruction.
        /// </summary>
        byte SBC_A_IXL()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SBC A,(IX+d) instruction.
        /// </summary>
        byte SBC_A_aIX_plus_n()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The AND IXH instruction.
        /// </summary>
        byte AND_IXH()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The AND IXL instruction.
        /// </summary>
        byte AND_IXL()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The AND (IX+d) instruction.
        /// </summary>
        byte AND_aIX_plus_n()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The XOR IXH instruction.
        /// </summary>
        byte XOR_IXH()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The XOR IXL instruction.
        /// </summary>
        byte XOR_IXL()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The XOR (IX+d) instruction.
        /// </summary>
        byte XOR_aIX_plus_n()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The OR IXH instruction.
        /// </summary>
        byte OR_IXH()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The OR IXL instruction.
        /// </summary>
        byte OR_IXL()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The OR (IX+d) instruction.
        /// </summary>
        byte OR_aIX_plus_n()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The CP IXH instruction.
        /// </summary>
        byte CP_IXH()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The CP IXL instruction.
        /// </summary>
        byte CP_IXL()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The CP (IX+d) instruction.
        /// </summary>
        byte CP_aIX_plus_n()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The POP IX instruction.
        /// </summary>
        byte POP_IX()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The EX (SP),IX instruction.
        /// </summary>
        byte EX_aSP_IX()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The PUSH IX instruction.
        /// </summary>
        byte PUSH_IX()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The JP (IX) instruction.
        /// </summary>
        byte JP_aIX()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The LD SP,IX instruction.
        /// </summary>
        byte LD_SP_IX()
        {
            throw new NotImplementedException();
        }
    }
}
