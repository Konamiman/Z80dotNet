using NUnit.Framework;
using AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class EX_DE_HL_tests : InstructionsExecutionTestsBase
    {
        private const byte EX_DE_HL_opcode = 0xEB;

        [Test]
        public void EX_DE_HL_exchanges_the_AF_registers()
        {
            var DE = Fixture.Create<short>();
            var HL = Fixture.Create<short>();

            Registers.DE = DE;
            Registers.HL = HL;

            Execute(EX_DE_HL_opcode);

            Assert.Multiple(() =>
            {
                Assert.That(Registers.DE, Is.EqualTo(HL));
                Assert.That(Registers.HL, Is.EqualTo(DE));
            });
        }

        [Test]
        public void EX_DE_HL_does_not_change_flags()
        {
            AssertNoFlagsAreModified(EX_DE_HL_opcode);
        }

        [Test]
        public void EX_DE_HL_returns_proper_T_states()
        {
            var states = Execute(EX_DE_HL_opcode);
            Assert.That(states, Is.EqualTo(4));
        }
    }

}