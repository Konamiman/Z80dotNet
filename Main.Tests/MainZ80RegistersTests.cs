using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Konamiman.Z80dotNet.Tests
{
    public class MainZ80RegistersTests
    {
        private Fixture Fixture { get; set; }
        MainZ80Registers Sut { get; set; }

        [SetUp]
        public void Setup()
        {
            Fixture = new Fixture();
            Sut = new MainZ80Registers();
        }

        [Test]
        public void Gets_A_and_F_correctly_from_AF()
        {
            var A = Fixture.Create<byte>();
            var F = Fixture.Create<byte>();
            var AF = NumberUtils.CreateShort(A, F);

            Sut.AF = AF;

            Assert.AreEqual(A, Sut.A);
            Assert.AreEqual(F, Sut.F);
        }

        [Test]
        public void Sets_AF_correctly_from_A_and_F()
        {
            var A = Fixture.Create<byte>();
            var F = Fixture.Create<byte>();
            var expected = NumberUtils.CreateShort(A, F);

            Sut.A = A;
            Sut.F = F;

            Assert.AreEqual(expected, Sut.AF);
        }

        [Test]
        public void Gets_B_and_C_correctly_from_BC()
        {
            var B = Fixture.Create<byte>();
            var C = Fixture.Create<byte>();
            var BC = NumberUtils.CreateShort(B, C);

            Sut.BC = BC;

            Assert.AreEqual(B, Sut.B);
            Assert.AreEqual(C, Sut.C);
        }

        [Test]
        public void Sets_BC_correctly_from_B_and_C()
        {
            var B = Fixture.Create<byte>();
            var C = Fixture.Create<byte>();
            var expected = NumberUtils.CreateShort(B, C);

            Sut.B = B;
            Sut.C = C;

            Assert.AreEqual(expected, Sut.BC);
        }

        [Test]
        public void Gets_D_and_E_correctly_from_DE()
        {
            var D = Fixture.Create<byte>();
            var E = Fixture.Create<byte>();
            var DE = NumberUtils.CreateShort(D, E);

            Sut.DE = DE;

            Assert.AreEqual(D, Sut.D);
            Assert.AreEqual(E, Sut.E);
        }

        [Test]
        public void Sets_DE_correctly_from_D_and_E()
        {
            var D = Fixture.Create<byte>();
            var E = Fixture.Create<byte>();
            var expected = NumberUtils.CreateShort(D, E);

            Sut.D = D;
            Sut.E = E;

            Assert.AreEqual(expected, Sut.DE);
        }

        [Test]
        public void Gets_H_and_L_correctly_from_HL()
        {
            var H = Fixture.Create<byte>();
            var L = Fixture.Create<byte>();
            var HL = NumberUtils.CreateShort(H, L);

            Sut.HL = HL;

            Assert.AreEqual(H, Sut.H);
            Assert.AreEqual(L, Sut.L);
        }

        [Test]
        public void Sets_HL_correctly_from_H_and_L()
        {
            var H = Fixture.Create<byte>();
            var L = Fixture.Create<byte>();
            var expected = NumberUtils.CreateShort(H, L);

            Sut.H = H;
            Sut.L = L;

            Assert.AreEqual(expected, Sut.HL);
        }
    }
}
