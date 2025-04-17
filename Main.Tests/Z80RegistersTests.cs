using Moq;
using NUnit.Framework;
using AutoFixture;
using System;

namespace Konamiman.Z80dotNet.Tests
{
    public class Z80RegistersTests
    {
        private Fixture Fixture { get; set; }
        Z80Registers Sut { get; set; }

        [SetUp]
        public void Setup()
        {
            Fixture = new Fixture();
            Sut = new Z80Registers();
        }

        [Test]
        public void Alternate_registers_are_properly_set()
        {
            Assert.That(Sut.Alternate, Is.InstanceOf<MainZ80Registers>());
        }

        [Test]
        public void Can_set_Alternate_to_non_null_value()
        {
            var value = new Mock<IMainZ80Registers>().Object;
            Sut.Alternate = value;
            Assert.That(Sut.Alternate, Is.EqualTo(value));
        }

        [Test]
        public void Cannot_set_Alternate_to_null()
        {
            Assert.Throws<ArgumentNullException>(() => Sut.Alternate = null);
        }

        [Test]
        public void Gets_IXh_and_IXl_correctly_from_IX()
        {
            var IXh = Fixture.Create<byte>();
            var IXl = Fixture.Create<byte>();
            var IX = NumberUtils.CreateShort(IXl, IXh);

            Sut.IX = IX;

            Assert.Multiple(() =>
            {
                Assert.That(Sut.IXH, Is.EqualTo(IXh));
                Assert.That(Sut.IXL, Is.EqualTo(IXl));
            });
        }

        [Test]
        public void Sets_IX_correctly_from_IXh_and_IXl()
        {
            var IXh = Fixture.Create<byte>();
            var IXl = Fixture.Create<byte>();
            var expected = NumberUtils.CreateShort(IXl, IXh);

            Sut.IXH = IXh;
            Sut.IXL = IXl;

            Assert.That(Sut.IX, Is.EqualTo(expected));
        }

        [Test]
        public void Gets_IYh_and_IYl_correctly_from_IY()
        {
            var IYh = Fixture.Create<byte>();
            var IYl = Fixture.Create<byte>();
            var IY = NumberUtils.CreateShort(IYl, IYh);

            Sut.IY = IY;

            Assert.Multiple(() =>
            {
                Assert.That(Sut.IYH, Is.EqualTo(IYh));
                Assert.That(Sut.IYL, Is.EqualTo(IYl));
            });
        }

        [Test]
        public void Sets_IY_correctly_from_IYh_and_IYl()
        {
            var IYh = Fixture.Create<byte>();
            var IYl = Fixture.Create<byte>();
            var expected = NumberUtils.CreateShort(IYl, IYh);

            Sut.IYH = IYh;
            Sut.IYL = IYl;

            Assert.That(Sut.IY, Is.EqualTo(expected));
        }

        [Test]
        public void Gets_I_and_R_correctly_from_IR()
        {
            var I = Fixture.Create<byte>();
            var R = Fixture.Create<byte>();
            var IR = NumberUtils.CreateShort(R, I);

            Sut.IR = IR;

            Assert.Multiple(() =>
            {
                Assert.That(Sut.I, Is.EqualTo(I));
                Assert.That(Sut.R, Is.EqualTo(R));
            });
        }

        [Test]
        public void Sets_IR_correctly_from_I_and_R()
        {
            var I = Fixture.Create<byte>();
            var R = Fixture.Create<byte>();
            var expected = NumberUtils.CreateShort(R, I);

            Sut.I = I;
            Sut.R = R;

            Assert.That(Sut.IR, Is.EqualTo(expected));
        }
    }
}
