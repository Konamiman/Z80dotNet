using System;

namespace Konamiman.Z80dotNet
{
    /// <summary>
    /// Represents a full set of Z80 registers.
    /// </summary>
    public class Z80Registers
    {
        /// <summary>
        /// The main register set (AF, BC, DE, HL)
        /// </summary>
        public MainZ80Registers Main { get; set; }

        /// <summary>
        /// The alternate register set (AF', BC', DE', HL')
        /// </summary>
        public MainZ80Registers Alternate { get; set; }

        /// <summary>
        /// The IX register pair
        /// </summary>
        public short IX { get; set; }

        /// <summary>
        /// The IY register pair
        /// </summary>
        public short IY { get; set; }

        /// <summary>
        /// The program counter
        /// </summary>
        public short PC { get; set; }

        /// <summary>
        /// The stack pointer
        /// </summary>
        public short SP { get; set; }

        /// <summary>
        /// The interrupt and refresh register
        /// </summary>
        public short IR { get; set; }

        private byte _IM;
        /// <summary>
        /// The current interrupt mode register. It has always the value 0, 1 or 2.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Attempt to set a value other than 0, 1 or 2</exception>
        public byte IM
        {
            get
            {
                return _IM;
            }
            set
            {
                if(value > 2)
                    throw new InvalidOperationException("IM can be set to 0, 1 or 2 only");

                _IM = value;
            }
        }

        /// <summary>
        /// The IFF1 flag. It has always the value 0 or 1.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Attempt to set a value other than 0 or 1</exception>
        public Bit IFF1 { get; set; }

        /// <summary>
        /// The IFF2 flag. It has always the value 0 or 1.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Attempt to set a value other than 0 or 1</exception>
        public Bit IFF2 { get; set; }  // 0 or 1

        /// <summary>
        /// The IXh register.
        /// </summary>
        public byte IXh
        {
            get
            {
                return IX.GetHighByte();
            }
            set
            {
                IX = IX.SetHighByte(value);
            }
        }

        /// <summary>
        /// The IXl register.
        /// </summary>
        public byte IXl
        {
            get
            {
                return IX.GetLowByte();
            }
            set
            {
                IX = IX.SetLowByte(value);
            }
        }

        /// <summary>
        /// The IYh register.
        /// </summary>
        public byte IYh
        {
            get
            {
                return IY.GetHighByte();
            }
            set
            {
                IY = IY.SetHighByte(value);
            }
        }

        /// <summary>
        /// The IYl register.
        /// </summary>
        public byte IYl
        {
            get
            {
                return IY.GetLowByte();
            }
            set
            {
                IY = IY.SetLowByte(value);
            }
        }

        /// <summary>
        /// The I register.
        /// </summary>
        public byte I
        {
            get
            {
                return IR.GetHighByte();
            }
            set
            {
                IR = IR.SetHighByte(value);
            }
        }

        /// <summary>
        /// The R register.
        /// </summary>
        public byte R
        {
            get
            {
                return IR.GetLowByte();
            }
            set
            {
                IR = IR.SetLowByte(value);
            }
        }
    }
}