using NUnit.Framework;
using AutoFixture;
using System;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class Z80InstructionsExecutor_core_test : InstructionsExecutionTestsBase
    {
        private const byte NOP_opcode = 0x00;
        private const byte LD_BC_nn_opcode = 0x01;
        private const byte ADD_HL_BC_opcode = 0x09;
        private const byte IN_B_C_opcode = 0x40;
        private const byte RLC_B_opcode = 0x00;

		[Test]
		public void InstructionsExecution_fire_FetchFinished_event_and_return_proper_T_states_count()
		{
		    var fetchFinishedEventsCount = 0;

		    Sut.InstructionFetchFinished += (sender, e) => fetchFinishedEventsCount++;

            SetMemoryContents(0);
            Assert.That(Execute(NOP_opcode), Is.EqualTo(4));
		    SetMemoryContents(0, Fixture.Create<byte>(), Fixture.Create<byte>());
            Assert.That(Execute(LD_BC_nn_opcode), Is.EqualTo(10));

            SetMemoryContents(0);
            Assert.Multiple(() =>
            {
                Assert.That(Execute(0xCB, nextFetches: 0), Is.EqualTo(8));

                Assert.That(Execute(ADD_HL_BC_opcode, 0xDD), Is.EqualTo(15));
                Assert.That(Execute(ADD_HL_BC_opcode, 0xFD), Is.EqualTo(15));

                Assert.That(Execute(IN_B_C_opcode, 0xED), Is.EqualTo(12));

                Assert.That(Execute(0xCB, 0xDD, 0), Is.EqualTo(23));
                Assert.That(Execute(0xCB, 0xFD, 0), Is.EqualTo(23));

                Assert.That(fetchFinishedEventsCount, Is.EqualTo(8));
            });
        }

        [Test]
        public void Unsupported_instructions_just_return_8_TStates_elapsed()
        {
			var fetchFinishedEventsCount = 0;

			Sut.InstructionFetchFinished += (sender, e) => fetchFinishedEventsCount++;

            Assert.Multiple(() =>
            {
                Assert.That(Execute(0x3F, 0xED), Is.EqualTo(8));
                Assert.That(Execute(0xC0, 0xED), Is.EqualTo(8));
                Assert.That(Execute(0x80, 0xED), Is.EqualTo(8));
                Assert.That(Execute(0x9F, 0xED), Is.EqualTo(8));

                Assert.That(fetchFinishedEventsCount, Is.EqualTo(4));
            });
        }

		[Test]
        public void Unsupported_instructions_invoke_overridable_method_ExecuteUnsopported_ED_Instruction()
		{
		    Sut = NewFakeInstructionExecutor();

		    Execute(0x3F, 0xED);
		    Execute(0xC0, 0xED);

            Assert.That(((FakeInstructionExecutor)Sut).UnsupportedExecuted, Is.EqualTo(new Byte[] {0x3F, 0xC0}));
        }

        [Test]
		public void Execute_increases_R_appropriately()
        {
			Registers.R = 0xFE;

			Execute(NOP_opcode);
            Assert.That(Registers.R, Is.EqualTo(0xFF));

            Execute(LD_BC_nn_opcode, null, Fixture.Create<byte>(), Fixture.Create<byte>());
            Assert.That(Registers.R, Is.EqualTo(0x80));

			Execute(RLC_B_opcode, 0xCB);
            Assert.That(Registers.R, Is.EqualTo(0x82));

            Execute(ADD_HL_BC_opcode, 0xDD);
            Assert.That(Registers.R, Is.EqualTo(0x84));

            Execute(ADD_HL_BC_opcode, 0xFD);
            Assert.That(Registers.R, Is.EqualTo(0x86));

			Execute(IN_B_C_opcode, 0xED);
            Assert.That(Registers.R, Is.EqualTo(0x88));

            Execute(0xCB, 0xDD, 0);
            Assert.That(Registers.R, Is.EqualTo(0x8A));

            Execute(0xCB, 0xFD, 0);
            Assert.That(Registers.R, Is.EqualTo(0x8C));
        }

        [Test]
        public void DD_FD_not_followed_by_valid_opcode_are_trated_as_nops()
        {
            var fetchFinishedEventsCount = 0;

		    Sut.InstructionFetchFinished += (sender, e) => fetchFinishedEventsCount++;

            Assert.Multiple(() =>
            {
                Assert.That(Execute(0xFD, 0xDD), Is.EqualTo(4));
                Assert.That(Execute(0x01, 0xFD), Is.EqualTo(4));
                Assert.That(Execute(0x01, null, Fixture.Create<byte>(), Fixture.Create<byte>()), Is.EqualTo(10));

                Assert.That(fetchFinishedEventsCount, Is.EqualTo(3));
            });
        }
    }
}
