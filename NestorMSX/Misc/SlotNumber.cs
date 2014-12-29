using System;

namespace Konamiman.NestorMSX.Misc
{
    /// <summary>
    /// Represents a MSX slot number that can be implicitly cast to/from and compared with bytes.
    /// </summary>
    public class SlotNumber
    {
        /// <summary>
        /// Creates a new instance of the class from an encoded slot number byte.
        /// </summary>
        /// <param name="encodedSlotNumber">Encoded slot number byte (only primary, or primary + 4*sub + 0x80)</param>
        public SlotNumber(byte encodedSlotNumber)
        {
            PrimarySlotNumber = encodedSlotNumber & 3;
            EncodedByte = (byte)(encodedSlotNumber & 0x8F);

            if((encodedSlotNumber & 0x80) == 0) {
                SubSlotNumber = 0;
                IsExpandedSlot = false;
            }
            else {
                SubSlotNumber = (encodedSlotNumber >> 2) & 3;
                IsExpandedSlot = true;
            }
        }

        /// <summary>
        /// Creates a new instance of the class from a primary and secondary slot numbers.
        /// </summary>
        /// <param name="primarySlotNumber">Primary slot number</param>
        /// <param name="subSlotNumber">Secondary slot number</param>
        public SlotNumber(int primarySlotNumber, int subSlotNumber)
        {
            if(!ValidSlotNumberPart(primarySlotNumber) || !ValidSlotNumberPart(subSlotNumber))
                throw new InvalidOperationException("Primary and secondary slot numbers must be in the range 0 to 3");

            PrimarySlotNumber = primarySlotNumber;
            SubSlotNumber = subSlotNumber;
            IsExpandedSlot = true;
            EncodedByte = (byte)(0x80 | (subSlotNumber << 2) | primarySlotNumber);
        }

        private bool ValidSlotNumberPart(int slotNumberPart)
        {
            return slotNumberPart == (slotNumberPart & 3);
        }

        /// <summary>
        /// Gets the primary slot number part of the slot number represented by this instance.
        /// </summary>
        public int PrimarySlotNumber { get; private set; }

        /// <summary>
        /// Gets the secondary slot number part of the slot number represented by this instance.
        /// This value is zero if the represented slot is not expanded.
        /// </summary>
        public int SubSlotNumber { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the represented slot is expanded.
        /// </summary>
        public bool IsExpandedSlot { get; private set; }

        /// <summary>
        /// Gets the encoded byte of the slot number represented by this instance
        /// (only primary, or primary + 4*sub + 0x80)
        /// </summary>
        public byte EncodedByte { get; private set; }

        #region Equality and conversion operators

        public static implicit operator SlotNumber(byte value)
        {
            return new SlotNumber(value);
        }

        public static implicit operator byte(SlotNumber value)
        {
            return value.EncodedByte;
        }

        public static bool operator ==(SlotNumber slotValue, byte byteValue)
        {
            return slotValue.EncodedByte == (byte)(byteValue & 0x8F);
        }

        public static bool operator !=(SlotNumber slotValue, byte byteValue)
        {
            return !(slotValue == byteValue);
        }

        public override bool Equals(object obj)
        {
            if(obj is byte)
                return this.EncodedByte == (byte)(((byte)obj) & 0x8F);
            else if(obj is SlotNumber)
                return this.EncodedByte == (byte)(((SlotNumber)obj).EncodedByte & 0x8F);
            else
                return base.Equals(obj);
        }

        #endregion
    }
}
