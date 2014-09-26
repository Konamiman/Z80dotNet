using System;

namespace Konamiman.Z80dotNet
{
    public partial class Z80InstructionExecutor
    {
        private Func<byte>[] CB_InstructionExecutors;

        private void Initialize_CB_InstructionsTable()
        {
            CB_InstructionExecutors = new Func<byte>[]
            {
                RLC_B,    //00
                RLC_C,    //01
                RLC_D,    //02
                RLC_E,    //03
                RLC_H,    //04
                RLC_L,    //05
                RLC_aHL,    //06
                RLC_A,    //07
                RRC_B,    //08
                RRC_C,    //09
                RRC_D,    //0A
                RRC_E,    //0B
                RRC_H,    //0C
                RRC_L,    //0D
                RRC_aHL,    //0E
                RRC_A,    //0F
                RL_B,    //10
                RL_C,    //11
                RL_D,    //12
                RL_E,    //13
                RL_H,    //14
                RL_L,    //15
                RL_aHL,    //16
                RL_A,    //17
                RR_B,    //18
                RR_C,    //19
                RR_D,    //1A
                RR_E,    //1B
                RR_H,    //1C
                RR_L,    //1D
                RR_aHL,    //1E
                RR_A,    //1F
                SLA_B,    //20
                SLA_C,    //21
                SLA_D,    //22
                SLA_E,    //23
                SLA_H,    //24
                SLA_L,    //25
                SLA_aHL,    //26
                SLA_A,    //27
                SRA_B,    //28
                SRA_C,    //29
                SRA_D,    //2A
                SRA_E,    //2B
                SRA_H,    //2C
                SRA_L,    //2D
                SRA_aHL,    //2E
                SRA_A,    //2F
                SLL_B,    //30
                SLL_C,    //31
                SLL_D,    //32
                SLL_E,    //33
                SLL_H,    //34
                SLL_L,    //35
                SLL_aHL,    //36
                SLL_A,    //37
                SRL_B,    //38
                SRL_C,    //39
                SRL_D,    //3A
                SRL_E,    //3B
                SRL_H,    //3C
                SRL_L,    //3D
                SRL_aHL,    //3E
                SRL_A,    //3F
                BIT_0_B,    //40
                BIT_0_C,    //41
                BIT_0_D,    //42
                BIT_0_E,    //43
                BIT_0_H,    //44
                BIT_0_L,    //45
                BIT_0_aHL,    //46
                BIT_0_A,    //47
                BIT_1_B,    //48
                BIT_1_C,    //49
                BIT_1_D,    //4A
                BIT_1_E,    //4B
                BIT_1_H,    //4C
                BIT_1_L,    //4D
                BIT_1_aHL,    //4E
                BIT_1_A,    //4F
                BIT_2_B,    //50
                BIT_2_C,    //51
                BIT_2_D,    //52
                BIT_2_E,    //53
                BIT_2_H,    //54
                BIT_2_L,    //55
                BIT_2_aHL,    //56
                BIT_2_A,    //57
                BIT_3_B,    //58
                BIT_3_C,    //59
                BIT_3_D,    //5A
                BIT_3_E,    //5B
                BIT_3_H,    //5C
                BIT_3_L,    //5D
                BIT_3_aHL,    //5E
                BIT_3_A,    //5F
                BIT_4_B,    //60
                BIT_4_C,    //61
                BIT_4_D,    //62
                BIT_4_E,    //63
                BIT_4_H,    //64
                BIT_4_L,    //65
                BIT_4_aHL,    //66
                BIT_4_A,    //67
                BIT_5_B,    //68
                BIT_5_C,    //69
                BIT_5_D,    //6A
                BIT_5_E,    //6B
                BIT_5_H,    //6C
                BIT_5_L,    //6D
                BIT_5_aHL,    //6E
                BIT_5_A,    //6F
                BIT_6_B,    //70
                BIT_6_C,    //71
                BIT_6_D,    //72
                BIT_6_E,    //73
                BIT_6_H,    //74
                BIT_6_L,    //75
                BIT_6_aHL,    //76
                BIT_6_A,    //77
                BIT_7_B,    //78
                BIT_7_C,    //79
                BIT_7_D,    //7A
                BIT_7_E,    //7B
                BIT_7_H,    //7C
                BIT_7_L,    //7D
                BIT_7_aHL,    //7E
                BIT_7_A,    //7F
                RES_0_B,    //80
                RES_0_C,    //81
                RES_0_D,    //82
                RES_0_E,    //83
                RES_0_H,    //84
                RES_0_L,    //85
                RES_0_aHL,    //86
                RES_0_A,    //87
                RES_1_B,    //88
                RES_1_C,    //89
                RES_1_D,    //8A
                RES_1_E,    //8B
                RES_1_H,    //8C
                RES_1_L,    //8D
                RES_1_aHL,    //8E
                RES_1_A,    //8F
                RES_2_B,    //90
                RES_2_C,    //91
                RES_2_D,    //92
                RES_2_E,    //93
                RES_2_H,    //94
                RES_2_L,    //95
                RES_2_aHL,    //96
                RES_2_A,    //97
                RES_3_B,    //98
                RES_3_C,    //99
                RES_3_D,    //9A
                RES_3_E,    //9B
                RES_3_H,    //9C
                RES_3_L,    //9D
                RES_3_aHL,    //9E
                RES_3_A,    //9F
                RES_4_B,    //A0
                RES_4_C,    //A1
                RES_4_D,    //A2
                RES_4_E,    //A3
                RES_4_H,    //A4
                RES_4_L,    //A5
                RES_4_aHL,    //A6
                RES_4_A,    //A7
                RES_5_B,    //A8
                RES_5_C,    //A9
                RES_5_D,    //AA
                RES_5_E,    //AB
                RES_5_H,    //AC
                RES_5_L,    //AD
                RES_5_aHL,    //AE
                RES_5_A,    //AF
                RES_6_B,    //B0
                RES_6_C,    //B1
                RES_6_D,    //B2
                RES_6_E,    //B3
                RES_6_H,    //B4
                RES_6_L,    //B5
                RES_6_aHL,    //B6
                RES_6_A,    //B7
                RES_7_B,    //B8
                RES_7_C,    //B9
                RES_7_D,    //BA
                RES_7_E,    //BB
                RES_7_H,    //BC
                RES_7_L,    //BD
                RES_7_aHL,    //BE
                RES_7_A,    //BF
                SET_0_B,    //C0
                SET_0_C,    //C1
                SET_0_D,    //C2
                SET_0_E,    //C3
                SET_0_H,    //C4
                SET_0_L,    //C5
                SET_0_aHL,    //C6
                SET_0_A,    //C7
                SET_1_B,    //C8
                SET_1_C,    //C9
                SET_1_D,    //CA
                SET_1_E,    //CB
                SET_1_H,    //CC
                SET_1_L,    //CD
                SET_1_aHL,    //CE
                SET_1_A,    //CF
                SET_2_B,    //D0
                SET_2_C,    //D1
                SET_2_D,    //D2
                SET_2_E,    //D3
                SET_2_H,    //D4
                SET_2_L,    //D5
                SET_2_aHL,    //D6
                SET_2_A,    //D7
                SET_3_B,    //D8
                SET_3_C,    //D9
                SET_3_D,    //DA
                SET_3_E,    //DB
                SET_3_H,    //DC
                SET_3_L,    //DD
                SET_3_aHL,    //DE
                SET_3_A,    //DF
                SET_4_B,    //E0
                SET_4_C,    //E1
                SET_4_D,    //E2
                SET_4_E,    //E3
                SET_4_H,    //E4
                SET_4_L,    //E5
                SET_4_aHL,    //E6
                SET_4_A,    //E7
                SET_5_B,    //E8
                SET_5_C,    //E9
                SET_5_D,    //EA
                SET_5_E,    //EB
                SET_5_H,    //EC
                SET_5_L,    //ED
                SET_5_aHL,    //EE
                SET_5_A,    //EF
                SET_6_B,    //F0
                SET_6_C,    //F1
                SET_6_D,    //F2
                SET_6_E,    //F3
                SET_6_H,    //F4
                SET_6_L,    //F5
                SET_6_aHL,    //F6
                SET_6_A,    //F7
                SET_7_B,    //F8
                SET_7_C,    //F9
                SET_7_D,    //FA
                SET_7_E,    //FB
                SET_7_H,    //FC
                SET_7_L,    //FD
                SET_7_aHL,    //FE
                SET_7_A    //FF
            };
        }

