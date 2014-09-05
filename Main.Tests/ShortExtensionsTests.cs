using NUnit.Framework;

namespace Konamiman.Z80dotNet.Tests
{
    public class ShortExtensionsTests
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
        public void SetHighByte_works_and_converts_short_to_ushort()
        {
            var expected = (ushort)0xDE12;
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
    }
}
