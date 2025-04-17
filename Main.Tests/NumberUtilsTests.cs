using NUnit.Framework;
using System;

namespace Konamiman.Z80dotNet.Tests
{
    public class NumberUtilsTests
    {
        [Test]
        public void GetHighByte_works_for_ushort_values_over_8000h()
        {
            byte expected = 0xDE;
            var actual = ((ushort)0xDE12).GetHighByte();
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void GetHighByte_works_for_short_values_under_8000h()
        {
            byte expected = 0x12;
            var actual = ((short)0x12DE).GetHighByte();
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void SetHighByte_works_for_ushort_values_over_8000h()
        {
            var expected = (ushort)0xDE12;
            var actual = ((ushort)0xFF12).SetHighByte(0xDE);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void SetHighByte_works_for_short_values_under_8000()
        {
            short expected = 0x12DE;
            var actual = ((short)0x34DE).SetHighByte(0x12);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void SetHighByte_works_for_short_values_over_8000()
        {
            short expected = 0xDE12 - 65536;
            var actual = ((short)0x3412).SetHighByte(0xDE);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void GetLowByte_works_for_shorts()
        {
            byte expected = 0xDE;
            var actual = ((short)0x12DE).GetLowByte();
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void GetLowByte_works_for_ushorts()
        {
            byte expected = 0xDE;
            var actual = ((ushort)0xFFDE).GetLowByte();
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void SetLowByte_works_for_ushorts()
        {
            var expected = (ushort)0xDE12;
            var actual = ((ushort)0xDEFF).SetLowByte(0x12);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void SetLowByte_works_for_shorts()
        {
            var expected = (short)0x12DE;
            var actual = ((short)0x12FF).SetLowByte(0xDE);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void CreateShort_works_for_high_bytes_under_80h()
        {
            short expected = 0x12DE;
            var actual = NumberUtils.CreateShort(0xDE, 0x12);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void CreateShort_works_for_high_bytes_over_80h()
        {
            short expected = 0xDE12 - 65536;
            var actual = NumberUtils.CreateShort(0x12, 0xDE);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void GetBit_works_for_lsb()
        {
            byte lsb0 = 0xFE;
            Assert.That(lsb0.GetBit(0).Value, Is.EqualTo(0));

            byte lsb1 = 0x01;
            Assert.That(lsb1.GetBit(0).Value, Is.EqualTo(1));
        }

        [Test]
        public void GetBit_works_for_msb()
        {
            byte msb0 = 0x7F;
            Assert.That(msb0.GetBit(7).Value, Is.EqualTo(0));

            byte msb1 = 0x80;
            Assert.That(msb1.GetBit(7).Value, Is.EqualTo(1));
        }

        [Test]
        public void GetBit_works_for_middle_bit()
        {
            byte bit4reset = 0x10 ^ 0xFF ;
            Assert.That(bit4reset.GetBit(4).Value, Is.EqualTo(0));

            byte bit4set = 0x10;
            Assert.That(bit4set.GetBit(4).Value, Is.EqualTo(1));
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
            Assert.That(lsb0.WithBit(0, 0), Is.EqualTo(0xFE));

            byte lsb1 = 0x00;
            Assert.That(lsb1.WithBit(0, 1), Is.EqualTo(0x01));
        }

        [Test]
        public void SetBit_works_for_msb()
        {
            byte msb0 = 0xFF;
            Assert.That(msb0.WithBit(7, 0), Is.EqualTo(0x7F));

            byte msb1 = 0x00;
            Assert.That(msb1.WithBit(7, 1), Is.EqualTo(0x80));
        }

        [Test]
        public void SetBit_works_for_middle_bit()
        {
            byte bit4reset = 0xFF;
            Assert.That(bit4reset.WithBit(4, 0), Is.EqualTo(0xEF));

            byte bit4set = 0x00;
            Assert.That(bit4set.WithBit(4, 1), Is.EqualTo(0x10));
        }

        [Test]
        public void SetBit_fails_for_negative_bit_number()
        {
            Assert.Throws<InvalidOperationException>(() => ((byte)0).WithBit(-1, 0));
        }

        [Test]
        public void SetBit_fails_for_bit_number_over_7()
        {
            Assert.Throws<InvalidOperationException>(() => ((byte)0).WithBit(8, 0));
        }

        [Test]
        public void ToShort_works_for_numbers_below_8000h()
        {
            Assert.That(0x1234.ToShort(), Is.EqualTo((short)0x1234));
        }

        [Test]
        public void ToShort_works_for_numbers_above_8000h()
        {
            Assert.That(0xFFFF.ToShort(), Is.EqualTo((short)-1));
        }

        [Test]
        public void ToShort_works_for_8000h()
        {
            Assert.That(0x8000.ToShort(), Is.EqualTo((short)-32768));
        }

        [Test]
        public void ToUshort_works_for_number_below_zero()
        {
            Assert.That((-1).ToUShort(), Is.EqualTo((ushort)0xFFFF));
        }

        [Test]
        public void ToUshort_works_for_number_above_zero()
        {
            Assert.That(1.ToUShort(), Is.EqualTo((ushort)1));
        }

        [Test]
        public void ToUshort_works_for_zero()
        {
            Assert.That(0.ToUShort(), Is.EqualTo((ushort)0));
        }

        [Test]
        public void ToSignedByte_works_for_numbers_below_80h()
        {
            Assert.That(0x12.ToSignedByte(), Is.EqualTo((SByte)0x12));
        }

        [Test]
        public void ToSignedByte_works_for_80h()
        {
            Assert.That(0x80.ToSignedByte(), Is.EqualTo((SByte)(-128)));
        }

        [Test]
        public void ToSignedByte_works_for_numbers_above_80h()
        {
            Assert.That(0xFF.ToSignedByte(), Is.EqualTo((SByte)(-1)));
        }

        [Test]
        public void Inc_short_works_for_non_boundary_values()
        {
            Assert.That(((short)1).Inc(), Is.EqualTo(2));
        }

        [Test]
        public void Inc_short_works_for_boundary_values()
        {
            Assert.That(0xFFFF.ToShort().Inc(), Is.EqualTo(0));
        }

        [Test]
        public void Inc_ushort_works_for_non_boundary_values()
        {
            Assert.That(((ushort)1).Inc(), Is.EqualTo(2));
        }

        [Test]
        public void Inc_ushort_works_for_boundary_values()
        {
            Assert.That(0xFFFF.ToUShort().Inc(), Is.EqualTo(0));
        }

        [Test]
        public void Dec_short_works_for_non_boundary_values()
        {
            Assert.That(((short)2).Dec(), Is.EqualTo(1));
        }

        [Test]
        public void Dec_short_works_for_boundary_values()
        {
            Assert.That(((short)0).Dec(), Is.EqualTo(0xFFFF.ToShort()));
        }

        [Test]
        public void Add_short_works_for_non_boundary_values()
        {
            Assert.That(((short)2).Add(3), Is.EqualTo(5));
        }

        [Test]
        public void Add_ushort_works_for_boundary_values()
        {
            Assert.That(0xFFFE.ToUShort().Add(3), Is.EqualTo(1));
        }

        [Test]
        public void Add_ushort_works_for_non_boundary_values()
        {
            Assert.That(((ushort)2).Add(3), Is.EqualTo(5));
        }

        [Test]
        public void Add_short_works_for_boundary_values()
        {
            Assert.That(0xFFFE.ToShort().Add(3), Is.EqualTo(1));
        }

        [Test]
        public void Sub_short_works_for_non_boundary_values()
        {
            Assert.That(((short)5).Sub(3), Is.EqualTo(2));
        }

        [Test]
        public void Sub_short_works_for_boundary_values()
        {
            Assert.That(((short)1).Sub(3), Is.EqualTo(0xFFFE.ToShort()));
        }

        [Test]
        public void Sub_ushort_works_for_non_boundary_values()
        {
            Assert.That(((ushort)5).Sub(3), Is.EqualTo(2));
        }

        [Test]
        public void Sub_ushort_works_for_boundary_values()
        {
            Assert.That(((ushort)1).Sub(3), Is.EqualTo(0xFFFE.ToUShort()));
        }

        [Test]
        public void Inc_byte_works_for_non_boundary_values()
        {
            Assert.That(((byte)1).Inc(), Is.EqualTo(2));
        }

        [Test]
        public void Inc_byte_works_for_boundary_values()
        {
            Assert.That(((byte)0xFF).Inc(), Is.EqualTo(0));
        }

        [Test]
        public void Dec_byte_works_for_non_boundary_values()
        {
            Assert.That(((byte)2).Dec(), Is.EqualTo(1));
        }

        [Test]
        public void Dec_byte_works_for_boundary_values()
        {
            Assert.That(((byte)0).Dec(), Is.EqualTo(0xFF.ToShort()));
        }

        [Test]
        public void Add_byte_works_for_non_boundary_values()
        {
            Assert.That(((byte)2).Add(3), Is.EqualTo(5));
        }

        [Test]
        public void Add_byte_works_for_boundary_values()
        {
            Assert.That(((byte)0xFE).Add(3), Is.EqualTo(1));
        }

        [Test]
        public void Sub_byte_works_for_non_boundary_values()
        {
            Assert.That(((byte)5).Sub(3), Is.EqualTo(2));
        }

        [Test]
        public void Sub_byte_works_for_boundary_values()
        {
            Assert.That(((byte)1).Sub(3), Is.EqualTo(0xFE.ToShort()));
        }

        [Test]
        public void Test7Bits_works_as_expected()
        {
            Assert.Multiple(() =>
            {
                Assert.That(((byte)0).Inc7Bits(), Is.EqualTo(1));
                Assert.That(((byte)0x7F).Inc7Bits(), Is.EqualTo(0));
                Assert.That(((byte)0x80).Inc7Bits(), Is.EqualTo(0x81));
                Assert.That(((byte)0xFF).Inc7Bits(), Is.EqualTo(0x80));
            });
        }

        [Test]
        public void Between_works_as_expected()
        {
            byte value = 0x80;

            Assert.That(value.Between(0, 0x7F), Is.False);
            Assert.That(value.Between(0x7F, 0xFF), Is.True);
            Assert.That(value.Between(0x80, 0xFF), Is.True);
            Assert.That(value.Between(0x81, 0xFF), Is.False);
        }

        [Test]
        public void AddSignedByte_works_for_positive_values()
        {
            Assert.Multiple(() =>
            {
                Assert.That(((ushort)0x8000).AddSignedByte(0x10), Is.EqualTo(0x8010));
                Assert.That(((ushort)0x8000).AddSignedByte(0x7F), Is.EqualTo(0x807F));
            });
        }

        [Test]
        public void AddSignedByte_works_for_negative_values()
        {
            Assert.Multiple(() =>
            {
                Assert.That(((ushort)0x8000).AddSignedByte(0xF0), Is.EqualTo(0x7FF0));
                Assert.That(((ushort)0x8000).AddSignedByte(0x80), Is.EqualTo(0x7F80));
            });
        }

        [Test]
        public void ToByteArray_works_for_shorts()
        {
            short value = 0x1234;

            var actual = value.ToByteArray();

            Assert.That(actual, Is.EqualTo(new byte[] {0x34, 0x12}));
        }

        [Test]
        public void ToByteArray_works_for_ushorts()
        {
            ushort value = 0x1234;

            var actual = value.ToByteArray();

            Assert.That(actual, Is.EqualTo(new byte[] {0x34, 0x12}));
        }
    }
}
