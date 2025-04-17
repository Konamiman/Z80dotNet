using NUnit.Framework;
using AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class BIT_tests : InstructionsExecutionTestsBase
    {
        static BIT_tests()
        {
            BIT_Source = GetBitInstructionsSource(0x40, includeLoadReg: false, loopSevenBits: true);
        }

        public static object[] BIT_Source;
        
        private byte offset;

        [SetUp]
        public void Setup()
        {
            offset = Fixture.Create<byte>();
        }

        [Test]
        [TestCaseSource(nameof(BIT_Source))]
        public void BIT_gets_bit_correctly(string reg, string destReg, byte opcode, byte? prefix, int bit)
        {
            var value = ((byte)0).WithBit(bit, 1);
            SetupRegOrMem(reg, value, offset);
            ExecuteBit(opcode, prefix, offset);
            Assert.That(Registers.ZF.Value, Is.EqualTo(0));

            value = ((byte)0xFF).WithBit(bit, 0);
            SetupRegOrMem(reg, value, offset);
            ExecuteBit(opcode, prefix, offset);
            Assert.That(Registers.ZF.Value, Is.EqualTo(1));
        }

        [Test]
        [TestCaseSource(nameof(BIT_Source))]
        public void BIT_sets_PF_as_ZF(string reg, string destReg, byte opcode, byte? prefix, int bit)
        {
            for(int i = 0; i < 256; i++) {
                SetupRegOrMem(reg, (byte)i, offset);
                ExecuteBit(opcode, prefix, offset);
                Assert.That(Registers.PF, Is.EqualTo(Registers.ZF));
            }
        }

        [Test]
        [TestCaseSource(nameof(BIT_Source))]
        public void BIT_sets_SF_if_bit_is_7_and_is_set(string reg, string destReg, byte opcode, byte? prefix, int bit)
        {
            for(int i = 0; i < 256; i++) {
                var b = (byte)i;
                SetupRegOrMem(reg, b, offset);
                ExecuteBit(opcode, prefix, offset);
                var expected = (bit == 7 && b.GetBit(7) == 1);
                Assert.That((bool) Registers.SF, Is.EqualTo(expected));
            }
        }

        [Test]
        [TestCaseSource(nameof(BIT_Source))]
        public void BIT_resets_N(string reg, string destReg, byte opcode, byte? prefix, int bit)
        {
            AssertResetsFlags(() => ExecuteBit(opcode, prefix, offset), opcode, prefix, "N");
        }

        [Test]
        [TestCaseSource(nameof(BIT_Source))]
        public void BIT_sets_H(string reg, string destReg, byte opcode, byte? prefix, int bit)
        {
            AssertSetsFlags(() => ExecuteBit(opcode, prefix, offset), opcode, prefix, "H");
        }

        [Test]
        [TestCaseSource(nameof(BIT_Source))]
        public void BIT_does_not_modify_CF(string reg, string destReg, byte opcode, byte? prefix, int bit)
        {
            AssertDoesNotChangeFlags(() => ExecuteBit(opcode, prefix, offset), opcode, prefix, "C");
        }

        [Test]
        [TestCaseSource(nameof(BIT_Source))]
        public void BIT_returns_proper_T_states(string reg, string destReg, byte opcode, byte? prefix, int bit)
        {
            var states = ExecuteBit(opcode, prefix, offset);
            Assert.That(states, Is.EqualTo(reg == "(HL)" ? 12 : reg.StartsWith("(") ? 20 : 8));
        }
    }
}