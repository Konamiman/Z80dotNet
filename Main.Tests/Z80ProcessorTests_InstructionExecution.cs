using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Runtime.Serialization;
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
        private const byte LD_SP_HL_opcode = 0xF9;

        Z80ProcessorForTests Sut { get; set; }
        Fixture Fixture { get; set; }
        Mock<IClockSynchronizationHelper> clockSyncHelper;

        [SetUp]
        public void Setup()
        {
            Fixture = new Fixture();

            Sut = new Z80ProcessorForTests();
            Sut.AutoStopOnRetWithStackEmpty = true;
            Sut.Memory[0] = RET_opcode;

            clockSyncHelper = new Mock<IClockSynchronizationHelper>();

            Sut.InstructionExecutor = new FakeInstructionExecutor() {ProcessorAgent = Sut};
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

            DoBeforeFetch(b => Assert.IsTrue(Sut.HasInstructionExecutionContext));

            Sut.Start();

            Assert.AreEqual(1, Sut.Registers.PC);
        }

        void DoBeforeFetch(Action<byte> code)
        {
            ((FakeInstructionExecutor)Sut.InstructionExecutor).ExtraBeforeFetchCode = code;
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

            Sut.Continue();

            Assert.AreEqual(pc.Inc(), Sut.Registers.PC);
        }

        [Test]
        public void Start_sets_ProcessorState_to_running()
        {
            DoBeforeFetch(b => Assert.AreEqual(ProcessorState.Running, Sut.State));

            Sut.Start();
        }

        [Test]
        public void Execution_invokes_InstructionExecutor_for_each_fetched_opcode()
        {
            Sut.Memory[0] = NOP_opcode;
            Sut.Memory[1] = DI_opcode;
            Sut.Memory[2] = RET_opcode;

            Sut.Start();

            AssertExecuted(NOP_opcode, 1);
            AssertExecuted(DI_opcode, 1);
            AssertExecuted(RET_opcode, 1);
        }

        private void AssertExecuted(byte opcode, int times)
        {
            var dictionary = ((FakeInstructionExecutor)Sut.InstructionExecutor).TimesEachInstructionIsExecuted;
            if(times==0)
                Assert.IsFalse(dictionary.ContainsKey(opcode));
            else
                Assert.AreEqual(dictionary[opcode], times);
        }

        [Test]
        public void StopRequest_stops_execution()
        {
            Sut.Memory[0] = NOP_opcode;
            Sut.Memory[1] = DI_opcode;
            Sut.Memory[2] = RET_opcode;

            DoBeforeFetch(b => { if (b == DI_opcode) Sut.Stop(); });

            Sut.Start();

            AssertExecuted(NOP_opcode, 1);
            AssertExecuted(DI_opcode, 1);
            AssertExecuted(RET_opcode, 0);

            Assert.AreEqual(StopReason.StopInvoked, Sut.StopReason);
            Assert.AreEqual(ProcessorState.Stopped, Sut.State);
        }

        [Test]
        public void PauseRequest_stops_execution()
        {
            Sut.Memory[0] = NOP_opcode;
            Sut.Memory[1] = DI_opcode;
            Sut.Memory[2] = RET_opcode;

            DoBeforeFetch(b => { if (b == DI_opcode) Sut.Stop(isPause: true); });

            Sut.Start();

            AssertExecuted(NOP_opcode, 1);
            AssertExecuted(DI_opcode, 1);
            AssertExecuted(RET_opcode, 0);

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
            DoBeforeFetch(b => Assert.AreEqual(StopReason.NotApplicable, Sut.StopReason));

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

            DoAfterFetch(b => Sut.SetInterruptMode(2));

            Sut.Start();

            Assert.AreEqual(2, Sut.InterruptMode);
        }

        void DoAfterFetch(Action<byte> code)
        {
            ((FakeInstructionExecutor)Sut.InstructionExecutor).ExtraAfterFetchCode = code;
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Auto_stops_when_HALT_on_DI_found_or_when_RET_with_initial_stack_is_found_if_configured_to_do_so(bool autoStopOnDiPlusHalt)
        {
            Sut.AutoStopOnDiPlusHalt = autoStopOnDiPlusHalt;
            Sut.AutoStopOnRetWithStackEmpty = !autoStopOnDiPlusHalt;

            Sut.Memory[0] = DI_opcode;
            Sut.Memory[1] = HALT_opcode;
            Sut.Memory[2] = RET_opcode;

            DoBeforeFetch(b => Sut.Registers.IFF1 = 0);

            Sut.Start();

            AssertExecuted(DI_opcode, 1);
            AssertExecuted(HALT_opcode, 1);
            AssertExecuted(RET_opcode, autoStopOnDiPlusHalt ? 0 : 1);

            Assert.AreEqual(autoStopOnDiPlusHalt ? StopReason.DiPlusHalt : StopReason.RetWithStackEmpty, Sut.StopReason);
            Assert.AreEqual(ProcessorState.Stopped , Sut.State);
        }

        [Test]
        public void Does_not_auto_stop_when_HALT_on_EI_found_regardless_of_AutoStopOnDiPlusHalt_is_true()
        {
            Sut.AutoStopOnDiPlusHalt = true;
            Sut.AutoStopOnRetWithStackEmpty = true;

            Sut.Memory[0] = DI_opcode;
            Sut.Memory[1] = HALT_opcode;
            Sut.Memory[2] = RET_opcode;

            DoBeforeFetch(b => Sut.Registers.IFF1 = 1);

            Sut.Start();

            AssertExecuted(DI_opcode, 1);
            AssertExecuted(HALT_opcode, 1);
            AssertExecuted(RET_opcode, 1);

            Assert.AreEqual(StopReason.RetWithStackEmpty, Sut.StopReason);
            Assert.AreEqual(ProcessorState.Stopped , Sut.State);
        }

        [Test]
        public void Auto_stops_when_RET_is_found_with_stack_equal_to_initial_value_if_AutoStopOnRetWithStackEmpty_is_true()
        {
            Sut.AutoStopOnRetWithStackEmpty = true;

            Sut.Memory[0] = LD_SP_HL_opcode;
            Sut.Memory[1] = RET_opcode;
            Sut.Memory[2] = DI_opcode;

            var spValue = Fixture.Create<short>();

            DoBeforeFetch(b => Sut.Registers.IFF1 = 1);
            DoAfterFetch(b => { if(b == LD_SP_HL_opcode) Sut.Registers.SP = spValue; });

            Sut.Start();

            AssertExecuted(LD_SP_HL_opcode, 1);
            AssertExecuted(RET_opcode, 1);
            AssertExecuted(DI_opcode, 0);

            Assert.AreEqual(StopReason.RetWithStackEmpty, Sut.StopReason);
            Assert.AreEqual(ProcessorState.Stopped , Sut.State);
        }

        [Test]
        public void Does_not_auto_stops_when_RET_is_found_with_stack_not_equal_to_initial_value_if_AutoStopOnRetWithStackEmpty_is_true()
        {
            Sut.AutoStopOnRetWithStackEmpty = true;

            Sut.Memory[0] = NOP_opcode;
            Sut.Memory[1] = RET_opcode;
            Sut.Memory[2] = RET_opcode;
            Sut.Memory[3] = DI_opcode;

            var spValue = Fixture.Create<short>();

            DoBeforeFetch(b => Sut.Registers.IFF1 = 1);

            DoAfterFetch(b =>
            {
                if(b == NOP_opcode)
                    Sut.Registers.SP += 2;
                else if(b == RET_opcode)
                    Sut.Registers.SP -= 2;
            });

            Sut.Start();

            AssertExecuted(NOP_opcode, 1);
            AssertExecuted(RET_opcode, 2);
            AssertExecuted(DI_opcode, 0);

            Assert.AreEqual(StopReason.RetWithStackEmpty, Sut.StopReason);
            Assert.AreEqual(ProcessorState.Stopped , Sut.State);
        }

        private class FakeInstructionExecutor : IZ80InstructionExecutor
        {
            public IZ80ProcessorAgent ProcessorAgent { get; set; }

            public Action<byte> ExtraBeforeFetchCode { get; set; }

            public Action<byte> ExtraAfterFetchCode { get; set; }

            public int Execute(byte firstOpcodeByte)
            {
                if(TimesEachInstructionIsExecuted.ContainsKey(firstOpcodeByte))
                    TimesEachInstructionIsExecuted[firstOpcodeByte]++;
                else
                    TimesEachInstructionIsExecuted[firstOpcodeByte] = 1;

                if(ExtraBeforeFetchCode != null)
                    ExtraBeforeFetchCode(firstOpcodeByte);

                InstructionFetchFinished(this, 
                    new InstructionFetchFinishedEventArgs()
                    {
                        IsLdSpInstruction = (firstOpcodeByte == LD_SP_HL_opcode),
                        IsRetInstruction = (firstOpcodeByte == RET_opcode),
                        IsHaltInstruction = (firstOpcodeByte == HALT_opcode)
                    });

                if(ExtraAfterFetchCode != null)
                    ExtraAfterFetchCode(firstOpcodeByte);

                return 0;
            }

            public Dictionary<byte, int> TimesEachInstructionIsExecuted = new Dictionary<byte, int>();

            public event EventHandler<InstructionFetchFinishedEventArgs> InstructionFetchFinished;
        }

    }
}
