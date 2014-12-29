using System;

namespace Konamiman.NestorMSX.Misc
{
    /// <summary>
    /// Represents a two-bit number that can be implicitly cast to/from and compared with integers.
    /// </summary>
    public class TwinBit
    {
        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        /// <param name="value">The number that this instance will represent, must be an integer from 0 to 3</param>
        /// <exception cref="InvalidOperationException">The supplied number is invalid</exception>
        public TwinBit(int value)
        {
            if(value < 0 || value > 3)
                throw new InvalidOperationException("Page number must be a number between 0 and 3");

            this.Value = value;
        }

        /// <summary>
        /// Gets the page number that this instance represents
        /// </summary>
        public int Value { get; private set; }

        #region Equality and conversion operators

        public static implicit operator TwinBit(int value)
        {
            return new TwinBit(value);
        }

        public static implicit operator int(TwinBit value)
        {
            return value.Value;
        }

        public static bool operator ==(TwinBit twinBitValue, int intValue)
        {
            return twinBitValue.Value == intValue;
        }

        public static bool operator !=(TwinBit twinBitValue, int intValue)
        {
            return !(twinBitValue == intValue);
        }

        public override bool Equals(object obj)
        {
            if(obj is int)
                return this == (int)obj;
            else if(obj is TwinBit)
                return this.Value == ((TwinBit)obj).Value;
            else
                return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Value;
        }

        #endregion
    }
}
