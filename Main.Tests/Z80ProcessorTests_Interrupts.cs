using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Konamiman.Z80dotNet.Tests
{
    public class Z80ProcessorTests_Interrupts
    {
        Z80ProcessorForTests Sut { get; set; }
        Fixture Fixture { get; set; }
        Mock<IZ80InterruptSource> InterruptSource1 { get; set; }
        Mock<IZ80InterruptSource> InterruptSource2 { get; set; }

        [SetUp]
        public void Setup()
        {
            Fixture = new Fixture();

            Sut = new Z80ProcessorForTests();
            Sut.SetInstructionExecutionContextToNonNull();

            InterruptSource1 = new Mock<IZ80InterruptSource>();
            InterruptSource2 = new Mock<IZ80InterruptSource>();
        }

        [Test]
        public void Can_create_instances()
        {
            Assert.IsNotNull(Sut);
        }

        #region Interrupt source registration

        [Test]
        public void RegisterInterruptSource_and_GetRegisteredSources_are_simmetrical()
        {
            Sut.RegisterInterruptSource(InterruptSource1.Object);
            Sut.RegisterInterruptSource(InterruptSource2.Object);

            var expected = new[] {InterruptSource1.Object, InterruptSource2.Object};
            var actual = Sut.GetRegisteredInterruptSources();
            CollectionAssert.AreEqual(expected, actual);

        }

        [Test]
        public void RegisterInterruptSource_does_not_register_same_instance_twice()
        {
            Sut.RegisterInterruptSource(InterruptSource1.Object);
            Sut.RegisterInterruptSource(InterruptSource1.Object);

            var expected = new[] {InterruptSource1.Object};
            var actual = Sut.GetRegisteredInterruptSources();
            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void UnregisterAllInterruptSources_clears_sources_list()
        {
            Sut.RegisterInterruptSource(InterruptSource1.Object);
            Sut.RegisterInterruptSource(InterruptSource2.Object);

            Sut.UnregisterAllInterruptSources();

            var actual = Sut.GetRegisteredInterruptSources();
            CollectionAssert.IsEmpty(actual);
        }

        #endregion
    }
}
