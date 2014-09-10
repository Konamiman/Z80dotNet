using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Konamiman.Z80dotNet.Tests
{
    public class Z80ProcessorTests_InstructionExecution
    {
        Z80ProcessorForTests Sut { get; set; }
        Fixture Fixture { get; set; }
        Mock<IZ80InstructionExecutor> executor = new Mock<IZ80InstructionExecutor>();
        Mock<IClockSynchronizationHelper> clockSyncHelper = new Mock<IClockSynchronizationHelper>();

        [SetUp]
        public void Setup()
        {
            Fixture = new Fixture();

            Sut = new Z80ProcessorForTests();
            Sut.AutoStopOnRetWithStackEmpty = true;
            Sut.InstructionExecutor = executor.Object;
            Sut.ClockSynchronizationHelper = clockSyncHelper.Object;
        }

        [Test]
        public void Can_create_instances()
        {
            Assert.IsNotNull(Sut);
        }

        [Test]
        public void Start_does_a_Reset_and_sets_execution_context()
        {
            Sut.Registers.PC = Fixture.Create<short>();

            Sut.Start();

            Assert.AreEqual(0, Sut.Registers.PC);
            Assert.IsTrue(Sut.HasInstructionExecutionContext);
        }

        [Test]
        public void Starts_sets_global_state_if_passed_as_not_null()
        {
            var state = Fixture.Create<object>();
            Sut.UserState = null;

            Sut.Start(state);

            Assert.AreSame(state, Sut.UserState);
        }

        [Test]
        public void Starts_does_not_set_global_state_if_passed_as_null()
        {
            Sut.UserState = Fixture.Create<object>();

            Sut.Start(null);

            Assert.IsNotNull(Sut.UserState);
        }

        [Test]
        public void Continue_sets_execution_context_and_does_not_reset()
        {
            var pc = Fixture.Create<short>();
            Sut.Registers.PC = pc;

            Sut.Continue();

            Assert.AreEqual(pc, Sut.Registers.PC);
            Assert.IsTrue(Sut.HasInstructionExecutionContext);
        }
    }
}
