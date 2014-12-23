using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Konamiman.Z80dotNet.Tests
{
    public class Z80ProcessorTests_Interrupts
    {
        private const byte RET_opcode = 0xC9;
        private const byte DI_opcode = 0xF3;
        private const byte EI_opcode = 0xFB;
        private const byte HALT_opcode = 0x76;
        private const byte NOP_opcode = 0x00;

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
            Assert.IsNotNull(Sut);
        }

        #region Interrupt source registration

        [Test]
        public void RegisterInterruptSource_and_GetRegisteredSources_are_simmetrical()
        {
            Sut.RegisterInterruptSource(InterruptSource1);
            Sut.RegisterInterruptSource(InterruptSource2);

            var expected = new[] {InterruptSource1, InterruptSource2};
            var actual = Sut.GetRegisteredInterruptSources();
            CollectionAssert.AreEqual(expected, actual);

        }

        [Test]
        public void RegisterInterruptSource_does_not_register_same_instance_twice()
        {
            Sut.RegisterInterruptSource(InterruptSource1);
            Sut.RegisterInterruptSource(InterruptSource1);

            var expected = new[] {InterruptSource1};
            var actual = Sut.GetRegisteredInterruptSources();
            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void UnregisterAllInterruptSources_clears_sources_list()
        {
            Sut.RegisterInterruptSource(InterruptSource1);
            Sut.RegisterInterruptSource(InterruptSource2);

            Sut.UnregisterAllInterruptSources();

            var actual = Sut.GetRegisteredInterruptSources();
            CollectionAssert.IsEmpty(actual);
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

            Assert.True(serviceRoutineInvoked);
            Assert.True(serviceRoutineReturned);
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

            Assert.True(nmiFired);
            Assert.AreEqual(0, Sut.Registers.IFF1);
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

            Assert.True(nmiFired);
            Assert.False(serviceRoutineInvoked);
        }

        #endregion
    }
}
