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
        public void Gets_A_correctly_from_AF()
        {
            var A = Fixture.Create<byte>();
            var F = Fixture.Create<byte>();
            var AF = (short)((A << 8) + F);

            Sut.AF = AF;

            var actual = Sut.A;
            Assert.AreEqual(A, actual);
        }
    }
}
