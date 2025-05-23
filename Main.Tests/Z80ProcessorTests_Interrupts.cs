﻿using System;
using NUnit.Framework;
using AutoFixture;

namespace Konamiman.Z80dotNet.Tests
{
    public class Z80ProcessorTests_Interrupts
    {
        private const byte RET_opcode = 0xC9;
        private const byte DI_opcode = 0xF3;
        private const byte EI_opcode = 0xFB;
        private const byte HALT_opcode = 0x76;
        private const byte NOP_opcode = 0x00;
        private const byte RST20h_opcode = 0xE7;
        private const byte IM0_opcode = 0x46;
        private const byte IM1_opcode = 0x56;
        private const byte IM2_opcode = 0x5E;

        Z80ProcessorForTests Sut { get; set; }
        Fixture Fixture { get; set; }
        InterruptSourceForTests InterruptSource1 { get; set; }
        InterruptSourceForTests InterruptSource2 { get; set; }

        [SetUp]
        public void Setup()
        {
            Fixture = new Fixture();

            Sut = new Z80ProcessorForTests();
            Sut.SetInstructionExecutionContextToNonNull();

            InterruptSource1 = new InterruptSourceForTests();
            InterruptSource2 = new InterruptSourceForTests();
        }

        [Test]
        public void Can_create_instances()
        {
            Assert.That(Sut, Is.Not.Null);
        }

        #region Interrupt source registration

        [Test]
        public void RegisterInterruptSource_and_GetRegisteredSources_are_simmetrical()
        {
            Sut.RegisterInterruptSource(InterruptSource1);
            Sut.RegisterInterruptSource(InterruptSource2);

            var expected = new[] {InterruptSource1, InterruptSource2};
            var actual = Sut.GetRegisteredInterruptSources();
            Assert.That(actual, Is.EquivalentTo(expected));

        }

        [Test]
        public void RegisterInterruptSource_does_not_register_same_instance_twice()
        {
            Sut.RegisterInterruptSource(InterruptSource1);
            Sut.RegisterInterruptSource(InterruptSource1);

            var expected = new[] {InterruptSource1};
            var actual = Sut.GetRegisteredInterruptSources();
            Assert.That(actual, Is.EquivalentTo(expected));
        }

        [Test]
        public void UnregisterAllInterruptSources_clears_sources_list()
        {
            Sut.RegisterInterruptSource(InterruptSource1);
            Sut.RegisterInterruptSource(InterruptSource2);

            Sut.UnregisterAllInterruptSources();

            var actual = Sut.GetRegisteredInterruptSources();
            Assert.That(actual, Is.Empty);
        }

        #endregion

        #region Accepting NMI interrupts

        [Test]
        public void Nmi_is_accepted_after_instruction_execution()
        {
            Sut.RegisterInterruptSource(InterruptSource1);

            Sut.Memory[0] = NOP_opcode;
            Sut.Memory[1] = RET_opcode;
            Sut.Memory[0x66] = RET_opcode;
            bool nmiFired = false;
            bool serviceRoutineInvoked = false;
            bool serviceRoutineReturned = false;

            Sut.AutoStopOnRetWithStackEmpty = true;

            Sut.BeforeInstructionFetch +=
                (sender, args) =>
                {
                    if(!nmiFired) {
                        InterruptSource1.FireNmi();
                        nmiFired = true;
                    }

                    if(Sut.Registers.PC == 1)
                        serviceRoutineReturned = true;

                    if(Sut.Registers.PC == 0x66)
                        serviceRoutineInvoked = true;
                };

            Sut.Start();

            Assert.That(serviceRoutineInvoked);
            Assert.That(serviceRoutineReturned);
        }

        [Test]
        public void Nmi_resets_IFF1()
        {
            Sut.RegisterInterruptSource(InterruptSource1);

            Sut.Memory[0] = NOP_opcode;
            Sut.Memory[1] = RET_opcode;
            Sut.Memory[0x66] = RET_opcode;
            bool nmiFired = false;

            Sut.Reset();
            Sut.Registers.IFF1 = 1;
            Sut.AutoStopOnRetWithStackEmpty = true;

            Sut.BeforeInstructionFetch +=
                (sender, args) =>
                {
                    if(!nmiFired) {
                        InterruptSource1.FireNmi();
                        nmiFired = true;
                    }
                };

            Sut.Continue();

            Assert.That(nmiFired);
            Assert.That(Sut.Registers.IFF1.Value, Is.EqualTo(0));
        }

