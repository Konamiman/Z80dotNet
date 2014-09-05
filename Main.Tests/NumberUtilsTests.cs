using System;
using NUnit.Framework;

namespace Konamiman.Z80dotNet.Tests
{
    public class NumberUtilsTests
    {
        [Test]
        public void GetHighByte_works_for_ushort_values_over_8000h()
        {
            byte expected = 0xDE;
            var actual = ((ushort)0xDE12).GetHighByte();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetHighByte_works_for_short_values_under_8000h()
        {
            byte expected = 0x12;
            var actual = ((short)0x12DE).GetHighByte();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SetHighByte_works_for_ushort_values_over_8000h()
        {
            var expected = (ushort)0xDE12;
            var actual = ((ushort)0xFF12).SetHighByte(0xDE);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SetHighByte_works_for_short_values_under_8000()
        {
            short expected = 0x12DE;
            var actual = ((short)0x34DE).SetHighByte(0x12);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SetHighByte_works_for_short_values_over_8000()
        {
            short expected = 0xDE12 - 65536;
            var actual = ((short)0x3412).SetHighByte(0xDE);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetLowByte_works_for_shorts()
        {
            byte expected = 0xDE;
            var actual = ((short)0x12DE).GetLowByte();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetLowByte_works_for_ushorts()
        {
            byte expected = 0xDE;
            var actual = ((ushort)0xFFDE).GetLowByte();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SetLowByte_works_for_ushorts()
        {
            var expected = (ushort)0xDE12;
            var actual = ((ushort)0xDEFF).SetLowByte(0x12);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SetLowByte_works_for_shorts()
        {
            var expected = (short)0x12DE;
            var actual = ((short)0x12FF).SetLowByte(0xDE);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CreateShort_works_for_high_bytes_under_80h()
        {
            short expected = 0x12DE;
            var actual = NumberUtils.CreateShort(0x12, 0xDE);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CreateShort_works_for_high_bytes_over_80h()
        {
            short expected = 0xDE12 - 65536;
            var actual = NumberUtils.CreateShort(0xDE, 0x12);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetBit_works_for_lsb()
        {
            byte lsb0 = 0xFE;
            Assert.AreEqual(0, lsb0.GetBit(0));

            byte lsb1 = 0x01;
            Assert.AreEqual(1, lsb1.GetBit(0));
        }

        [Test]
        public void GetBit_works_for_msb()
        {
            byte msb0 = 0x7F;
            Assert.AreEqual(0, msb0.GetBit(7));

            byte msb1 = 0x80;
            Assert.AreEqual(1, msb1.GetBit(7));
        }

        [Test]
        public void GetBit_works_for_middle_bit()
        {
            byte bit4reset = 0x10 ^ 0xFF ;
            Assert.AreEqual(0, bit4reset.GetBit(4));

            byte bit4set = 0x10;
            Assert.AreEqual(1, bit4set.GetBit(4));
        }

        [Test]
        public void GetBit_fails_for_negative_bit_number()
        {
            Assert.Throws<InvalidOperationException>(() => ((byte)0).GetBit(-1));
        }

        [Test]
        public void GetBit_fails_for_bit_number_over_7()
        {
            Assert.Throws<InvalidOperationException>(() => ((byte)0).GetBit(8));
        }

        [Test]
        public void SetBit_works_for_lsb()
        {
            byte lsb0 = 0xFF;
            Assert.AreEqual(0xFE, lsb0.SetBit(0, 0));

            byte lsb1 = 0x00;
            Assert.AreEqual(0x01, lsb1.SetBit(0, 1));
        }

        [Test]
        public void SetBit_works_for_msb()
        {
            byte msb0 = 0xFF;
            Assert.AreEqual(0x7F, msb0.SetBit(7, 0));

            byte msb1 = 0x00;
            Assert.AreEqual(0x80, msb1.SetBit(7, 1));
        }

        [Test]
        public void SetBit_works_for_middle_bit()
        {
            byte bit4reset = 0xFF;
            Assert.AreEqual(0xEF, bit4reset.SetBit(4, 0));

            byte bit4set = 0x00;
            Assert.AreEqual(0x10, bit4set.SetBit(4, 1));
        }

        [Test]
        public void SetBit_fails_for_negative_bit_number()
        {
            Assert.Throws<InvalidOperationException>(() => ((byte)0).SetBit(-1, 0));
        }

        [Test]
        public void SetBit_fails_for_bit_number_over_7()
        {
            Assert.Throws<InvalidOperationException>(() => ((byte)0).SetBit(8, 0));
        }
    }
}
