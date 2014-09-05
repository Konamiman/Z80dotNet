using System;

namespace Konamiman.Z80dotNet
{
    public static class NumberUtils
    {
        /// <summary>
        /// Gets the high byte of a short value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte GetHighByte(this short value)
        {
            return (byte)(value >> 8);
        }

        /// <summary>
        /// Gets the high byte of an ushort value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte GetHighByte(this ushort value)
        {
            return (byte)(value >> 8);
        }

        /// <summary>
        /// Returns a modified version of an ushort number that has
        /// the specified value in the high byte.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="highByte"></param>
        /// <returns></returns>
        public static ushort SetHighByte(this ushort value, byte highByte)
        {
            return (ushort)((value & 0x00FF) | (highByte << 8));
        }

        /// <summary>
        /// Returns a modified version of a short number that has
        /// the specified value in the high byte.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="highByte"></param>
        /// <returns></returns>
        public static short SetHighByte(this short value, byte highByte)
        {
            var result = (ushort)((value & 0x00FF) | (highByte << 8));
            if (result > 65535)
                return (short)(result - 65536);
            else
                return (short)result;
        }

        /// <summary>
        /// Gets the low byte of a short value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte GetLowByte(this short value)
        {
            return (byte)(value & 0xFF);
        }

        /// <summary>
        /// Gets the low byte of an ushort value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte GetLowByte(this ushort value)
        {
            return (byte)(value & 0xFF);
        }

        /// <summary>
        /// Returns a modified version of an ushort number that has
        /// the specified value in the low byte.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="lowByte"></param>
        /// <returns></returns>
        public static ushort SetLowByte(this ushort value, byte lowByte)
        {
            return (ushort)((value & 0xFF00) | lowByte);
        }

        /// <summary>
        /// Returns a modified version of a short number that has
        /// the specified value in the low byte.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="lowByte"></param>
        /// <returns></returns>
        public static short SetLowByte(this short value, byte lowByte)
        {
            return (short)((value & 0xFF00) | lowByte);
        }

        /// <summary>
        /// Generates a short number from two bytes.
        /// </summary>
        /// <param name="highByte"></param>
        /// <param name="lowByte"></param>
        /// <returns></returns>
        public static short CreateShort(byte highByte, byte lowByte)
        {
            return (short)((highByte << 8) | lowByte);
        }

        /// <summary>
        /// Gets the value of a certain bit in a byte.
        /// The rightmost bit has position 0, the leftmost bit has position 7.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="bitPosition"></param>
        /// <returns></returns>
        public static Bit GetBit(this byte value, int bitPosition)
        {
            if(bitPosition < 0 || bitPosition > 7)
                throw new InvalidOperationException("bit position must be between 0 and 7");

            return (value & (1 << bitPosition));
        }

        /// <summary>
        /// Sets the value of a certain bit in a byte.
        /// The rightmost bit has position 0, the leftmost bit has position 7.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="bitPosition"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte SetBit(this byte target, int bitPosition, Bit value)
        {
            if(bitPosition < 0 || bitPosition > 7)
                throw new InvalidOperationException("bit position must be between 0 and 7");

            if(value) 
            {
                return (byte)(target | (1 << bitPosition));
            }
            else 
            {
                return (byte)(target & ~(1 << bitPosition));
            }
        }
    }
}