        [Test]
        [TestCase(DI_opcode)]
        [TestCase(EI_opcode)]
        public void Nmi_is_not_accepted_after_EI_or_DI(byte opcode)
        {
            Sut.RegisterInterruptSource(InterruptSource1);

            Sut.Memory[0] = opcode;
            Sut.Memory[1] = RET_opcode;
            Sut.Memory[0x66] = RET_opcode;
            bool nmiFired = false;
            bool serviceRoutineInvoked = false;

            Sut.Reset();
            Sut.Registers.IFF1 = 1;
            Sut.AutoStopOnRetWithStackEmpty = true;

            Sut.BeforeInstructionFetch +=
                (sender, args) =>
                {
                    if(!nmiFired) {
                        InterruptSource1.FireNmi();
                        nmiFired = true;
                    }

                    if(Sut.Registers.PC == 0x66)
                        serviceRoutineInvoked = true;

                    if(Sut.Registers.PC == 1)
                        args.ExecutionStopper.Stop();
                };

            Sut.Continue();

            Assert.That(nmiFired);
            Assert.That(serviceRoutineInvoked, Is.False);
        }

        #endregion

        #region Accepting INT interrupts

        [Test]
        public void Int_acceptance_clears_IFF1_and_IFF2()
        {
            Sut.RegisterInterruptSource(InterruptSource1);

            InterruptSource1.IntLineIsActive = true;
            InterruptSource1.ValueOnDataBus = RST20h_opcode;

            Sut.Memory[0] = NOP_opcode;
            Sut.Memory[1] = RET_opcode;

            Sut.Reset();
            Sut.Registers.IFF1 = 1;
            Sut.Registers.IFF1 = 1;
            Sut.AutoStopOnRetWithStackEmpty = true;

            Sut.BeforeInstructionFetch +=
                (sender, args) =>
                {
                    if(Sut.Registers.PC == 1)
                        args.ExecutionStopper.Stop();
                };

            Sut.Continue();

            Assert.Multiple(() =>
            {
                Assert.That(Sut.Registers.IFF1.Value, Is.EqualTo(0));
                Assert.That(Sut.Registers.IFF2.Value, Is.EqualTo(0));
            });
        }

        [Test]
        public void Int_is_not_accepted_with_ints_disabled()
        {
            Sut.RegisterInterruptSource(InterruptSource1);

            InterruptSource1.IntLineIsActive = true;
            InterruptSource1.ValueOnDataBus = RST20h_opcode;

            Sut.Memory[0] = NOP_opcode;
            Sut.Memory[1] = RET_opcode;
            Sut.Memory[0x20] = RET_opcode;
            bool serviceRoutineInvoked = false;

            Sut.Reset();
            Sut.Registers.IFF1 = 0;
            Sut.Registers.IFF2 = 0;
            Sut.InterruptMode = 0;
            Sut.AutoStopOnRetWithStackEmpty = true;

            Sut.BeforeInstructionFetch +=
                (sender, args) =>
                {
                    if(Sut.Registers.PC == 0x20)
                        serviceRoutineInvoked = true;

                    if(Sut.Registers.PC == 1)
                        args.ExecutionStopper.Stop();
                };

            Sut.Continue();

            Assert.That(serviceRoutineInvoked, Is.False);
        }

        [Test]
        public void Int_executes_opcode_from_data_bus_in_IM0_mode()
        {
            Sut.RegisterInterruptSource(InterruptSource1);

            InterruptSource1.IntLineIsActive = true;
            InterruptSource1.ValueOnDataBus = RST20h_opcode;

            Sut.Memory[0] = NOP_opcode;
            Sut.Memory[1] = RET_opcode;
            Sut.Memory[0x20] = RET_opcode;
            bool serviceRoutineInvoked = false;

            Sut.Reset();
            Sut.Registers.IFF1 = 1;
            Sut.InterruptMode = 0;
            Sut.AutoStopOnRetWithStackEmpty = true;

            Sut.BeforeInstructionFetch +=
                (sender, args) =>
                {
                    if(Sut.Registers.PC == 0x20)
                        serviceRoutineInvoked = true;

                    if(Sut.Registers.PC == 1)
                        args.ExecutionStopper.Stop();
                };

            Sut.Continue();

            Assert.That(serviceRoutineInvoked);
        }

