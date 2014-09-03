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

        /// <summary>
        /// The current interrupt mode register. It has always the value 0, 1 or 2.
        /// </summary>
        /// <exception cref="System.ArgumentException">Attempt to set a value other than 0, 1 or 2</exception>
        public byte IM { get; set; }    // 0, 1 or 2

        /// <summary>
        /// The IFF1 flag. It has always the value 0 or 1.
        /// </summary>
        /// <exception cref="System.ArgumentException">Attempt to set a value other than 0 or 1</exception>
        public byte IFF1 { get; set; }  // 0 or 1

        /// <summary>
        /// The IFF2 flag. It has always the value 0 or 1.
        /// </summary>
        /// <exception cref="System.ArgumentException">Attempt to set a value other than 0 or 1</exception>
        public byte IFF2 { get; set; }  // 0 or 1
    }
}