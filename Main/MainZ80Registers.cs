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
    }
}