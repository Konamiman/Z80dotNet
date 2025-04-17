using NUnit.Framework;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class RRCA_tests : InstructionsExecutionTestsBase
    {
        private const byte RRCA_opcode = 0x0F;

        [Test]
        public void RRCA_rotates_byte_correctly()
        {
            var values = new byte[] { 0x82, 0x41, 0xA0, 0x50, 0x28, 0x14, 0x0A, 0x05 };
            Registers.A = 0x05;

            for(var i = 0; i < values.Length; i++)
            {
                Execute(RRCA_opcode);
                Assert.That(Registers.A, Is.EqualTo(values[i]));
            }
        }

        [Test]
        public void RRCA_sets_CF_correctly()
        {
            Registers.A = 0x06;

            Execute(RRCA_opcode);
            Assert.That(Registers.CF.Value, Is.EqualTo(0));

            Execute(RRCA_opcode);
            Assert.That(Registers.CF.Value, Is.EqualTo(1));

            Execute(RRCA_opcode);
            Assert.That(Registers.CF.Value, Is.EqualTo(1));

            Execute(RRCA_opcode);
            Assert.That(Registers.CF.Value, Is.EqualTo(0));
        }

        [Test]
        public void RRCA_resets_H_and_N()
        {
            AssertResetsFlags(RRCA_opcode, null, "H", "N");
        }

        [Test]
        public void RRCA_does_not_change_SF_ZF_PF()
        {
            AssertDoesNotChangeFlags(RRCA_opcode, null, "S", "Z", "P");
        }

        [Test]
        [TestCase(0x00)]
        [TestCase(0xD7)]
        [TestCase(0x28)]
        [TestCase(0xFF)]
        public void RRCA_sets_bits_3_and_5_from_A(int value)
        {
            Registers.A = (byte)value;
            Execute(RRCA_opcode);
            Assert.Multiple(() =>
            {
                Assert.That(Registers.Flag3, Is.EqualTo(Registers.A.GetBit(3)));
                Assert.That(Registers.Flag5, Is.EqualTo(Registers.A.GetBit(5)));
            });
        }

        [Test]
        public void RRCA_returns_proper_T_states()
        {
            var states = Execute(RRCA_opcode);
            Assert.That(states, Is.EqualTo(4));
        }
    }
}