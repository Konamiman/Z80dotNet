using NUnit.Framework;
using AutoFixture;

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
            var AF = NumberUtils.CreateShort(F, A);

            Sut.AF = AF;

            Assert.Multiple(() =>
            {
                Assert.That(Sut.A, Is.EqualTo(A));
                Assert.That(Sut.F, Is.EqualTo(F));
            });
        }

        [Test]
        public void Sets_AF_correctly_from_A_and_F()
        {
            var A = Fixture.Create<byte>();
            var F = Fixture.Create<byte>();
            var expected = NumberUtils.CreateShort(F, A);

            Sut.A = A;
            Sut.F = F;

            Assert.That(Sut.AF, Is.EqualTo(expected));
        }

        [Test]
        public void Gets_B_and_C_correctly_from_BC()
        {
            var B = Fixture.Create<byte>();
            var C = Fixture.Create<byte>();
            var BC = NumberUtils.CreateShort(C, B);

            Sut.BC = BC;

            Assert.Multiple(() =>
            {
                Assert.That(Sut.B, Is.EqualTo(B));
                Assert.That(Sut.C, Is.EqualTo(C));
            });
        }

        [Test]
        public void Sets_BC_correctly_from_B_and_C()
        {
            var B = Fixture.Create<byte>();
            var C = Fixture.Create<byte>();
            var expected = NumberUtils.CreateShort(C, B);

            Sut.B = B;
            Sut.C = C;

            Assert.That(Sut.BC, Is.EqualTo(expected));
        }

        [Test]
        public void Gets_D_and_E_correctly_from_DE()
        {
            var D = Fixture.Create<byte>();
            var E = Fixture.Create<byte>();
            var DE = NumberUtils.CreateShort(E, D);

            Sut.DE = DE;

            Assert.Multiple(() =>
            {
                Assert.That(Sut.D, Is.EqualTo(D));
                Assert.That(Sut.E, Is.EqualTo(E));
            });
        }

        [Test]
        public void Sets_DE_correctly_from_D_and_E()
        {
            var D = Fixture.Create<byte>();
            var E = Fixture.Create<byte>();
            var expected = NumberUtils.CreateShort(E, D);

            Sut.D = D;
            Sut.E = E;

            Assert.That(Sut.DE, Is.EqualTo(expected));
        }

        [Test]
        public void Gets_H_and_L_correctly_from_HL()
        {
            var H = Fixture.Create<byte>();
            var L = Fixture.Create<byte>();
            var HL = NumberUtils.CreateShort(L, H);

            Sut.HL = HL;

            Assert.Multiple(() =>
            {
                Assert.That(Sut.H, Is.EqualTo(H));
                Assert.That(Sut.L, Is.EqualTo(L));
            });
        }

        [Test]
        public void Sets_HL_correctly_from_H_and_L()
        {
            var H = Fixture.Create<byte>();
            var L = Fixture.Create<byte>();
            var expected = NumberUtils.CreateShort(L, H);

            Sut.H = H;
            Sut.L = L;

            Assert.That(Sut.HL, Is.EqualTo(expected));
        }

        [Test]
        public void Gets_CF_correctly_from_F()
        {
            Sut.F = 0xFE;
            Assert.That(Sut.CF.Value, Is.EqualTo(0));

            Sut.F = 0x01;
            Assert.That(Sut.CF.Value, Is.EqualTo(1));
        }

        [Test]
        public void Sets_F_correctly_from_CF()
        {
            Sut.F = 0xFF;
            Sut.CF = 0;
            Assert.That(Sut.F, Is.EqualTo(0xFE));

            Sut.F = 0x00;
            Sut.CF = 1;
            Assert.That(Sut.F, Is.EqualTo(0x01));
        }

        [Test]
        public void Gets_NF_correctly_from_F()
        {
            Sut.F = 0xFD;
            Assert.That(Sut.NF.Value, Is.EqualTo(0));

            Sut.F = 0x02;
            Assert.That(Sut.NF.Value, Is.EqualTo(1));
        }

        [Test]
        public void Sets_F_correctly_from_NF()
        {
            Sut.F = 0xFF;
            Sut.NF = 0;
            Assert.That(Sut.F, Is.EqualTo(0xFD));

            Sut.F = 0x00;
            Sut.NF = 1;
            Assert.That(Sut.F, Is.EqualTo(0x02));
        }

        [Test]
        public void Gets_PF_correctly_from_F()
        {
            Sut.F = 0xFB;
            Assert.That(Sut.PF.Value, Is.EqualTo(0));

            Sut.F = 0x04;
            Assert.That(Sut.PF.Value, Is.EqualTo(1));
        }

        [Test]
        public void Sets_F_correctly_from_PF()
        {
            Sut.F = 0xFF;
            Sut.PF = 0;
            Assert.That(Sut.F, Is.EqualTo(0xFB));

            Sut.F = 0x00;
            Sut.PF = 1;
            Assert.That(Sut.F, Is.EqualTo(0x04));
        }

        [Test]
        public void Gets_Flag3_correctly_from_F()
        {
            Sut.F = 0xF7;
            Assert.That(Sut.Flag3.Value, Is.EqualTo(0));

            Sut.F = 0x08;
            Assert.That(Sut.Flag3.Value, Is.EqualTo(1));
        }

        [Test]
        public void Sets_F_correctly_from_Flag3()
        {
            Sut.F = 0xFF;
            Sut.Flag3 = 0;
            Assert.That(Sut.F, Is.EqualTo(0xF7));

            Sut.F = 0x00;
            Sut.Flag3 = 1;
            Assert.That(Sut.F, Is.EqualTo(0x08));
        }

        [Test]
        public void Gets_HF_correctly_from_F()
        {
            Sut.F = 0xEF;
            Assert.That(Sut.HF.Value, Is.EqualTo(0));

            Sut.F = 0x10;
            Assert.That(Sut.HF.Value, Is.EqualTo(1));
        }

        [Test]
        public void Sets_F_correctly_from_HF()
        {
            Sut.F = 0xFF;
            Sut.HF = 0;
            Assert.That(Sut.F, Is.EqualTo(0xEF));

            Sut.F = 0x00;
            Sut.HF = 1;
            Assert.That(Sut.F, Is.EqualTo(0x10));
        }

        [Test]
        public void Gets_Flag5_correctly_from_F()
        {
            Sut.F = 0xDF;
            Assert.That(Sut.Flag5.Value, Is.EqualTo(0));

            Sut.F = 0x20;
            Assert.That(Sut.Flag5.Value, Is.EqualTo(1));
        }

        [Test]
        public void Sets_F_correctly_from_Flag5()
        {
            Sut.F = 0xFF;
            Sut.Flag5 = 0;
            Assert.That(Sut.F, Is.EqualTo(0xDF));

            Sut.F = 0x00;
            Sut.Flag5 = 1;
            Assert.That(Sut.F, Is.EqualTo(0x20));
        }

        [Test]
        public void Gets_ZF_correctly_from_F()
        {
            Sut.F = 0xBF;
            Assert.That(Sut.ZF.Value, Is.EqualTo(0));

            Sut.F = 0x40;
            Assert.That(Sut.ZF.Value, Is.EqualTo(1));
        }

        [Test]
        public void Sets_F_correctly_from_ZF()
        {
            Sut.F = 0xFF;
            Sut.ZF = 0;
            Assert.That(Sut.F, Is.EqualTo(0xBF));

            Sut.F = 0x00;
            Sut.ZF = 1;
            Assert.That(Sut.F, Is.EqualTo(0x40));
        }

        [Test]
        public void Gets_SF_correctly_from_F()
        {
            Sut.F = 0x7F;
            Assert.That(Sut.SF.Value, Is.EqualTo(0));

            Sut.F = 0x80;
            Assert.That(Sut.SF.Value, Is.EqualTo(1));
        }

        [Test]
        public void Sets_F_correctly_from_SF()
        {
            Sut.F = 0xFF;
            Sut.SF = 0;
            Assert.That(Sut.F, Is.EqualTo(0x7F));

            Sut.F = 0x00;
            Sut.SF = 1;
            Assert.That(Sut.F, Is.EqualTo(0x80));
        }
    }
}
