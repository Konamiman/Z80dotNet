using System;

namespace Konamiman.Z80dotNet
{
    /// <summary>
    /// Represents a set of the Z80 main registers (AF, BC, DE, HL)
    /// </summary>
    public class MainZ80Registers
    {
        /// <summary>
        /// The AF register pair
        /// </summary>
        public short AF { get; set; }

        /// <summary>
        /// The BC register pair
        /// </summary>
        public short BC { get; set; }

        /// <summary>
        /// The DE register pair
        /// </summary>
        public short DE { get; set; }

        /// <summary>
        /// The HL register pair
        /// </summary>
        public short HL { get; set; }

        /// <summary>
        /// The A register.
        /// </summary>
        public byte A
        {
            get
            {
                return AF.GetHighByte();
            }
            set
            {
                AF = AF.SetHighByte(value);
            }
        }

        /// <summary>
        /// The F register.
        /// </summary>
        public byte F
        {
            get
            {
                return AF.GetLowByte();
            }
            set
            {
                AF = AF.SetLowByte(value);
            }
        }

        /// <summary>
        /// The B register.
        /// </summary>
        public byte B
        {
            get
            {
                return BC.GetHighByte();
            }
            set
            {
                BC = BC.SetHighByte(value);
            }
        }

        /// <summary>
        /// The C register.
        /// </summary>
        public byte C
        {
            get
            {
                return BC.GetLowByte();
            }
            set
            {
                BC = BC.SetLowByte(value);
            }
        }

        /// <summary>
        /// The D register.
        /// </summary>
        public byte D
        {
            get
            {
                return DE.GetHighByte();
            }
            set
            {
                DE = DE.SetHighByte(value);
            }
        }

        /// <summary>
        /// The E register.
        /// </summary>
        public byte E
        {
            get
            {
                return DE.GetLowByte();
            }
            set
            {
                DE = DE.SetLowByte(value);
            }
        }

        /// <summary>
        /// The H register.
        /// </summary>
        public byte H
        {
            get
            {
                return HL.GetHighByte();
            }
            set
            {
                HL = HL.SetHighByte(value);
            }
        }

        /// <summary>
        /// The E register.
        /// </summary>
        public byte L
        {
            get
            {
                return HL.GetLowByte();
            }
            set
            {
                HL = HL.SetLowByte(value);
            }
        }

        /// <summary>
        /// The carry (C) flag.
        /// </summary>
        public int CF
        {
            get
            {
                return F.GetBit(0);
            }
            set
            {
                F = F.SetBit(0, value);
            }
        }

        /// <summary>
        /// The addition/substraction (N) flag.
        /// </summary>
        public int NF
        {
            get
            {
                return F.GetBit(1);
            }
            set
            {
                F = F.SetBit(1, value);
            }
        }

        /// <summary>
        /// The parity/overflow (P/V) flag.
        /// </summary>
        public int PF
        {
            get
            {
                return F.GetBit(2);
            }
            set
            {
                F = F.SetBit(2, value);
            }
        }

        /// <summary>
        /// The unused flag at bit 3 of F.
        /// </summary>
        public int Flag3
        {
            get
            {
                return F.GetBit(3);
            }
            set
            {
                F = F.SetBit(3, value);
            }
        }

        /// <summary>
        /// The half carry (H) flag.
        /// </summary>
        public int HF
        {
            get
            {
                return F.GetBit(4);
            }
            set
            {
                F = F.SetBit(4, value);
            }
        }

        /// <summary>
        /// The unused flag at bit 5 of F.
        /// </summary>
        public int Flag5
        {
            get
            {
                return F.GetBit(5);
            }
            set
            {
                F = F.SetBit(5, value);
            }
        }

        /// <summary>
        /// The zero (Z) flag.
        /// </summary>
        public int ZF
        {
            get
            {
                return F.GetBit(6);
            }
            set
            {
                F = F.SetBit(6, value);
            }
        }

        /// <summary>
        /// The sign (S) flag.
        /// </summary>
        public int SF
        {
            get
            {
                return F.GetBit(7);
            }
            set
            {
                F = F.SetBit(7, value);
            }
        }
    }
}