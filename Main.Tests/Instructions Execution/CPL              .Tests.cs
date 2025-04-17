using NUnit.Framework;
using AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class CPL_tests : InstructionsExecutionTestsBase
    {
        private const byte CPL_opcode = 0x2F;

        [Test]
        public void CPL_complements_byte_correctly()
        {
            var value = Fixture.Create<byte>();
            Registers.A = value;

            Execute(CPL_opcode);

            Assert.That(Registers.A, Is.EqualTo((byte)(~value)));
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
        [TestCase(0x00)]
        [TestCase(0x0F)]
        [TestCase(0xF0)]
        [TestCase(0xFF)]
        public void CPL_sets_bits_3_and_5_from_A(int value)
        {
            Registers.A = (byte)value;
            Execute(CPL_opcode);
            Assert.Multiple(() =>
            {
                Assert.That(Registers.Flag3, Is.EqualTo(Registers.A.GetBit(3)));
                Assert.That(Registers.Flag5, Is.EqualTo(Registers.A.GetBit(5)));
            });
        }

        [Test]
        public void CPL_returns_proper_T_states()
        {
            var states = Execute(CPL_opcode);
            Assert.That(states, Is.EqualTo(4));
        }
    }
}