        //TODO: Move instructions to their own files when they are implemented.

        /// <summary>
        /// The RLC B instruction.
        /// </summary>
        byte RLC_B()
        {
            FetchFinished();
            return 8;
        }

        /// <summary>
        /// The RLC C instruction.
        /// </summary>
        byte RLC_C()
        {
            FetchFinished();
            return 8;
        }

        /// <summary>
        /// The RLC D instruction.
        /// </summary>
        byte RLC_D()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RLC E instruction.
        /// </summary>
        byte RLC_E()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RLC H instruction.
        /// </summary>
        byte RLC_H()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RLC L instruction.
        /// </summary>
        byte RLC_L()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RLC (HL) instruction.
        /// </summary>
        byte RLC_aHL()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RLC A instruction.
        /// </summary>
        byte RLC_A()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RRC B instruction.
        /// </summary>
        byte RRC_B()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RRC C instruction.
        /// </summary>
        byte RRC_C()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RRC D instruction.
        /// </summary>
        byte RRC_D()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RRC E instruction.
        /// </summary>
        byte RRC_E()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RRC H instruction.
        /// </summary>
        byte RRC_H()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RRC L instruction.
        /// </summary>
        byte RRC_L()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RRC (HL) instruction.
        /// </summary>
        byte RRC_aHL()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RRC A instruction.
        /// </summary>
        byte RRC_A()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RL B instruction.
        /// </summary>
        byte RL_B()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RL C instruction.
        /// </summary>
        byte RL_C()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RL D instruction.
        /// </summary>
        byte RL_D()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RL E instruction.
        /// </summary>
        byte RL_E()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RL H instruction.
        /// </summary>
        byte RL_H()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RL L instruction.
        /// </summary>
        byte RL_L()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RL (HL) instruction.
        /// </summary>
        byte RL_aHL()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RL A instruction.
        /// </summary>
        byte RL_A()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RR B instruction.
        /// </summary>
        byte RR_B()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RR C instruction.
        /// </summary>
        byte RR_C()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RR D instruction.
        /// </summary>
        byte RR_D()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RR E instruction.
        /// </summary>
        byte RR_E()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RR H instruction.
        /// </summary>
        byte RR_H()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RR L instruction.
        /// </summary>
        byte RR_L()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RR (HL) instruction.
        /// </summary>
        byte RR_aHL()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RR A instruction.
        /// </summary>
        byte RR_A()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SLA B instruction.
        /// </summary>
        byte SLA_B()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SLA C instruction.
        /// </summary>
        byte SLA_C()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SLA D instruction.
        /// </summary>
        byte SLA_D()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SLA E instruction.
        /// </summary>
        byte SLA_E()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SLA H instruction.
        /// </summary>
        byte SLA_H()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SLA L instruction.
        /// </summary>
        byte SLA_L()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SLA (HL) instruction.
        /// </summary>
        byte SLA_aHL()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SLA A instruction.
        /// </summary>
        byte SLA_A()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SRA B instruction.
        /// </summary>
        byte SRA_B()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SRA C instruction.
        /// </summary>
        byte SRA_C()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SRA D instruction.
        /// </summary>
        byte SRA_D()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SRA E instruction.
        /// </summary>
        byte SRA_E()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SRA H instruction.
        /// </summary>
        byte SRA_H()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SRA L instruction.
        /// </summary>
        byte SRA_L()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SRA (HL) instruction.
        /// </summary>
        byte SRA_aHL()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SRA A instruction.
        /// </summary>
        byte SRA_A()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SLL B instruction.
        /// </summary>
        byte SLL_B()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SLL C instruction.
        /// </summary>
        byte SLL_C()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SLL D instruction.
        /// </summary>
        byte SLL_D()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SLL E instruction.
        /// </summary>
        byte SLL_E()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SLL H instruction.
        /// </summary>
        byte SLL_H()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SLL L instruction.
        /// </summary>
        byte SLL_L()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SLL (HL) instruction.
        /// </summary>
        byte SLL_aHL()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SLL A instruction.
        /// </summary>
        byte SLL_A()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SRL B instruction.
        /// </summary>
        byte SRL_B()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SRL C instruction.
        /// </summary>
        byte SRL_C()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SRL D instruction.
        /// </summary>
        byte SRL_D()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SRL E instruction.
        /// </summary>
        byte SRL_E()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SRL H instruction.
        /// </summary>
        byte SRL_H()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SRL L instruction.
        /// </summary>
        byte SRL_L()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SRL (HL) instruction.
        /// </summary>
        byte SRL_aHL()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SRL A instruction.
        /// </summary>
        byte SRL_A()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 0,B instruction.
        /// </summary>
        byte BIT_0_B()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 0,C instruction.
        /// </summary>
        byte BIT_0_C()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 0,D instruction.
        /// </summary>
        byte BIT_0_D()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 0,E instruction.
        /// </summary>
        byte BIT_0_E()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 0,H instruction.
        /// </summary>
        byte BIT_0_H()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 0,L instruction.
        /// </summary>
        byte BIT_0_L()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 0,(HL) instruction.
        /// </summary>
        byte BIT_0_aHL()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 0,A instruction.
        /// </summary>
        byte BIT_0_A()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 1,B instruction.
        /// </summary>
        byte BIT_1_B()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 1,C instruction.
        /// </summary>
        byte BIT_1_C()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 1,D instruction.
        /// </summary>
        byte BIT_1_D()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 1,E instruction.
        /// </summary>
        byte BIT_1_E()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 1,H instruction.
        /// </summary>
        byte BIT_1_H()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 1,L instruction.
        /// </summary>
        byte BIT_1_L()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 1,(HL) instruction.
        /// </summary>
        byte BIT_1_aHL()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 1,A instruction.
        /// </summary>
        byte BIT_1_A()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 2,B instruction.
        /// </summary>
        byte BIT_2_B()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 2,C instruction.
        /// </summary>
        byte BIT_2_C()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 2,D instruction.
        /// </summary>
        byte BIT_2_D()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 2,E instruction.
        /// </summary>
        byte BIT_2_E()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 2,H instruction.
        /// </summary>
        byte BIT_2_H()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 2,L instruction.
        /// </summary>
        byte BIT_2_L()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 2,(HL) instruction.
        /// </summary>
        byte BIT_2_aHL()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 2,A instruction.
        /// </summary>
        byte BIT_2_A()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 3,B instruction.
        /// </summary>
        byte BIT_3_B()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 3,C instruction.
        /// </summary>
        byte BIT_3_C()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 3,D instruction.
        /// </summary>
        byte BIT_3_D()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 3,E instruction.
        /// </summary>
        byte BIT_3_E()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 3,H instruction.
        /// </summary>
        byte BIT_3_H()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 3,L instruction.
        /// </summary>
        byte BIT_3_L()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 3,(HL) instruction.
        /// </summary>
        byte BIT_3_aHL()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 3,A instruction.
        /// </summary>
        byte BIT_3_A()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 4,B instruction.
        /// </summary>
        byte BIT_4_B()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 4,C instruction.
        /// </summary>
        byte BIT_4_C()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 4,D instruction.
        /// </summary>
        byte BIT_4_D()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 4,E instruction.
        /// </summary>
        byte BIT_4_E()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 4,H instruction.
        /// </summary>
        byte BIT_4_H()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 4,L instruction.
        /// </summary>
        byte BIT_4_L()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 4,(HL) instruction.
        /// </summary>
        byte BIT_4_aHL()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 4,A instruction.
        /// </summary>
        byte BIT_4_A()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 5,B instruction.
        /// </summary>
        byte BIT_5_B()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 5,C instruction.
        /// </summary>
        byte BIT_5_C()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 5,D instruction.
        /// </summary>
        byte BIT_5_D()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 5,E instruction.
        /// </summary>
        byte BIT_5_E()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 5,H instruction.
        /// </summary>
        byte BIT_5_H()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 5,L instruction.
        /// </summary>
        byte BIT_5_L()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 5,(HL) instruction.
        /// </summary>
        byte BIT_5_aHL()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 5,A instruction.
        /// </summary>
        byte BIT_5_A()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 6,B instruction.
        /// </summary>
        byte BIT_6_B()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 6,C instruction.
        /// </summary>
        byte BIT_6_C()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 6,D instruction.
        /// </summary>
        byte BIT_6_D()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 6,E instruction.
        /// </summary>
        byte BIT_6_E()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 6,H instruction.
        /// </summary>
        byte BIT_6_H()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 6,L instruction.
        /// </summary>
        byte BIT_6_L()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 6,(HL) instruction.
        /// </summary>
        byte BIT_6_aHL()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 6,A instruction.
        /// </summary>
        byte BIT_6_A()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 7,B instruction.
        /// </summary>
        byte BIT_7_B()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 7,C instruction.
        /// </summary>
        byte BIT_7_C()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 7,D instruction.
        /// </summary>
        byte BIT_7_D()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 7,E instruction.
        /// </summary>
        byte BIT_7_E()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 7,H instruction.
        /// </summary>
        byte BIT_7_H()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 7,L instruction.
        /// </summary>
        byte BIT_7_L()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 7,(HL) instruction.
        /// </summary>
        byte BIT_7_aHL()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The BIT 7,A instruction.
        /// </summary>
        byte BIT_7_A()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 0,B instruction.
        /// </summary>
        byte RES_0_B()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 0,C instruction.
        /// </summary>
        byte RES_0_C()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 0,D instruction.
        /// </summary>
        byte RES_0_D()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 0,E instruction.
        /// </summary>
        byte RES_0_E()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 0,H instruction.
        /// </summary>
        byte RES_0_H()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 0,L instruction.
        /// </summary>
        byte RES_0_L()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 0,(HL) instruction.
        /// </summary>
        byte RES_0_aHL()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 0,A instruction.
        /// </summary>
        byte RES_0_A()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 1,B instruction.
        /// </summary>
        byte RES_1_B()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 1,C instruction.
        /// </summary>
        byte RES_1_C()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 1,D instruction.
        /// </summary>
        byte RES_1_D()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 1,E instruction.
        /// </summary>
        byte RES_1_E()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 1,H instruction.
        /// </summary>
        byte RES_1_H()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 1,L instruction.
        /// </summary>
        byte RES_1_L()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 1,(HL) instruction.
        /// </summary>
        byte RES_1_aHL()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 1,A instruction.
        /// </summary>
        byte RES_1_A()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 2,B instruction.
        /// </summary>
        byte RES_2_B()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 2,C instruction.
        /// </summary>
        byte RES_2_C()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 2,D instruction.
        /// </summary>
        byte RES_2_D()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 2,E instruction.
        /// </summary>
        byte RES_2_E()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 2,H instruction.
        /// </summary>
        byte RES_2_H()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 2,L instruction.
        /// </summary>
        byte RES_2_L()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 2,(HL) instruction.
        /// </summary>
        byte RES_2_aHL()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 2,A instruction.
        /// </summary>
        byte RES_2_A()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 3,B instruction.
        /// </summary>
        byte RES_3_B()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 3,C instruction.
        /// </summary>
        byte RES_3_C()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 3,D instruction.
        /// </summary>
        byte RES_3_D()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 3,E instruction.
        /// </summary>
        byte RES_3_E()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 3,H instruction.
        /// </summary>
        byte RES_3_H()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 3,L instruction.
        /// </summary>
        byte RES_3_L()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 3,(HL) instruction.
        /// </summary>
        byte RES_3_aHL()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 3,A instruction.
        /// </summary>
        byte RES_3_A()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 4,B instruction.
        /// </summary>
        byte RES_4_B()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 4,C instruction.
        /// </summary>
        byte RES_4_C()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 4,D instruction.
        /// </summary>
        byte RES_4_D()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 4,E instruction.
        /// </summary>
        byte RES_4_E()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 4,H instruction.
        /// </summary>
        byte RES_4_H()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 4,L instruction.
        /// </summary>
        byte RES_4_L()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 4,(HL) instruction.
        /// </summary>
        byte RES_4_aHL()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 4,A instruction.
        /// </summary>
        byte RES_4_A()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 5,B instruction.
        /// </summary>
        byte RES_5_B()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 5,C instruction.
        /// </summary>
        byte RES_5_C()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 5,D instruction.
        /// </summary>
        byte RES_5_D()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 5,E instruction.
        /// </summary>
        byte RES_5_E()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 5,H instruction.
        /// </summary>
        byte RES_5_H()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 5,L instruction.
        /// </summary>
        byte RES_5_L()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 5,(HL) instruction.
        /// </summary>
        byte RES_5_aHL()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 5,A instruction.
        /// </summary>
        byte RES_5_A()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 6,B instruction.
        /// </summary>
        byte RES_6_B()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 6,C instruction.
        /// </summary>
        byte RES_6_C()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 6,D instruction.
        /// </summary>
        byte RES_6_D()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 6,E instruction.
        /// </summary>
        byte RES_6_E()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 6,H instruction.
        /// </summary>
        byte RES_6_H()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 6,L instruction.
        /// </summary>
        byte RES_6_L()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 6,(HL) instruction.
        /// </summary>
        byte RES_6_aHL()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 6,A instruction.
        /// </summary>
        byte RES_6_A()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 7,B instruction.
        /// </summary>
        byte RES_7_B()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 7,C instruction.
        /// </summary>
        byte RES_7_C()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 7,D instruction.
        /// </summary>
        byte RES_7_D()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 7,E instruction.
        /// </summary>
        byte RES_7_E()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 7,H instruction.
        /// </summary>
        byte RES_7_H()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 7,L instruction.
        /// </summary>
        byte RES_7_L()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 7,(HL) instruction.
        /// </summary>
        byte RES_7_aHL()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RES 7,A instruction.
        /// </summary>
        byte RES_7_A()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 0,B instruction.
        /// </summary>
        byte SET_0_B()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 0,C instruction.
        /// </summary>
        byte SET_0_C()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 0,D instruction.
        /// </summary>
        byte SET_0_D()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 0,E instruction.
        /// </summary>
        byte SET_0_E()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 0,H instruction.
        /// </summary>
        byte SET_0_H()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 0,L instruction.
        /// </summary>
        byte SET_0_L()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 0,(HL) instruction.
        /// </summary>
        byte SET_0_aHL()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 0,A instruction.
        /// </summary>
        byte SET_0_A()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 1,B instruction.
        /// </summary>
        byte SET_1_B()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 1,C instruction.
        /// </summary>
        byte SET_1_C()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 1,D instruction.
        /// </summary>
        byte SET_1_D()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 1,E instruction.
        /// </summary>
        byte SET_1_E()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 1,H instruction.
        /// </summary>
        byte SET_1_H()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 1,L instruction.
        /// </summary>
        byte SET_1_L()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 1,(HL) instruction.
        /// </summary>
        byte SET_1_aHL()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 1,A instruction.
        /// </summary>
        byte SET_1_A()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 2,B instruction.
        /// </summary>
        byte SET_2_B()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 2,C instruction.
        /// </summary>
        byte SET_2_C()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 2,D instruction.
        /// </summary>
        byte SET_2_D()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 2,E instruction.
        /// </summary>
        byte SET_2_E()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 2,H instruction.
        /// </summary>
        byte SET_2_H()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 2,L instruction.
        /// </summary>
        byte SET_2_L()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 2,(HL) instruction.
        /// </summary>
        byte SET_2_aHL()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 2,A instruction.
        /// </summary>
        byte SET_2_A()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 3,B instruction.
        /// </summary>
        byte SET_3_B()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 3,C instruction.
        /// </summary>
        byte SET_3_C()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 3,D instruction.
        /// </summary>
        byte SET_3_D()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 3,E instruction.
        /// </summary>
        byte SET_3_E()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 3,H instruction.
        /// </summary>
        byte SET_3_H()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 3,L instruction.
        /// </summary>
        byte SET_3_L()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 3,(HL) instruction.
        /// </summary>
        byte SET_3_aHL()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 3,A instruction.
        /// </summary>
        byte SET_3_A()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 4,B instruction.
        /// </summary>
        byte SET_4_B()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 4,C instruction.
        /// </summary>
        byte SET_4_C()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 4,D instruction.
        /// </summary>
        byte SET_4_D()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 4,E instruction.
        /// </summary>
        byte SET_4_E()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 4,H instruction.
        /// </summary>
        byte SET_4_H()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 4,L instruction.
        /// </summary>
        byte SET_4_L()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 4,(HL) instruction.
        /// </summary>
        byte SET_4_aHL()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 4,A instruction.
        /// </summary>
        byte SET_4_A()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 5,B instruction.
        /// </summary>
        byte SET_5_B()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 5,C instruction.
        /// </summary>
        byte SET_5_C()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 5,D instruction.
        /// </summary>
        byte SET_5_D()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 5,E instruction.
        /// </summary>
        byte SET_5_E()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 5,H instruction.
        /// </summary>
        byte SET_5_H()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 5,L instruction.
        /// </summary>
        byte SET_5_L()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 5,(HL) instruction.
        /// </summary>
        byte SET_5_aHL()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 5,A instruction.
        /// </summary>
        byte SET_5_A()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 6,B instruction.
        /// </summary>
        byte SET_6_B()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 6,C instruction.
        /// </summary>
        byte SET_6_C()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 6,D instruction.
        /// </summary>
        byte SET_6_D()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 6,E instruction.
        /// </summary>
        byte SET_6_E()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 6,H instruction.
        /// </summary>
        byte SET_6_H()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 6,L instruction.
        /// </summary>
        byte SET_6_L()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 6,(HL) instruction.
        /// </summary>
        byte SET_6_aHL()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 6,A instruction.
        /// </summary>
        byte SET_6_A()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 7,B instruction.
        /// </summary>
        byte SET_7_B()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 7,C instruction.
        /// </summary>
        byte SET_7_C()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 7,D instruction.
        /// </summary>
        byte SET_7_D()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 7,E instruction.
        /// </summary>
        byte SET_7_E()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 7,H instruction.
        /// </summary>
        byte SET_7_H()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 7,L instruction.
        /// </summary>
        byte SET_7_L()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 7,(HL) instruction.
        /// </summary>
        byte SET_7_aHL()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SET 7,A instruction.
        /// </summary>
        byte SET_7_A()
        {
            throw new NotImplementedException();
        }
    }
}
