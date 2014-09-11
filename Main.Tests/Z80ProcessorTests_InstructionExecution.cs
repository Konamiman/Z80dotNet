using System;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Konamiman.Z80dotNet.Tests
{
    public class Z80ProcessorTests_InstructionExecution
    {
        private const byte RET_opcode = 0xC9;
        private const byte DI_opcode = 0xF3;
        private const byte HALT_opcode = 0x76;
        private const byte NOP_opcode = 0;

        Z80ProcessorForTests Sut { get; set; }
        Fixture Fixture { get; set; }
        Mock<IZ80InstructionExecutor> executor;
        Mock<IClockSynchronizationHelper> clockSyncHelper;

        [SetUp]
        public void Setup()
        {
            Fixture = new Fixture();

            Sut = new Z80ProcessorForTests();
            Sut.AutoStopOnRetWithStackEmpty = true;
            Sut.Memory[0] = RET_opcode;

            executor = new Mock<IZ80InstructionExecutor>();
            clockSyncHelper = new Mock<IClockSynchronizationHelper>();

            executor.SetupGet(x => x.ProcessorAgent).Returns(Sut);
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

            executor.Setup(x => x.Execute(It.IsAny<byte>()))
                .Callback<byte>(b => Assert.IsTrue(Sut.HasInstructionExecutionContext));

            Sut.Start();

            Assert.AreEqual(1, Sut.Registers.PC);
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
        public void No_execution_context_after_start_returns()
        {
            Sut.Start(null);

            Assert.IsFalse(Sut.HasInstructionExecutionContext);
        }

        [Test]
        public void Continue_sets_execution_context_and_does_not_reset()
        {
            var pc = Fixture.Create<short>();
            Sut.Registers.PC = pc;
            Sut.Memory[pc] = RET_opcode;

            executor.Setup(x => x.Execute(It.IsAny<byte>()))
                .Callback<byte>(b => Assert.IsTrue(Sut.HasInstructionExecutionContext));

            Sut.Continue();

            Assert.AreEqual(pc.Inc(), Sut.Registers.PC);
        }

        [Test]
        public void Start_sets_ProcessorState_to_running()
        {
            executor
                .Setup(x => x.Execute(RET_opcode))
                .Callback<byte>(b => Assert.AreEqual(ProcessorState.Running, Sut.State))
                .Returns(0);

            Sut.Start();
        }

        [Test]
        public void Execution_invokes_InstructionExecutor_for_each_fetched_opcode()
        {
            Sut.Memory[0] = NOP_opcode;
            Sut.Memory[1] = DI_opcode;
            Sut.Memory[2] = RET_opcode;

            Sut.Start();

            executor.Verify(e => e.Execute(NOP_opcode), Times.Once());
            executor.Verify(e => e.Execute(DI_opcode), Times.Once());
            executor.Verify(e => e.Execute(RET_opcode), Times.Once());
        }

        [Test]
        public void StopRequest_stops_execution()
        {
            Sut.Memory[0] = NOP_opcode;
            Sut.Memory[1] = DI_opcode;
            Sut.Memory[2] = RET_opcode;

            executor
                .Setup(x => x.Execute(DI_opcode))
                .Callback<byte>(b => executor.Object.ProcessorAgent.Stop())
                .Returns(0);

            Sut.Start();

            executor.Verify(e => e.Execute(NOP_opcode), Times.Once());
            executor.Verify(e => e.Execute(DI_opcode), Times.Once());
            executor.Verify(e => e.Execute(RET_opcode), Times.Never());

            Assert.AreEqual(StopReason.StopInvoked, Sut.StopReason);
            Assert.AreEqual(ProcessorState.Stopped, Sut.State);
        }

        [Test]
        public void PauseRequest_stops_execution()
        {
            Sut.Memory[0] = NOP_opcode;
            Sut.Memory[1] = DI_opcode;
            Sut.Memory[2] = RET_opcode;

            executor
                .Setup(x => x.Execute(DI_opcode))
                .Callback<byte>(b => executor.Object.ProcessorAgent.Stop(isPause: true))
                .Returns(0);

            Sut.Start();

            executor.Verify(e => e.Execute(NOP_opcode), Times.Once());
            executor.Verify(e => e.Execute(DI_opcode), Times.Once());
            executor.Verify(e => e.Execute(RET_opcode), Times.Never());

            Assert.AreEqual(StopReason.PauseInvoked, Sut.StopReason);
            Assert.AreEqual(ProcessorState.Paused , Sut.State);
        }

        [Test]
        public void Cannot_stop_if_no_execution_context()
        {
            Assert.Throws<InvalidOperationException>(() => Sut.Stop());
        }

        [Test]
        public void StopReason_is_not_applicable_while_executing()
        {
            executor
                .Setup(x => x.Execute(RET_opcode))
                .Callback<byte>(b => Assert.AreEqual(StopReason.NotApplicable, Sut.StopReason))
                .Returns(0);

            Sut.Start();
        }

        [Test]
        public void Cannot_change_interrupt_mode_from_agent_interface_if_no_execution_context()
        {
            Assert.Throws<InvalidOperationException>(() => Sut.SetInterruptMode(0));
        }

        [Test]
        public void Can_change_interrupt_mode()
        {
            Sut.InterruptMode = 0;

            executor
                .Setup(x => x.Execute(RET_opcode))
                .Callback<byte>(b => executor.Object.ProcessorAgent.SetInterruptMode(2))
                .Returns(0);

            Sut.Start();

            Assert.AreEqual(2, Sut.InterruptMode);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Auto_stops_when_HALT_on_DI_found_only_if_AutoStopOnDiPlusHalt_is_true(bool autoStopIsEnabled)
        {
            Sut.AutoStopOnDiPlusHalt = autoStopIsEnabled;

            Sut.Memory[0] = DI_opcode;
            Sut.Memory[1] = HALT_opcode;
            Sut.Memory[2] = RET_opcode;

            executor
                .Setup(x => x.Execute(DI_opcode))
                .Callback<byte>(b => executor.Object.ProcessorAgent.Registers.IFF1 = 0)
                .Returns(0);

            Sut.Start();

            executor.Verify(e => e.Execute(DI_opcode), Times.Once());
            executor.Verify(e => e.Execute(HALT_opcode), Times.Once());
            executor.Verify(e => e.Execute(RET_opcode), autoStopIsEnabled ? Times.Never() : Times.Once());

            Assert.AreEqual(autoStopIsEnabled ? StopReason.DiPlusHalt : StopReason.RetWithStackEmpty, Sut.StopReason);
            Assert.AreEqual(ProcessorState.Stopped , Sut.State);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Does_not_auto_stop_when_HALT_on_EI_found_regardless_of_AutoStopOnDiPlusHalt_is_true(bool autoStopIsEnabled)
        {
            Sut.AutoStopOnDiPlusHalt = autoStopIsEnabled;

            Sut.Memory[0] = DI_opcode;
            Sut.Memory[1] = HALT_opcode;
            Sut.Memory[2] = RET_opcode;

            executor
                .Setup(x => x.Execute(DI_opcode))
                .Callback<byte>(b => executor.Object.ProcessorAgent.Registers.IFF1 = 1)
                .Returns(0);

            Sut.Start();

            executor.Verify(e => e.Execute(DI_opcode), Times.Once());
            executor.Verify(e => e.Execute(HALT_opcode), Times.Once());
            executor.Verify(e => e.Execute(RET_opcode), Times.Once());

            Assert.AreEqual(StopReason.RetWithStackEmpty, Sut.StopReason);
            Assert.AreEqual(ProcessorState.Stopped , Sut.State);
        }
    }
}
