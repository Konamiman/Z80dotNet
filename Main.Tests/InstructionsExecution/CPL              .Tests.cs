using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class CPL_tests : InstructionsExecutionTestsBase
    {
        private const byte CPL_opcode = 0x2F;

        [Test]
        public void CPL_complements_byte_correctly()
        {
            Registers.A = 0x99;

            Execute(CPL_opcode);

            Assert.AreEqual(0x66, Registers.A);
        }

        [Test]
        public void CPL_sets_H_and_N()
        {
            AssertSetsFlags(CPL_opcode, null, "H", "N");
        }

        [Test]
        public void CPL_does_not_change_SF_ZF_PF_CF()
        {
            AssertDoesNotChangeFlags(CPL_opcode, null, "S", "Z", "P", "C");
        }

        [Test]
        public void CPL_returns_proper_T_states()
        {
            var states = Execute(CPL_opcode);
            Assert.AreEqual(4, states);
        }
    }
}