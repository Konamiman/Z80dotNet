using System;

namespace Konamiman.Z80dotNet
{
    public static class NumberUtils
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

        public static short SetHighByte(this short target, byte value)
        {
            var result = (ushort)((target & 0x00FF) | (value << 8));
            if (result > 65535)
                return (short)(result - 65536);
            else
                return (short)result;
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

        public static short CreateShort(byte high, byte low)
        {
            return (short)((high << 8) | low);
        }

        public static int GetBit(this byte target, int bit)
        {
            if(bit < 0 || bit > 7)
                throw new InvalidOperationException("bit number must be between 0 and 7");

            return (target & (1 << bit)) == 0 ? 0 : 1;
        }

        public static byte SetBit(this byte target, int bit, int value)
        {
            if(bit < 0 || bit > 7)
                throw new InvalidOperationException("bit number must be between 0 and 7");

            if(value != 0 && value != 1)
                throw new InvalidOperationException("bit value must be 0 or 1");

            if(value == 1) 
            {
                return (byte)(target | (1 << bit));
            }
            else 
            {
                return (byte)(target & ~(1 << bit));
            }
        }
    }
}
