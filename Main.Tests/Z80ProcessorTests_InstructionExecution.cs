using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using System;
using System.Collections.Generic;

namespace Konamiman.Z80dotNet.Tests
{
    public class Z80ProcessorTests_InstructionExecution
    {
        private const byte RET_opcode = 0xC9;
        private const byte DI_opcode = 0xF3;
        private const byte HALT_opcode = 0x76;
        private const byte NOP_opcode = 0x00;
        private const byte LD_SP_HL_opcode = 0xF9;

        Z80Processor Sut { get; set; }
        Fixture Fixture { get; set; }
        Mock<IClockSynchronizationHelper> clockSyncHelper;

        [SetUp]
        public void Setup()
        {
            Fixture = new Fixture();

            Sut = new Z80Processor();
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

        #region Start, Stop, Pause, Continue

        [Test]
        public void Start_does_a_Reset()
        {
            Sut.Registers.PC = Fixture.Create<short>();

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

            DoAfterFetch(b => { if (b == DI_opcode) Sut.Stop(); });

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

            DoAfterFetch(b => { if (b == DI_opcode) Sut.Stop(isPause: true); });

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

        #endregion

        #region Conditions at runtime

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

        #endregion

        #region Auto-stop conditions

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

        #endregion

        #region Before and after instruction execution events

        [Test]
        public void Fires_before_and_after_instruction_execution_with_proper_opcodes_and_local_state()
        {
            var executeInvoked = false;
            var beforeEventRaised = false;
            var afterEventRaised = false;
            var localState = Fixture.Create<object>();

            var instructionBytes = new byte[]
            {
                RET_opcode, HALT_opcode, DI_opcode, NOP_opcode
            };
            Sut.Memory.SetContents(0, instructionBytes);

            DoBeforeFetch(b =>
            {
                Sut.FetchNextOpcode();
                Sut.FetchNextOpcode();
                Sut.FetchNextOpcode();
            });

            Sut.BeforeInstructionExecution += (sender, e) =>
            {
                beforeEventRaised = true;
                Assert.IsFalse(executeInvoked);
                executeInvoked = true;
                Assert.AreEqual(instructionBytes, e.Opcode);
                Assert.IsNull(e.LocalUserState);

                e.LocalUserState = localState;
            };

            Sut.AfterInstructionExecution += (sender, e) =>
            {
                afterEventRaised = true;
                Assert.IsTrue(executeInvoked);
                Assert.AreEqual(instructionBytes, e.Opcode);
                Assert.AreEqual(localState, e.LocalUserState);
            };

            Sut.Start();

            Assert.IsTrue(beforeEventRaised);
            Assert.IsTrue(afterEventRaised);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Stops_execution_if_requested_from_AfterInstructionExecutionEvent(bool isPause)
        {
            Sut.Memory[0] = NOP_opcode;
            Sut.Memory[1] = DI_opcode;
            Sut.Memory[2] = HALT_opcode;
            Sut.Memory[3] = RET_opcode;

            Sut.AfterInstructionExecution += (sender, e) =>
            {
                if(e.Opcode[0] == DI_opcode)
                    e.ExecutionStopper.Stop(isPause);
            };

            Sut.Start();

            AssertExecuted(NOP_opcode, 1);
            AssertExecuted(DI_opcode, 1);
            AssertExecuted(HALT_opcode, 0);
            AssertExecuted(RET_opcode, 0);

            Assert.AreEqual(isPause ? StopReason.PauseInvoked : StopReason.StopInvoked, Sut.StopReason);
            Assert.AreEqual(isPause ? ProcessorState.Paused : ProcessorState.Stopped , Sut.State);
        }

        #endregion

        #region Invoking agent members at the right time

        [Test]
        public void ProcessorAgent_members_other_than_FetchNextOpcode_can_be_invoked_only_after_instruction_fetch_complete()
        {
            var address = Fixture.Create<byte>();
            var value = Fixture.Create<byte>();

            DoBeforeFetch(b =>
            {
                Assert.Throws<InvalidOperationException>(() => Sut.ReadFromMemory(address));
                Assert.Throws<InvalidOperationException>(() => Sut.ReadFromPort(address));
                Assert.Throws<InvalidOperationException>(() => Sut.WriteToMemory(address, value));
                Assert.Throws<InvalidOperationException>(() => Sut.WriteToPort(address, value));
                Assert.Throws<InvalidOperationException>(() => Sut.SetInterruptMode(0));
                Assert.Throws<InvalidOperationException>(() => ((IZ80ProcessorAgent)Sut).Registers.ToString());
                Assert.Throws<InvalidOperationException>(() => Sut.Stop());
            });

            DoAfterFetch(b =>
            {
                Sut.ReadFromMemory(address);
                Sut.ReadFromPort(address);
                Sut.WriteToMemory(address, value);
                Sut.WriteToPort(address, value);
                Sut.SetInterruptMode(0);
                var dummy = ((IZ80ProcessorAgent)Sut).Registers;
                Sut.Stop();
            });

            Sut.Start();
        }

        [Test]
        public void FetchNextOpcode_can_be_invoked_only_before_instruction_fetch_complete()
        {
            DoBeforeFetch(b => Sut.FetchNextOpcode());

            DoAfterFetch(b => Assert.Throws<InvalidOperationException>(() => Sut.FetchNextOpcode()));

            Sut.Start();
        }

        #endregion

        #region T states management

        [Test]
        public void Counts_T_states_for_instruction_execution_and_memory_and_ports_access_appropriately()
        {
            var executionStates = Fixture.Create<byte>();
            var M1readMemoryStates = Fixture.Create<byte>();
            var memoryAccessStates = Fixture.Create<byte>();
            var portAccessStates = Fixture.Create<byte>();
            var memoryAddress = Fixture.Create<ushort>();
            var portAddress = Fixture.Create<byte>();
            var value = Fixture.Create<byte>();

            SetStatesReturner(b => executionStates);

            Sut.Memory[0] = NOP_opcode;
            Sut.Memory[1] = LD_SP_HL_opcode;
            Sut.Memory[2] = RET_opcode;

            Sut.SetMemoryWaitStatesForM1(0, 3, M1readMemoryStates);
            Sut.SetMemoryWaitStatesForNonM1(memoryAddress, 1, memoryAccessStates);
            Sut.SetPortWaitStates(portAddress, 1, portAccessStates);

            DoAfterFetch(b =>
            {
                if(b == NOP_opcode)
                {
                    Sut.ReadFromMemory(memoryAddress);
                    Sut.WriteToMemory(memoryAddress, value);
                    Sut.ReadFromPort(portAddress);
                    Sut.WriteToPort(portAddress, value);
                }
            });

            Sut.Start();

            var expected =
                //3 instructions of 1 byte each executed...
                executionStates * 3 +
                M1readMemoryStates * 3 +
                //...plus 1 read+1 write to memory + port
                memoryAccessStates * 2 +
                portAccessStates * 2;

            Assert.AreEqual(expected, Sut.TStatesElapsedSinceReset);
            Assert.AreEqual(expected, Sut.TStatesElapsedSinceStart);
        }

        private void SetStatesReturner(Func<byte, byte> returner)
        {
            ((FakeInstructionExecutor)Sut.InstructionExecutor).TStatesReturner = returner;
        }

        [Test]
        public void Start_sets_all_TStates_to_zero()
        {
            var M1readMemoryStates = Fixture.Create<byte>();
            Sut.SetMemoryWaitStatesForM1(0, 1, M1readMemoryStates);
            var secondRun = false;

            Sut.AfterInstructionExecution += (sender, e) =>
            {
                if(!secondRun)
                {
                    Assert.AreEqual(M1readMemoryStates, Sut.TStatesElapsedSinceReset);
                    Assert.AreEqual(M1readMemoryStates, Sut.TStatesElapsedSinceStart);
                }
            };

            Sut.Start();

            Sut.BeforeInstructionExecution += (sender, e) =>
            {
                Assert.AreEqual(0, Sut.TStatesElapsedSinceStart);
                Assert.AreEqual(0, Sut.TStatesElapsedSinceReset);
            };

            secondRun = true;
            Sut.Start();
        }

        [Test]
        public void Continue_does_not_modify_TStates()
        {
            Sut.Memory[1] = RET_opcode;

            var M1readMemoryStates = Fixture.Create<byte>();
            Sut.SetMemoryWaitStatesForM1(0, 2, M1readMemoryStates);
            var secondRun = false;

            Sut.AfterInstructionExecution += (sender, e) =>
            {
                if(secondRun)
                {
                    Assert.AreEqual(M1readMemoryStates * 2, Sut.TStatesElapsedSinceReset);
                    Assert.AreEqual(M1readMemoryStates * 2, Sut.TStatesElapsedSinceStart);
                }
                else
                {
                    Assert.AreEqual(M1readMemoryStates, Sut.TStatesElapsedSinceReset);
                    Assert.AreEqual(M1readMemoryStates, Sut.TStatesElapsedSinceStart);
                }
            };

            Sut.Start();

            secondRun = true;
            Sut.Continue();
        }

        [Test]
        public void Reset_zeroes_TStatesSinceReset_but_not_TStatesSinceStart()
        {
            var M1readMemoryStates = Fixture.Create<byte>();
            Sut.SetMemoryWaitStatesForM1(0, 1, M1readMemoryStates);
            var secondRun = false;

            Sut.AfterInstructionExecution += (sender, e) =>
            {
                if(secondRun)
                {
                    Assert.AreEqual(M1readMemoryStates, Sut.TStatesElapsedSinceReset);
                    Assert.AreEqual(M1readMemoryStates * 2, Sut.TStatesElapsedSinceStart);
                }
                else
                {
                    Assert.AreEqual(M1readMemoryStates, Sut.TStatesElapsedSinceReset);
                    Assert.AreEqual(M1readMemoryStates, Sut.TStatesElapsedSinceStart);
                }
            };

            Sut.Start();

            secondRun = true;
            Sut.Reset();
            Sut.Continue();
        }

        [Test]
        public void ClockSyncHelper_is_notified_of_total_states_after_instruction_execution()
        {
            var M1readMemoryStates = Fixture.Create<byte>();
            var executionStates = Fixture.Create<byte>();
            
            SetStatesReturner(b => executionStates);

            Sut.SetMemoryWaitStatesForM1(0, 1, M1readMemoryStates);

            Sut.AfterInstructionExecution += (sender, args) => 
                clockSyncHelper.Verify(h => h.TryWait(It.IsAny<int>()), Times.Never());

            Sut.Start();

            clockSyncHelper.Verify(h => h.TryWait(M1readMemoryStates + executionStates));
        }

        [Test]
        public void AfterInstructionExecuted_event_contains_proper_Tstates_count()
        {
            var M1readMemoryStates = Fixture.Create<byte>();
            var executionStates = Fixture.Create<byte>();
            bool instructionExecuted = false;
            
            SetStatesReturner(b => executionStates);

            Sut.SetMemoryWaitStatesForM1(0, 1, M1readMemoryStates);

            Sut.AfterInstructionExecution += (sender, args) =>
            {
                instructionExecuted = true;
                Assert.AreEqual(executionStates + M1readMemoryStates, args.TotalTStates);
            };

            Sut.Start();

            Assert.IsTrue(instructionExecuted);
        }

        #endregion

        #region ExecuteNextInstruction

        [Test]
        public void ExecuteNextInstruction_executes_just_one_instruction_and_finishes()
        {
            Sut.Memory[0] = NOP_opcode;
            Sut.Memory[1] = NOP_opcode;
            Sut.Memory[2] = RET_opcode;
            var instructionsExecutedCount = 0;

            DoBeforeFetch(b => instructionsExecutedCount++);

            Sut.ExecuteNextInstruction();

            Assert.AreEqual(1, instructionsExecutedCount);
        }

        [Test]
        public void ExecuteNextInstruction_always_sets_StopReason_to_ExecuteNextInstructionInvoked()
        {
            Sut.Memory[0] = RET_opcode;
            Sut.ExecuteNextInstruction();
            Assert.AreEqual(StopReason.ExecuteNextInstructionInvoked, Sut.StopReason);

            Sut.Memory[0] = DI_opcode;
            Sut.Reset();
            Sut.ExecuteNextInstruction();
            Assert.AreEqual(StopReason.ExecuteNextInstructionInvoked, Sut.StopReason);

            DoAfterFetch(b => Sut.Stop());
            Sut.Reset();
            Sut.ExecuteNextInstruction();
            Assert.AreEqual(StopReason.ExecuteNextInstructionInvoked, Sut.StopReason);
        }

        [Test]
        public void ExecuteNextInstruction_executes_instructions_sequentially()
        {
            Sut.Memory[0] = RET_opcode;
            Sut.Memory[1] = NOP_opcode;
            Sut.Memory[2] = DI_opcode;

            var executedOpcodes = new List<byte>();

            DoBeforeFetch(executedOpcodes.Add);

            Sut.ExecuteNextInstruction();
            Sut.ExecuteNextInstruction();
            Sut.ExecuteNextInstruction();

            Assert.AreEqual(Sut.Memory.GetContents(0, 3), executedOpcodes);
        }

        [Test]
        public void ExecuteNextInstruction_returns_count_of_elapsed_TStates()
        {
            var executionStates = Fixture.Create<byte>();
            var M1States = Fixture.Create<byte>();
            var memoryReadStates = Fixture.Create<byte>();
            var address = Fixture.Create<ushort>();

            Sut.SetMemoryWaitStatesForM1(0, 1, M1States);
            Sut.SetMemoryWaitStatesForNonM1(address, 1, memoryReadStates);

            SetStatesReturner(b => executionStates);
            DoAfterFetch(b => Sut.ReadFromMemory(address));

            var actual = Sut.ExecuteNextInstruction();
            var expected = executionStates + M1States + memoryReadStates;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ExecuteNextInstruction_updates_TStatesCounts_appropriately()
        {
            var statesCount = Fixture.Create<byte>();

            SetStatesReturner(b => statesCount);

            Sut.ExecuteNextInstruction();
            Assert.AreEqual(statesCount, Sut.TStatesElapsedSinceStart);
            Sut.ExecuteNextInstruction();
            Assert.AreEqual(statesCount * 2, Sut.TStatesElapsedSinceStart);
        }

        #endregion

        #region FakeInstructionExecutor class

        private class FakeInstructionExecutor : IZ80InstructionExecutor
        {
            public IZ80ProcessorAgent ProcessorAgent { get; set; }

            public Action<byte> ExtraBeforeFetchCode { get; set; }

            public Action<byte> ExtraAfterFetchCode { get; set; }

            public Func<byte, byte> TStatesReturner { get; set; }

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

                if(TStatesReturner == null)
                    return 0;
                else
                    return TStatesReturner(firstOpcodeByte);
            }

            public Dictionary<byte, int> TimesEachInstructionIsExecuted = new Dictionary<byte, int>();

            public event EventHandler<InstructionFetchFinishedEventArgs> InstructionFetchFinished;
        }

        #endregion
    }
}
