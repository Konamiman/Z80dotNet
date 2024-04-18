using NUnit.Framework;
using AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class DJNZ_tests : InstructionsExecutionTestsBase
    {
        private const byte DJNZ_opcode = 0x10;

        [Test]
        public void DJNZ_decreases_B()
        {
            var value = Fixture.Create<byte>();
            Registers.B = value;

            Execute(DJNZ_opcode);

            Assert.That(Registers.B, Is.EqualTo(value.Dec()));
        }

        [Test]
        public void DJNZ_does_not_jump_if_B_decreases_to_zero()
        {
            var instructionAddress = Fixture.Create<ushort>();

            Registers.B = 1;
            ExecuteAt(instructionAddress, DJNZ_opcode, nextFetches: new[] {Fixture.Create<byte>()});

            Assert.That(Registers.PC, Is.EqualTo(instructionAddress + 2));
        }

        [Test]
        public void DJNZ_returns_proper_T_states_when_no_jump_is_done()
        {
            Registers.B = 1;
            var states = Execute(DJNZ_opcode);

            Assert.That(states, Is.EqualTo(8));
        }

        [Test]
        public void DJNZ_jumps_to_proper_address_if_B_does_not_decrease_to_zero()
        {
            var instructionAddress = Fixture.Create<ushort>();

            Registers.B = 0;
            ExecuteAt(instructionAddress, DJNZ_opcode, nextFetches: new byte[] {0x7F});
            Assert.That(Registers.PC, Is.EqualTo(instructionAddress.Add(129)));

            Registers.B = 0;
            ExecuteAt(instructionAddress, DJNZ_opcode, nextFetches: new byte[] {0x80});
            Assert.That(Registers.PC, Is.EqualTo(instructionAddress.Sub(126)));
        }

        [Test]
        public void DJNZ_returns_proper_T_states_when_jump_is_done()
        {
            Registers.B = 0;
            var states = Execute(DJNZ_opcode);

            Assert.That(states, Is.EqualTo(13));
        }

        [Test]
        public void DJNZ_does_not_modify_flags()
        {
            AssertNoFlagsAreModified(DJNZ_opcode);
        }
    }
}