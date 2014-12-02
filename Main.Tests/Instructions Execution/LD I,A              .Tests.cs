using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class LD_I_A_tests : InstructionsExecutionTestsBase
    {
        private const byte opcode = 0x47;
        private const byte prefix = 0xED;

        [Test]
        public void LD_I_A_loads_value_correctly()
        {
            var oldValue = Fixture.Create<byte>();
            var newValue = Fixture.Create<byte>();
            Registers.I = oldValue;
            Registers.A = newValue;

            Execute(opcode, prefix);

            Assert.AreEqual(newValue, Registers.I);
        }

        [Test]
        public void LD_I_A_does_not_modify_flags()
        {
            AssertDoesNotChangeFlags(opcode, prefix);
        }

        [Test]
        public void LD_I_A_returns_proper_T_states()
        {
            var states = Execute(opcode, prefix);
            Assert.AreEqual(9, states);
        }
    }
}