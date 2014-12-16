using NUnit.Framework;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class BIT_tests : InstructionsExecutionTestsBase
    {
        private const byte prefix = 0xCB;

        static BIT_tests()
        {
            BIT_Source = GetBitInstructionsSource(0x40);
        }

        public static object[] BIT_Source;
        
        [Test]
        [TestCaseSource("BIT_Source")]
        public void BIT_gets_bit_correctly(string reg, int bit, byte opcode)
        {
            var value = ((byte)0).WithBit(bit, 1);
            SetupRegOrMem(reg, value);
            Execute(opcode, prefix);
            Assert.AreEqual(0, Registers.ZF);

            value = ((byte)0xFF).WithBit(bit, 0);
            SetupRegOrMem(reg, value);
            Execute(opcode, prefix);
            Assert.AreEqual(1, Registers.ZF);
        }

        [Test]
        [TestCaseSource("BIT_Source")]
        public void BIT_sets_PF_as_ZF(string reg, int bit, byte opcode)
        {
            for(int i = 0; i < 256; i++) {
                SetupRegOrMem(reg, (byte)i);
                Execute(opcode, prefix);
                Assert.AreEqual(Registers.ZF, Registers.PF);
            }
        }

        [Test]
        [TestCaseSource("BIT_Source")]
        public void BIT_sets_SF_if_bit_is_7_and_is_set(string reg, int bit, byte opcode)
        {
            for(int i = 0; i < 256; i++) {
                var b = (byte)i;
                SetupRegOrMem(reg, b);
                Execute(opcode, prefix);
                var expected = (bit == 7 && b.GetBit(7) == 1);
                Assert.AreEqual(expected, (bool) Registers.SF);
            }
        }

        [Test]
        [TestCaseSource("BIT_Source")]
        public void BIT_resets_N(string reg, int bit, byte opcode)
        {
            AssertResetsFlags(opcode, prefix, "N");
        }

        [Test]
        [TestCaseSource("BIT_Source")]
        public void BIT_sets_H(string reg, int bit, byte opcode)
        {
            AssertSetsFlags(opcode, prefix, "H");
        }

        [Test]
        [TestCaseSource("BIT_Source")]
        public void BIT_does_not_modify_CF(string reg, int bit, byte opcode)
        {
            AssertDoesNotChangeFlags(opcode, prefix, "C");
        }

        [Test]
        [TestCaseSource("BIT_Source")]
        public void BIT_returns_proper_T_states(string reg, int bit, byte opcode)
        {
            var states = Execute(opcode, prefix);
            Assert.AreEqual(reg == "(HL)" ? 12 : 8, states);
        }
    }
}