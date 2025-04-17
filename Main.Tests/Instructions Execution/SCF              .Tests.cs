using NUnit.Framework;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class SCF_tests : InstructionsExecutionTestsBase
    {
        private const byte SCF_opcode = 0x37;

        [Test]
        public void SCF_sets_CF_correctly()
        {
            Registers.CF = 0;

            Execute(SCF_opcode);

            Assert.That(Registers.CF.Value, Is.EqualTo(1));
        }

        [Test]
        public void SCF_resets_H_and_N()
        {
            AssertResetsFlags(SCF_opcode, null, "H", "N");
        }

        [Test]
        public void SCF_does_not_change_SF_ZF_PF()
        {
            AssertDoesNotChangeFlags(SCF_opcode, null, "S", "Z", "P");
        }

        [Test]
        public void SCF_sets_bits_3_and_5_from_A()
        {
            Registers.A = Registers.A.WithBit(3, 1);
            Registers.A = Registers.A.WithBit(5, 0);
            Execute(SCF_opcode);
            Assert.Multiple(() =>
            {
                Assert.That(Registers.Flag3.Value, Is.EqualTo(1));
                Assert.That(Registers.Flag5.Value, Is.EqualTo(0));
            });

            Registers.A = Registers.A.WithBit(3, 0);
            Registers.A = Registers.A.WithBit(5, 1);
            Execute(SCF_opcode);
            Assert.Multiple(() =>
            {
                Assert.That(Registers.Flag3.Value, Is.EqualTo(0));
                Assert.That(Registers.Flag5.Value, Is.EqualTo(1));
            });
        }

        [Test]
        [TestCase(0x00)]
        [TestCase(0x0F)]
        [TestCase(0xF0)]
        [TestCase(0xFF)]
        public void SCF_sets_bits_3_and_5_from_A(int value)
        {
            Registers.A = (byte)value;
            Execute(SCF_opcode);
            Assert.Multiple(() =>
            {
                Assert.That(Registers.Flag3, Is.EqualTo(Registers.A.GetBit(3)));
                Assert.That(Registers.Flag5, Is.EqualTo(Registers.A.GetBit(5)));
            });
        }

        [Test]
        public void SCF_returns_proper_T_states()
        {
            var states = Execute(SCF_opcode);
            Assert.That(states, Is.EqualTo(4));
        }
    }
}