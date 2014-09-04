using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Konamiman.Z80dotNet.Tests
{
    public class PlainMemoryTests
    {
        private int MemorySize { get; set; }
        private PlainMemory Sut { get; set; }
        private Fixture Fixture { get; set; }

        [SetUp]
        public void Setup()
        {
            Fixture = new Fixture();
            MemorySize = Fixture.Create<int>();
            Sut = new PlainMemory(MemorySize);
        }

        [Test]
        public void Can_create_instances()
        {
            Assert.IsNotNull(Sut);
        }
    }
}
