using System;

namespace Konamiman.NestorMSX.Misc
{
    /// <summary>
    /// Represents a Z80 page number that can be implicitly cast to/from and compared with integers.
    /// </summary>
    public class Z80Page
    {
        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        /// <param name="pageNumber">The page number that this instance will represent, must be an integer from 0 to 3</param>
        /// <exception cref="InvalidOperationException">The supplied page number is invalid</exception>
        public Z80Page(int pageNumber)
        {
            if(pageNumber < 0 || pageNumber > 3)
                throw new InvalidOperationException("Page number must be a number between 0 and 3");

            this.PageNumber = pageNumber;
            this.AddressMask = (ushort)(pageNumber << 14);
        }

        /// <summary>
        /// Returns a new instance that represents the page to which a given address belongs.
        /// </summary>
        /// <param name="address">Address to get the page for</param>
        /// <returns>Page of the supplied address</returns>
        public static Z80Page FromAddress(ushort address)
        {
            return new Z80Page((ushort)(address >> 14));
        }

        /// <summary>
        /// Gets the page number that this instance represents
        /// </summary>
        public int PageNumber { get; private set; }

        /// <summary>
        /// Gets the address mask for this page, calculated as (page number) * 16384
        /// </summary>
        /// <remarks>
        /// An address belongs to a page if (address | 0xC000) = mask of the page.
        /// </remarks>
        public ushort AddressMask { get; private set; }

        #region Equality and conversion operators

        public static implicit operator Z80Page(int value)
        {
            return new Z80Page(value);
        }

        public static implicit operator int(Z80Page value)
        {
            return value.PageNumber;
        }

        public static bool operator ==(Z80Page pageValue, int intValue)
        {
            return pageValue.PageNumber == intValue;
        }

        public static bool operator !=(Z80Page pageValue, int intValue)
        {
            return !(pageValue == intValue);
        }

        public override bool Equals(object obj)
        {
            if(obj is int)
                return this == (int)obj;
            else if(obj is Z80Page)
                return this.PageNumber == ((Z80Page)obj).PageNumber;
            else
                return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return PageNumber;
        }

        #endregion
    }
}
