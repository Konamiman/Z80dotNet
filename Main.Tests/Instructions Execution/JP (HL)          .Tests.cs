using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class JP_aHL_tests : InstructionsExecutionTestsBase
    {
        private const byte JP_aHL_opcode = 0xE9;

        [Test]
        public void JP_aHL_jumps_to_address_contained_in_HL()
        {
            var instructionAddress = Fixture.Create<ushort>();
            var jumpAddress = Fixture.Create<short>();

            Registers.HL = jumpAddress;
            ExecuteAt(instructionAddress, JP_aHL_opcode);

            Assert.AreEqual(jumpAddress.ToUShort(), Registers.PC);
        }

        [Test]
        public void JP_aHL_does_not_change_flags()
        {
            AssertNoFlagsAreModified(JP_aHL_opcode);
        }

        [Test]
        public void JP_aHL_returns_proper_T_states()
        {
            var states = Execute(JP_aHL_opcode);
            Assert.AreEqual(4, states);
        }
    }
}