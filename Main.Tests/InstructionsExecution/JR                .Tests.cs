using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class JR_tests : InstructionsExecutionTestsBase
    {
        private const byte JR_opcode = 0x18;

        [Test]
        public void JR_jumps_to_proper_address()
        {
            var instructionAddress = Fixture.Create<ushort>();

            ExecuteAt(instructionAddress, JR_opcode, nextFetches: new byte[] {0x7F});
            Assert.AreEqual(instructionAddress.Add(129), Registers.PC);

            ExecuteAt(instructionAddress, JR_opcode, nextFetches: new byte[] {0x80});
            Assert.AreEqual(instructionAddress.Sub(126), Registers.PC);
        }

        [Test]
        public void JR_returns_proper_T_states()
        {
            var states = Execute(JR_opcode);

            Assert.AreEqual(12, states);
        }

        [Test]
        public void JR_does_not_modify_flags()
        {
            var value = Fixture.Create<byte>();
            Registers.F = value;
            Execute(JR_opcode);

            Assert.AreEqual(value, Registers.F);
        }
    }
}