        [Test]
        public void Int1_executes_RST38h()
        {
            Sut.RegisterInterruptSource(InterruptSource1);

            InterruptSource1.IntLineIsActive = true;
            InterruptSource1.ValueOnDataBus = RST20h_opcode;

            Sut.Memory[0] = NOP_opcode;
            Sut.Memory[1] = RET_opcode;
            Sut.Memory[0x38] = RET_opcode;
            bool serviceRoutineInvoked = false;

            Sut.Reset();
            Sut.Registers.IFF1 = 1;
            Sut.InterruptMode = 1;
            Sut.AutoStopOnRetWithStackEmpty = true;

            Sut.BeforeInstructionFetch +=
                (sender, args) =>
                {
                    if(Sut.Registers.PC == 0x38)
                        serviceRoutineInvoked = true;

                    if(Sut.Registers.PC == 1)
                        args.ExecutionStopper.Stop();
                };

            Sut.Continue();

            Assert.That(serviceRoutineInvoked);
        }

        [Test]
        public void Int2_calls_to_address_composed_from_I_and_data_bus_and_triggers_memory_events()
        {
            Sut.RegisterInterruptSource(InterruptSource1);

            InterruptSource1.IntLineIsActive = true;

            var registerI = Fixture.Create<byte>();
            var dataBusValue = Fixture.Create<byte>();
            var pointerAddress = NumberUtils.CreateShort(dataBusValue, registerI).ToUShort();
            var calledAddress = Fixture.Create<ushort>();
            Sut.Memory[pointerAddress] = calledAddress.GetLowByte();
            Sut.Memory[pointerAddress.Inc()] = calledAddress.GetHighByte();
            Sut.Registers.I = registerI;
            InterruptSource1.ValueOnDataBus = dataBusValue;

            Sut.Memory[0] = NOP_opcode;
            Sut.Memory[1] = RET_opcode;
            Sut.Memory[calledAddress] = RET_opcode;
            bool serviceRoutineInvoked = false;
            bool beforeMemoryReadEventFiredForPointerAddress = false;
            bool afterMemoryReadEventFiredForPointerAddress = false;

            Sut.Reset();
            Sut.Registers.IFF1 = 1;
            Sut.InterruptMode = 2;
            Sut.AutoStopOnRetWithStackEmpty = true;

            Sut.BeforeInstructionFetch +=
                (sender, args) =>
                {
                    if(Sut.Registers.PC == calledAddress)
                        serviceRoutineInvoked = true;

                    if(Sut.Registers.PC == 1)
                        args.ExecutionStopper.Stop();
                };

            Sut.MemoryAccess +=
                (sender, args) =>
                {
                    if(args.Address != pointerAddress)
                        return;

                    if(args.EventType == MemoryAccessEventType.BeforeMemoryRead)
                        beforeMemoryReadEventFiredForPointerAddress = true;
                    else if(args.EventType == MemoryAccessEventType.AfterMemoryRead)
                        afterMemoryReadEventFiredForPointerAddress = true;
                };

            Sut.Continue();

            Assert.That(serviceRoutineInvoked);
            Assert.That(beforeMemoryReadEventFiredForPointerAddress, "BeforeMemoryRead not fired for pointer");
            Assert.That(afterMemoryReadEventFiredForPointerAddress, "AfterMemoryRead not fired for pointer");
        }

