namespace Konamiman.Z80dotNet
{
    public static class ShortExtensions
    {
        public static byte GetHighByte(this short value)
        {
            return (byte)(value >> 8);
        }

        public static byte GetHighByte(this ushort value)
        {
            return (byte)(value >> 8);
        }

        public static ushort SetHighByte(this ushort target, byte value)
        {
            return (ushort)((target & 0x00FF) | (value << 8));
        }

        public static ushort SetHighByte(this short target, byte value)
        {
            return (ushort)((target & 0x00FF) | (value << 8));
        }

        public static byte GetLowByte(this short value)
        {
            return (byte)(value & 0xFF);
        }

        public static byte GetLowByte(this ushort value)
        {
            return (byte)(value & 0xFF);
        }

        public static ushort SetLowByte(this ushort target, byte value)
        {
            return (ushort)((target & 0xFF00) | value);
        }

        public static short SetLowByte(this short target, byte value)
        {
            return (short)((target & 0xFF00) | value);
        }
    }
}
