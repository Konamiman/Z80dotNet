using System;

namespace Konamiman.Z80dotNet
{
    public partial class Z80InstructionExecutor
    {
        private Func<byte>[] SingleByte_InstructionExecutors;

        private void Initialize_SingleByte_InstructionsTable()
        {
            SingleByte_InstructionExecutors = new Func<byte>[]
            {
                NOP,        //00
                LD_BC_nn,   //01
                LD_BC_A,    //02
                INC_BC,     //03
                INC_B,      //04
                DEC_B,      //05
                LD_B_n,     //06
                RLCA,       //07
                EX_AF_AF,   //08
                ADD_HL_BC,  //09
                LD_A_BC,    //0A
                DEC_BC,     //0B
                INC_C,      //0C
                DEC_C,      //0D
                LD_C_n,     //0E
                RRCA,       //0F
                DJNZ,       //10
                LD_DE_nn,   //11
                LD_DE_A,    //12
                INC_DE,     //13
                INC_D,      //14
                DEC_D,      //15
                LD_D_n,     //16
                RLA,        //17
                JR,         //18
                ADD_HL_DE,  //19
                LD_A_DE,    //1A
                DEC_DE,     //1B
                INC_E,      //1C
                DEC_E,      //1D
                LD_E_n,     //1E
                RRCA,       //1F
                JR_NZ,      //20
                LD_HL_nn,   //21
                LD_aa_HL,   //22
                INC_HL,     //23
                INC_H,      //24
                DEC_H,      //25
                LD_H_n,     //26
                DAA,        //27
                JR_Z,       //28
                ADD_HL_HL,  //29
                LD_HL_aa,   //2A
                DEC_HL,     //2B
                INC_L,      //2C
                DEC_L,      //2D
                LD_L_n,     //2E
                CPL,        //2F
            };
        }

        //TODO: Move instructions to their own files and include them in the table when they are implemented.

        byte LD_BC_A()
        {
            throw new NotImplementedException();
        }

        byte INC_BC()
        {
            throw new NotImplementedException();
        }

        byte INC_B()
        {
            throw new NotImplementedException();
        }

        byte DEC_B()
        {
            throw new NotImplementedException();
        }

        byte LD_B_n()
        {
            throw new NotImplementedException();
        }

        byte RLCA()
        {
            throw new NotImplementedException();
        }

        byte EX_AF_AF()
        {
            throw new NotImplementedException();
        }

        byte ADD_HL_BC()
        {
            throw new NotImplementedException();
        }

        byte LD_A_BC()
        {
            throw new NotImplementedException();
        }

        byte DEC_BC()
        {
            throw new NotImplementedException();
        }

        byte INC_C()
        {
            throw new NotImplementedException();
        }

        byte DEC_C()
        {
            throw new NotImplementedException();
        }

        byte LD_C_n()
        {
            throw new NotImplementedException();
        }

        byte RRCA()
        {
            throw new NotImplementedException();
        }

        byte DJNZ()
        {
            throw new NotImplementedException();
        }

        byte LD_DE_A()
        {
            throw new NotImplementedException();
        }

        byte INC_DE()
        {
            throw new NotImplementedException();
        }

        byte INC_D()
        {
            throw new NotImplementedException();
        }

        byte DEC_D()
        {
            throw new NotImplementedException();
        }

        byte LD_D_n()
        {
            throw new NotImplementedException();
        }

        byte RLA()
        {
            throw new NotImplementedException();
        }

        byte JR()
        {
            throw new NotImplementedException();
        }

        byte ADD_HL_DE()
        {
            throw new NotImplementedException();
        }

        byte LD_A_DE()
        {
            throw new NotImplementedException();
        }

        byte DEC_DE()
        {
            throw new NotImplementedException();
        }

        byte INC_E()
        {
            throw new NotImplementedException();
        }

        byte DEC_E()
        {
            throw new NotImplementedException();
        }

        byte LD_E_n()
        {
            throw new NotImplementedException();
        }

        byte RRA()
        {
            throw new NotImplementedException();
        }

        byte JR_NZ()
        {
            throw new NotImplementedException();
        }

        byte LD_aa_HL()
        {
            throw new NotImplementedException();
        }

        byte INC_HL()
        {
            throw new NotImplementedException();
        }

        byte INC_H()
        {
            throw new NotImplementedException();
        }

        byte DEC_H()
        {
            throw new NotImplementedException();
        }

        byte LD_H_n()
        {
            throw new NotImplementedException();
        }

        byte DAA()
        {
            throw new NotImplementedException();
        }

        byte JR_Z()
        {
            throw new NotImplementedException();
        }

        byte ADD_HL_HL()
        {
            throw new NotImplementedException();
        }

        byte LD_HL_aa()
        {
            throw new NotImplementedException();
        }

        byte DEC_HL()
        {
            throw new NotImplementedException();
        }

        byte INC_L()
        {
            throw new NotImplementedException();
        }

        byte DEC_L()
        {
            throw new NotImplementedException();
        }

        byte LD_L_n()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The X instruction.
        /// </summary>
        /// <returns></returns>
        byte CPL()
        {
            throw new NotImplementedException();
        }




    }
}