        private void Sut_MemoryAccess(object sender, MemoryAccessEventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Halt behavior

        [Test]
        public void Halt_causes_the_processor_to_execute_NOPs_without_increasing_SP()
        {
            Sut.Memory[0] = NOP_opcode;
            Sut.Memory[1] = HALT_opcode;
            Sut.Memory[2] = RET_opcode;
            Sut.Memory[3] = RET_opcode;

            Sut.AutoStopOnDiPlusHalt = false;

            var maxPCreached = 0;
            var instructionsExecutedCount = 0;
            var nopsExecutedcount = 0;

            Sut.BeforeInstructionFetch +=
                (sender, args) =>
                {
                    maxPCreached = Math.Max(Sut.Registers.PC, maxPCreached);

                    if(instructionsExecutedCount == 5)
                        args.ExecutionStopper.Stop();

                    instructionsExecutedCount++;
                };

            Sut.BeforeInstructionExecution +=
                (sender, args) =>
                {
                    if(args.Opcode[0] == 0)
                        nopsExecutedcount++;
                };

            Sut.Start();

            Assert.That(Sut.IsHalted);
            Assert.Multiple(() =>
            {
                Assert.That(maxPCreached, Is.EqualTo(2));
                Assert.That(nopsExecutedcount, Is.EqualTo(4));
            });
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Halted_processor_awakes_on_interrupt(bool isNmi)
        {
            Sut.RegisterInterruptSource(InterruptSource1);

            Sut.Memory[0] = NOP_opcode;
            Sut.Memory[1] = HALT_opcode;
            Sut.Memory[2] = RET_opcode;
            Sut.Memory[3] = RET_opcode;
            Sut.Memory[0x66] = RET_opcode;
            Sut.Memory[0x38] = RET_opcode;

            Sut.AutoStopOnDiPlusHalt = false;
            Sut.AutoStopOnRetWithStackEmpty = true;
            Sut.Reset();
            Sut.Registers.IFF1 = 1;
            Sut.InterruptMode = 1;

            var instructionsExecutedCount = 0;

            Sut.BeforeInstructionFetch +=
                (sender, args) =>
                {
                    if(instructionsExecutedCount == 10)
                        if(isNmi)
                            InterruptSource1.FireNmi();
                        else
                            InterruptSource1.IntLineIsActive = true;

                    if(instructionsExecutedCount == 15)
                        args.ExecutionStopper.Stop();
                    else
                        instructionsExecutedCount++;
                };

            Sut.Continue();

            Assert.That(Sut.IsHalted, Is.False);
            Assert.Multiple(() =>
            {
                Assert.That(instructionsExecutedCount, Is.EqualTo(13)); //10 + extra NOP + RET on 0x66 + RET on 2
                Assert.That(Sut.StopReason, Is.EqualTo(StopReason.RetWithStackEmpty));
            });
        }

        #endregion

        #region Interrupt servicing start events

        [Test]
        public void Fires_NonMaskableInterruptServicingStart()
        {
            Sut.RegisterInterruptSource(InterruptSource1);

            Sut.Memory[0] = NOP_opcode;

            var nonMaskableInterruptServicingStartFired = false;
            Sut.NonMaskableInterruptServicingStart += (server, args) =>
            {
                nonMaskableInterruptServicingStartFired = true;
            };

            InterruptSource1.FireNmi();

            Sut.ExecuteNextInstruction();

            Assert.That(nonMaskableInterruptServicingStartFired, Is.True);
        }

        [Test]
        [TestCase(IM0_opcode)]
        [TestCase(IM1_opcode)]
        [TestCase(IM2_opcode)]
        public void Fires_MaskableInterruptServicingStart(byte imOpcode)
        {
            Sut.RegisterInterruptSource(InterruptSource1);

            Sut.Memory[0] = 0xED;
            Sut.Memory[1] = imOpcode;
            Sut.Memory[2] = EI_opcode;
            Sut.Memory[3] = NOP_opcode;

            Sut.ExecuteNextInstruction(); //This sets interrupt mode
            Sut.ExecuteNextInstruction(); //This runs EI

            var maskableInterruptServicingStartFired = false;
            Sut.MaskableInterruptServicingStart += (server, args) =>
            {
                maskableInterruptServicingStartFired = true;
            };

            InterruptSource1.IntLineIsActive = true;

            Sut.ExecuteNextInstruction();

            Assert.That(maskableInterruptServicingStartFired, Is.True);
        }

        #endregion
    }
}
