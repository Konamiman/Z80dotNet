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
    }
}