using System.Linq;
using Konamiman.NestorMSX.Hardware;
using NUnit.Framework;

namespace Konamiman.NestorMSX.Tests
{
    public class NotConnectedMemoryTests
    {
        [Test]
        public void Must_use_single_instance()
        {
            Assert.IsNotNull(NotConnectedMemory.Value);
        }

        [Test]
        public void Read_always_returns_0xFF()
        {
            var sut = NotConnectedMemory.Value;
            for(int address = 0; address <= ushort.MaxValue; address++)
                Assert.AreEqual(0xFF, sut[address]);
        }

        [Test]
        public void GetContents_returns_all_0xFF()
        {
            var sut = NotConnectedMemory.Value;
            var contents = sut.GetContents(0, 65535);
            Assert.AreEqual(65535, contents.Length);
            Assert.True(contents.All(b => b==0xFF));
        }

        [Test]
        public void SetContents_does_nothing()
        {
            var sut = NotConnectedMemory.Value;
            sut.SetContents(0, Enumerable.Repeat((byte)0, 65535).ToArray());
            Read_always_returns_0xFF();
        }

        [Test]
        public void Write_does_nothing()
        {
            var sut = NotConnectedMemory.Value;
            for(int address = 0; address <= ushort.MaxValue; address++) {
                sut[address] = 0;
                Assert.AreEqual(0xFF, sut[address]);
            }
        }
    }
}
