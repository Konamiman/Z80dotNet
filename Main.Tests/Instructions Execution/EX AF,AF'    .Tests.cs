using NUnit.Framework;
using AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class EX_AF_AF_tests : InstructionsExecutionTestsBase
    {
        private const byte EX_AF_AF_opcode = 0x08;

        [Test]
        public void EX_AF_AF_exchanges_the_AF_registers()
        {
            var mainValue = Fixture.Create<short>();
            var alternateValue = Fixture.Create<short>();

            Registers.AF = mainValue;
            Registers.Alternate.AF = alternateValue;

            Execute(EX_AF_AF_opcode);

            Assert.Multiple(() =>
            {
                Assert.That(Registers.AF, Is.EqualTo(alternateValue));
                Assert.That(Registers.Alternate.AF, Is.EqualTo(mainValue));
            });
        }

        [Test]
        public void EX_AF_AF_returns_proper_T_states()
        {
            var states = Execute(EX_AF_AF_opcode);
            Assert.That(states, Is.EqualTo(4));
        }
    }

}