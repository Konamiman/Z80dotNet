using Moq;
using NUnit.Framework;
using AutoFixture;
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

        Z80ProcessorForTests Sut { get; set; }
        Fixture Fixture { get; set; }
        Mock<IClockSynchronizer> clockSyncHelper;

        [SetUp]
        public void Setup()
        {
            Fixture = new Fixture();

            Sut = new Z80ProcessorForTests();
            Sut.AutoStopOnRetWithStackEmpty = true;
            Sut.Memory[0] = RET_opcode;
            Sut.MustFailIfNoInstructionFetchComplete = true;

            clockSyncHelper = new Mock<IClockSynchronizer>();

            Sut.InstructionExecutor = new FakeInstructionExecutor() {ProcessorAgent = Sut};
            Sut.ClockSynchronizer = clockSyncHelper.Object;
        }

        [Test]
        public void Can_create_instances()
        {
            Assert.That(Sut, Is.Not.Null);
        }

        #region Start, Stop, Pause, Continue

        [Test]
        public void Start_does_a_Reset()
        {
            Sut.Registers.PC = Fixture.Create<ushort>();

            Sut.Start();

            Assert.That(Sut.Registers.PC, Is.EqualTo(1));
        }

        void DoBeforeFetch(Action<byte> code)
        {
            ((FakeInstructionExecutor)Sut.InstructionExecutor).ExtraBeforeFetchCode = code;
        }

        [Test]
        public void Start_sets_StartOfStack_to_0xFFFF()
        {
            Sut.SetStartOFStack(Fixture.Create<short>());

            Sut.AutoStopOnDiPlusHalt = true;
            Sut.Memory[0] = DI_opcode;
            Sut.Memory[1] = HALT_opcode;

            Sut.Start();

            Assert.That(Sut.StartOfStack, Is.EqualTo(0xFFFF.ToShort()));
        }

        [Test]
        public void Starts_sets_global_state_if_passed_as_not_null()
        {
            var state = Fixture.Create<object>();
            Sut.UserState = null;

            Sut.Start(state);

            Assert.That(state, Is.SameAs(Sut.UserState));
        }

        [Test]
        public void Starts_does_not_set_global_state_if_passed_as_null()
        {
            Sut.UserState = Fixture.Create<object>();

            Sut.Start(null);

            Assert.That(Sut.UserState, Is.Not.Null);
        }

        [Test]
        public void Continue_sets_execution_context_and_does_not_reset()
        {
            var pc = Fixture.Create<ushort>();
            Sut.Registers.PC = pc;
            Sut.Memory[pc] = RET_opcode;
            Sut.SetStartOFStack(Sut.Registers.SP);

            Sut.Continue();

            Assert.That(Sut.Registers.PC, Is.EqualTo(pc.Inc()));
        }

        [Test]
        public void Start_sets_ProcessorState_to_running()
        {
            DoBeforeFetch(b => Assert.That(Sut.State, Is.EqualTo(ProcessorState.Running)));

            Sut.Start();
        }

        private void AssertExecuted(byte opcode, int times)
        {
            var dictionary = ((FakeInstructionExecutor)Sut.InstructionExecutor).TimesEachInstructionIsExecuted;
            if(times==0)
                Assert.That(dictionary, Does.Not.ContainKey(opcode));
            else
                Assert.That(times, Is.EqualTo(dictionary[opcode]));
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

            Assert.Multiple(() =>
            {
                Assert.That(Sut.StopReason, Is.EqualTo(StopReason.StopInvoked));
                Assert.That(Sut.State, Is.EqualTo(ProcessorState.Stopped));
            });
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

            Assert.Multiple(() =>
            {
                Assert.That(Sut.StopReason, Is.EqualTo(StopReason.PauseInvoked));
                Assert.That(Sut.State, Is.EqualTo(ProcessorState.Paused));
            });
        }

        [Test]
        public void Cannot_stop_if_no_execution_context()
        {
            Assert.Throws<InvalidOperationException>(() => Sut.Stop());
        }

        [Test]
        public void StopReason_is_not_applicable_while_executing()
        {
            DoBeforeFetch(b => Assert.That(Sut.StopReason, Is.EqualTo(StopReason.NotApplicable)));

            Sut.Start();
        }

        [Test]
        public void Has_proper_state_after_unhandled_exception()
        {
            DoBeforeFetch(b => {throw new Exception();} );

            Assert.Throws<Exception>(() => Sut.Start());

            Assert.Multiple(() =>
            {
                Assert.That(Sut.State, Is.EqualTo(ProcessorState.Stopped));
                Assert.That(Sut.StopReason, Is.EqualTo(StopReason.ExceptionThrown));
            });

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

            Assert.That(Sut.InterruptMode, Is.EqualTo(2));
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
            Sut.Memory[1] = autoStopOnDiPlusHalt ? HALT_opcode : NOP_opcode;
            Sut.Memory[2] = RET_opcode;

            DoBeforeFetch(b => Sut.Registers.IFF1 = 0);

            Sut.Start();

            AssertExecuted(DI_opcode, 1);
            AssertExecuted(HALT_opcode, autoStopOnDiPlusHalt ? 1 : 0);
            AssertExecuted(RET_opcode, autoStopOnDiPlusHalt ? 0 : 1);

            Assert.Multiple(() =>
            {
                Assert.That(Sut.StopReason, Is.EqualTo(autoStopOnDiPlusHalt ? StopReason.DiPlusHalt : StopReason.RetWithStackEmpty));
                Assert.That(Sut.State, Is.EqualTo(ProcessorState.Stopped));
            });
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

            var instructionsExecutedCount = 0;

            Sut.AfterInstructionExecution +=
                (sender, args) =>
                {
                    if(instructionsExecutedCount == 5)
                        args.ExecutionStopper.Stop();
                    else
                        instructionsExecutedCount++;
                };

            Sut.Start();

            AssertExecuted(DI_opcode, 1);
            AssertExecuted(HALT_opcode, 1);
            AssertExecuted(RET_opcode, 0);

            Assert.Multiple(() =>
            {
                Assert.That(instructionsExecutedCount, Is.EqualTo(5));
                Assert.That(Sut.StopReason, Is.EqualTo(StopReason.StopInvoked));
                Assert.That(Sut.State, Is.EqualTo(ProcessorState.Stopped));
            });
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

            Assert.Multiple(() =>
            {
                Assert.That(Sut.StopReason, Is.EqualTo(StopReason.RetWithStackEmpty));
                Assert.That(Sut.State, Is.EqualTo(ProcessorState.Stopped));
            });
        }

        [Test]
        public void LD_SP_instructions_change_value_of_StartOfStack()
        {
            Sut.AutoStopOnRetWithStackEmpty = true;

            Sut.Memory[0] = LD_SP_HL_opcode;
            Sut.Memory[1] = RET_opcode;
            Sut.SetStartOFStack(Fixture.Create<short>());

            var spValue = Fixture.Create<short>();

            DoAfterFetch(b => { if(b == LD_SP_HL_opcode) Sut.Registers.SP = spValue; });

            Sut.Start();

            AssertExecuted(LD_SP_HL_opcode, 1);
            AssertExecuted(RET_opcode, 1);

            Assert.Multiple(() =>
            {
                Assert.That(Sut.StopReason, Is.EqualTo(StopReason.RetWithStackEmpty));
                Assert.That(Sut.State, Is.EqualTo(ProcessorState.Stopped));
                Assert.That(Sut.StartOfStack, Is.EqualTo(spValue));
            });
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

            Assert.Multiple(() =>
            {
                Assert.That(Sut.StopReason, Is.EqualTo(StopReason.RetWithStackEmpty));
                Assert.That(Sut.State, Is.EqualTo(ProcessorState.Stopped));
            });
        }

        #endregion

        #region Before and after instruction execution events

        [Test]
        public void Fires_before_and_after_instruction_execution_with_proper_opcodes_and_local_state()
        {
            var executeInvoked = false;
            var beforeFetchEventRaised = false;
            var beforeExecutionEventRaised = false;
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

            Sut.BeforeInstructionFetch += (sender, e) =>
            {
                beforeFetchEventRaised = true;
                Assert.That(executeInvoked, Is.False);
                Assert.That(beforeExecutionEventRaised, Is.False);
                Assert.That(afterEventRaised, Is.False);
                executeInvoked = false;
                Assert.That(e.LocalUserState, Is.Null);

                e.LocalUserState = localState;
            };

            Sut.BeforeInstructionExecution += (sender, e) =>
            {
                beforeExecutionEventRaised = true;
                Assert.Multiple(() => {
                    Assert.That(executeInvoked, Is.False);
                    Assert.That(beforeExecutionEventRaised, Is.True);
                    Assert.That(afterEventRaised, Is.False);
                });
                executeInvoked = true;
                Assert.Multiple(() =>
                {
                    Assert.That(e.Opcode, Is.EqualTo(instructionBytes));
                    Assert.That(e.LocalUserState, Is.EqualTo(localState));
                });
            };

            Sut.AfterInstructionExecution += (sender, e) =>
            {
                afterEventRaised = true;
                Assert.Multiple(() =>
                {
                    Assert.That(executeInvoked);
                    Assert.That(beforeFetchEventRaised);
                    Assert.That(beforeExecutionEventRaised);
                    Assert.That(e.Opcode, Is.EqualTo(instructionBytes));
                    Assert.That(e.LocalUserState, Is.EqualTo(localState));
                });
            };

            Sut.Start();

            Assert.That(beforeFetchEventRaised);
            Assert.That(beforeExecutionEventRaised);
            Assert.That(afterEventRaised);
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

            Assert.Multiple(() =>
            {
                Assert.That(Sut.StopReason, Is.EqualTo(isPause ? StopReason.PauseInvoked : StopReason.StopInvoked));
                Assert.That(Sut.State, Is.EqualTo(isPause ? ProcessorState.Paused : ProcessorState.Stopped));
            });
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Stops_execution_if_requested_from_BeforeInstructionFetchEvent(bool isPause)
        {
            Sut.Memory[0] = NOP_opcode;
            Sut.Memory[1] = DI_opcode;
            Sut.Memory[2] = HALT_opcode;
            Sut.Memory[3] = RET_opcode;

            Sut.BeforeInstructionFetch += (sender, e) =>
            {
                if(Sut.Registers.PC == 2)
                    e.ExecutionStopper.Stop(isPause);
            };

            Sut.Start();

            AssertExecuted(NOP_opcode, 1);
            AssertExecuted(DI_opcode, 1);
            AssertExecuted(HALT_opcode, 0);
            AssertExecuted(RET_opcode, 0);

            Assert.Multiple(() =>
            {
                Assert.That(Sut.StopReason, Is.EqualTo(isPause ? StopReason.PauseInvoked : StopReason.StopInvoked));
                Assert.That(Sut.State, Is.EqualTo(isPause ? ProcessorState.Paused : ProcessorState.Stopped));
            });
        }

        #endregion

        #region Invoking agent members at the right time

        [Test]
        public void ProcessorAgent_members_other_than_FetchNextOpcode_and_Registers_can_be_invoked_only_after_instruction_fetch_complete()
        {
            var address = Fixture.Create<byte>();
            var value = Fixture.Create<byte>();

            Sut.MustFailIfNoInstructionFetchComplete = true;

            DoBeforeFetch(b =>
            {
                Assert.Throws<InvalidOperationException>(() => Sut.ReadFromMemory(address));
                Assert.Throws<InvalidOperationException>(() => Sut.ReadFromPort(address));
                Assert.Throws<InvalidOperationException>(() => Sut.WriteToMemory(address, value));
                Assert.Throws<InvalidOperationException>(() => Sut.WriteToPort(address, value));
                Assert.Throws<InvalidOperationException>(() => Sut.SetInterruptMode(0));
                var dummy = ((IZ80ProcessorAgent)Sut).Registers;
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

            Assert.Multiple(() =>
            {
                Assert.That(Sut.TStatesElapsedSinceReset, Is.EqualTo(expected));
                Assert.That(Sut.TStatesElapsedSinceStart, Is.EqualTo(expected));
            });
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
                    Assert.Multiple(() =>
                    {
                        Assert.That(Sut.TStatesElapsedSinceReset, Is.EqualTo(M1readMemoryStates));
                        Assert.That(Sut.TStatesElapsedSinceStart, Is.EqualTo(M1readMemoryStates));
                    });
                }
            };

            Sut.Start();

            Sut.BeforeInstructionExecution += (sender, e) =>
            {
                Assert.Multiple(() =>
                {
                    Assert.That(Sut.TStatesElapsedSinceStart, Is.EqualTo(0));
                    Assert.That(Sut.TStatesElapsedSinceReset, Is.EqualTo(0));
                });
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
                    Assert.Multiple(() =>
                    {
                        Assert.That(Sut.TStatesElapsedSinceReset, Is.EqualTo(M1readMemoryStates * 2));
                        Assert.That(Sut.TStatesElapsedSinceStart, Is.EqualTo(M1readMemoryStates * 2));
                    });
                }
                else
                {
                    Assert.Multiple(() =>
                    {
                        Assert.That(Sut.TStatesElapsedSinceReset, Is.EqualTo(M1readMemoryStates));
                        Assert.That(Sut.TStatesElapsedSinceStart, Is.EqualTo(M1readMemoryStates));
                    });
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
                    Assert.Multiple(() =>
                    {
                        Assert.That(Sut.TStatesElapsedSinceReset, Is.EqualTo(M1readMemoryStates));
                        Assert.That(Sut.TStatesElapsedSinceStart, Is.EqualTo(M1readMemoryStates * 2));
                    });
                }
                else
                {
                    Assert.Multiple(() =>
                    {
                        Assert.That(Sut.TStatesElapsedSinceReset, Is.EqualTo(M1readMemoryStates));
                        Assert.That(Sut.TStatesElapsedSinceStart, Is.EqualTo(M1readMemoryStates));
                    });
                }
            };

            Sut.Start();

            secondRun = true;
            Sut.Reset();
            Sut.Continue();
        }

        [Test]
        public void Reset_sets_StartOfStack_to_0xFFFF()
        {
            Sut.SetStartOFStack(Fixture.Create<short>());

            Sut.Reset();

            Assert.That(Sut.StartOfStack, Is.EqualTo(0xFFFF.ToShort()));
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
                Assert.That(args.TotalTStates, Is.EqualTo(executionStates + M1readMemoryStates));
            };

            Sut.Start();

            Assert.That(instructionExecuted);
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

            Assert.That(instructionsExecutedCount, Is.EqualTo(1));
        }

        [Test]
        public void ExecuteNextInstruction_always_sets_StopReason_to_ExecuteNextInstructionInvoked()
        {
            Sut.Memory[0] = RET_opcode;
            Sut.ExecuteNextInstruction();
            Assert.That(Sut.StopReason, Is.EqualTo(StopReason.ExecuteNextInstructionInvoked));

            Sut.Memory[0] = DI_opcode;
            Sut.Reset();
            Sut.ExecuteNextInstruction();
            Assert.That(Sut.StopReason, Is.EqualTo(StopReason.ExecuteNextInstructionInvoked));

            DoAfterFetch(b => Sut.Stop());
            Sut.Reset();
            Sut.ExecuteNextInstruction();
            Assert.That(Sut.StopReason, Is.EqualTo(StopReason.ExecuteNextInstructionInvoked));
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

            Assert.That(executedOpcodes, Is.EqualTo(Sut.Memory.GetContents(0, 3)));
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
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void ExecuteNextInstruction_updates_TStatesCounts_appropriately()
        {
            var statesCount = Fixture.Create<byte>();

            SetStatesReturner(b => statesCount);

            Sut.ExecuteNextInstruction();
            Assert.That(Sut.TStatesElapsedSinceStart, Is.EqualTo(statesCount));
            Sut.ExecuteNextInstruction();
            Assert.That(Sut.TStatesElapsedSinceStart, Is.EqualTo(statesCount * 2));
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

        #region InstructionFetchFinishedEventNotFiredException

        [Test]
        public void Fires_InstructionFetchFinishedEventNotFiredException_if_Execute_returns_without_firing_event()
        {
            var address = Fixture.Create<ushort>();
            Sut.Memory[address] = RET_opcode;
            Sut.Registers.PC = address;

            var executor = new Mock<IZ80InstructionExecutor>();
            executor.Setup(e => e.Execute(It.IsAny<byte>())).Returns(0);
            Sut.InstructionExecutor = executor.Object;

            var exception = Assert.Throws<InstructionFetchFinishedEventNotFiredException>(Sut.Continue);

            Assert.Multiple(() =>
            {
                Assert.That(exception.InstructionAddress, Is.EqualTo(address));
                Assert.That(exception.FetchedBytes, Is.EqualTo(new Byte[] { RET_opcode }));
            });
        }

        #endregion

         #region PeekNextOpcode

        [Test]
        public void PeekNextOpcode_returns_next_opcode_without_increasing_PC_and_without_elapsing_T_states()
        {
            var executionStates = Fixture.Create<byte>();
            var M1readMemoryStates = Fixture.Create<byte>();
            var memoryAccessStates = Fixture.Create<byte>();
            var memoryAddress = Fixture.Create<ushort>();

            SetStatesReturner(b => executionStates);

            Sut.Memory[0] = NOP_opcode;
            Sut.Memory[1] = LD_SP_HL_opcode;
            Sut.Memory[2] = RET_opcode;

            Sut.SetMemoryWaitStatesForM1(0, 3, M1readMemoryStates);
            Sut.SetMemoryWaitStatesForNonM1(memoryAddress, 1, memoryAccessStates);

            var beforeInvoked = false;

            DoBeforeFetch(b =>
            {
                if(b == LD_SP_HL_opcode)
                {
                    beforeInvoked = true;
                    for (int i = 0; i < 3; i++)
                    {
                        var oldPC = Sut.Registers.PC;
                        Assert.Multiple(() =>
                        {
                            Assert.That(Sut.PeekNextOpcode(), Is.EqualTo(RET_opcode));
                            Assert.That(Sut.Registers.PC, Is.EqualTo(oldPC));
                        });
                    }
                }
            });

            DoAfterFetch(b =>
            {
                if(b == NOP_opcode)
                {
                    Sut.ReadFromMemory(memoryAddress);
                }
            });

            Sut.Start();

            Assert.That(beforeInvoked);

            var expected =
                //3 instructions of 1 byte each executed...
                executionStates * 3 +
                M1readMemoryStates * 3 +
                //...plus 1 read from memory
                memoryAccessStates;

            AssertExecuted(NOP_opcode, 1);
            AssertExecuted(LD_SP_HL_opcode, 1);
            AssertExecuted(RET_opcode, 1);

            Assert.Multiple(() =>
            {
                Assert.That(Sut.TStatesElapsedSinceReset, Is.EqualTo(expected));
                Assert.That(Sut.TStatesElapsedSinceStart, Is.EqualTo(expected));
            });
        }

        [Test]
        public void PeekNextOpcode_can_be_invoked_only_before_instruction_fetch_complete()
        {
            DoBeforeFetch(b => Sut.FetchNextOpcode());

            DoAfterFetch(b => Assert.Throws<InvalidOperationException>(() => Sut.PeekNextOpcode()));

            Sut.Start();
        }

        [Test]
        public void FetchNextOpcode_after_peek_returns_correct_opcode_and_updates_T_states_appropriately()
        {
            var executionStates = Fixture.Create<byte>();
            var M1readMemoryStates_0 = Fixture.Create<byte>();
            var M1readMemoryStates_1 = Fixture.Create<byte>();
            var M1readMemoryStates_2 = Fixture.Create<byte>();
            var M1readMemoryStates_3 = Fixture.Create<byte>();
            var secondOpcodeByte = Fixture.Create<byte>();

            SetStatesReturner(b => executionStates);

            Sut.Memory[0] = NOP_opcode;
            Sut.Memory[1] = LD_SP_HL_opcode;
            Sut.Memory[2] = secondOpcodeByte;
            Sut.Memory[3] = RET_opcode;

            Sut.SetMemoryWaitStatesForM1(0, 1, M1readMemoryStates_0);
            Sut.SetMemoryWaitStatesForM1(1, 1, M1readMemoryStates_1);
            Sut.SetMemoryWaitStatesForM1(2, 1, M1readMemoryStates_2);
            Sut.SetMemoryWaitStatesForM1(3, 1, M1readMemoryStates_3);

            var beforeInvoked = false;

            DoBeforeFetch(b =>
            {
                if(b == LD_SP_HL_opcode)
                {
                    beforeInvoked = true;
                    Assert.Multiple(() =>
                    {
                        Assert.That(Sut.PeekNextOpcode(), Is.EqualTo(secondOpcodeByte));
                        Assert.That(Sut.Registers.PC, Is.EqualTo(2));
                        Assert.That(Sut.FetchNextOpcode(), Is.EqualTo(secondOpcodeByte));
                    });
                    Assert.That(Sut.Registers.PC, Is.EqualTo(3));
                }
            });

            Sut.Start();

            Assert.That(beforeInvoked);

            var expected =
                executionStates * 3 +
                M1readMemoryStates_0 +
                M1readMemoryStates_1 +
                M1readMemoryStates_2 +
                M1readMemoryStates_3;

            AssertExecuted(NOP_opcode, 1);
            AssertExecuted(LD_SP_HL_opcode, 1);
            AssertExecuted(RET_opcode, 1);

            Assert.Multiple(() =>
            {
                Assert.That(Sut.TStatesElapsedSinceReset, Is.EqualTo(expected));
                Assert.That(Sut.TStatesElapsedSinceStart, Is.EqualTo(expected));
            });
        }

        #endregion
    }
}
