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

        public static Bit GetBit(this byte target, int bitNumber)
        {
            if(bitNumber < 0 || bitNumber > 7)
                throw new InvalidOperationException("bit number must be between 0 and 7");

            return (target & (1 << bitNumber));
        }

        public static byte SetBit(this byte target, int bitNumber, Bit value)
        {
            if(bitNumber < 0 || bitNumber > 7)
                throw new InvalidOperationException("bit number must be between 0 and 7");

            if(value) 
            {
                return (byte)(target | (1 << bitNumber));
            }
            else 
            {
                return (byte)(target & ~(1 << bitNumber));
            }
        }
    }
}
