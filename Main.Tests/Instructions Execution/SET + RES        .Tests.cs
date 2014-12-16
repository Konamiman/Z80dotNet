using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class SET_tests : InstructionsExecutionTestsBase
    {
        private const byte prefix = 0xCB;

        static SET_tests()
        {
            SET_Source = GetBitInstructionsSource(0xC0);
            RES_Source = GetBitInstructionsSource(0x80);
        }

        public static object[] SET_Source;
        public static object[] RES_Source;
        
        [Test]
        [TestCaseSource("SET_Source")]
        public void SET_sets_bit_correctly(string reg, int bit, byte opcode)
        {
            var value = Fixture.Create<byte>().WithBit(bit, 0);
            SetupRegOrMem(reg, value);
            Execute(opcode, prefix);
            var expected = value.WithBit(bit, 1);
            var actual = ValueOfRegOrMem(reg);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [TestCaseSource("RES_Source")]
        public void RES_resets_bit_correctly(string reg, int bit, byte opcode)
        {
            var value = Fixture.Create<byte>().WithBit(bit, 1);
            SetupRegOrMem(reg, value);
            Execute(opcode, prefix);
            var expected = value.WithBit(bit, 0);
            var actual = ValueOfRegOrMem(reg);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [TestCaseSource("SET_Source")]
        [TestCaseSource("RES_Source")]
        public void SET_RES_do_not_change_flags(string reg, int bit, byte opcode)
        {
            AssertDoesNotChangeFlags(opcode, prefix);
        }

        [Test]
        [TestCaseSource("SET_Source")]
        [TestCaseSource("RES_Source")]
        public void SET_RES_return_proper_T_states(string reg, int bit, byte opcode)
        {
            var states = Execute(opcode, prefix);
            Assert.AreEqual(reg == "(HL)" ? 15 : 8, states);
        }
    }
}