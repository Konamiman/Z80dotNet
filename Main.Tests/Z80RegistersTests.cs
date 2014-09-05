using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Ploeh.AutoFixture;

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
        public void Gets_IXh_and_IXl_correctly_from_IX()
        {
            var IXh = Fixture.Create<byte>();
            var IXl = Fixture.Create<byte>();
            var IX = NumberUtils.CreateShort(IXh, IXl);

            Sut.IX = IX;

            Assert.AreEqual(IXh, Sut.IXh);
            Assert.AreEqual(IXl, Sut.IXl);
        }

        [Test]
        public void Sets_IX_correctly_from_IXh_and_IXl()
        {
            var IXh = Fixture.Create<byte>();
            var IXl = Fixture.Create<byte>();
            var expected = NumberUtils.CreateShort(IXh, IXl);

            Sut.IXh = IXh;
            Sut.IXl = IXl;

            Assert.AreEqual(expected, Sut.IX);
        }

        [Test]
        public void Gets_IYh_and_IYl_correctly_from_IY()
        {
            var IYh = Fixture.Create<byte>();
            var IYl = Fixture.Create<byte>();
            var IY = NumberUtils.CreateShort(IYh, IYl);

            Sut.IY = IY;

            Assert.AreEqual(IYh, Sut.IYh);
            Assert.AreEqual(IYl, Sut.IYl);
        }

        [Test]
        public void Sets_IY_correctly_from_IYh_and_IYl()
        {
            var IYh = Fixture.Create<byte>();
            var IYl = Fixture.Create<byte>();
            var expected = NumberUtils.CreateShort(IYh, IYl);

            Sut.IYh = IYh;
            Sut.IYl = IYl;

            Assert.AreEqual(expected, Sut.IY);
        }

        [Test]
        public void Gets_I_and_R_correctly_from_IR()
        {
            var I = Fixture.Create<byte>();
            var R = Fixture.Create<byte>();
            var IR = NumberUtils.CreateShort(I, R);

            Sut.IR = IR;

            Assert.AreEqual(I, Sut.I);
            Assert.AreEqual(R, Sut.R);
        }

        [Test]
        public void Sets_IR_correctly_from_I_and_R()
        {
            var I = Fixture.Create<byte>();
            var R = Fixture.Create<byte>();
            var expected = NumberUtils.CreateShort(I, R);

            Sut.I = I;
            Sut.R = R;

            Assert.AreEqual(expected, Sut.IR);
        }

#if false
        [Test]
        public void Can_set_IM_to_0_1_or_2()
        {
            Sut.IM = 0;
            Assert.AreEqual(0, Sut.IM);

            Sut.IM = 1;
            Assert.AreEqual(1, Sut.IM);

            Sut.IM = 2;
            Assert.AreEqual(2, Sut.IM);
        }

        [Test]
        public void Cannot_set_IM_to_larger_than_2()
        {
            Assert.Throws<InvalidOperationException>(() => Sut.IM = 3);
        }
#endif
    }
}
