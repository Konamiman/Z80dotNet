using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Konamiman.Z80dotNet.Tests
{
    public class Z80ProcessorTests
    {
        Z80Processor Sut { get; set; }

        [SetUp]
        public void Setup()
        {
            Sut = new Z80Processor();
        }

        [Test]
        public void Can_create_instances()
        {
            Assert.IsNotNull(Sut);
        }

        [Test]
        public void Has_proper_defaults()
        {
            Assert.AreEqual(4, Sut.ClockFrequencyInMHz);
            Assert.AreEqual(1, Sut.ClockSpeedFactor);

            Assert.IsFalse(Sut.AutoStopOnDiPlusHalt);
            Assert.IsFalse(Sut.AutoStopOnRetWithStackEmpty);

            Assert.AreEqual(Enumerable.Repeat<byte>(0, 65536).ToArray(), Sut.MemoryWaitStates);
            Assert.AreEqual(Enumerable.Repeat<byte>(0, 256).ToArray(), Sut.PortWaitStates);

            Assert.IsInstanceOf<PlainMemory>(Sut.Memory);
            Assert.AreEqual(65536, Sut.Memory.Size);
            Assert.IsInstanceOf<PlainMemory>(Sut.PortsSpace);
            Assert.AreEqual(256, Sut.PortsSpace.Size);

            for(int i = 0; i < 65536; i++)
                Assert.AreEqual(MemoryAccessMode.ReadAndWrite, Sut.GetMemoryAccessMode((ushort)i));
            for(int i = 0; i < 256; i++)
                Assert.AreEqual(MemoryAccessMode.ReadAndWrite, Sut.GetPortAccessMode((byte)i));

            Assert.IsInstanceOf<Z80Registers>(Sut.Registers);
            Assert.IsInstanceOf<MainZ80Registers>(Sut.Registers.Main);
            Assert.IsInstanceOf<MainZ80Registers>(Sut.Registers.Alternate);
        }
    }
}
