using System.Linq;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class PUSH_rr_tests : InstructionsExecutionTestsBase
    {
        public static object[] PUSH_rr_Source =
        {
            new object[] {"BC", (byte)0xC5},
            new object[] {"DE", (byte)0xD5},
            new object[] {"HL", (byte)0xE5},
            new object[] {"AF", (byte)0xF5},
        };

        [Test]
        [TestCaseSource("PUSH_rr_Source")]
        public void PUSH_rr_loads_stack_with_value_and_decreases_SP(string reg, byte opcode)
        {
            var value = Fixture.Create<short>();
            SetReg(reg, value);
            var oldSP = Fixture.Create<short>();
            Registers.SP = oldSP;
            
            Execute(opcode);

            Assert.AreEqual(oldSP.Sub(2), Registers.SP);
            Assert.AreEqual(value, ReadShortFromMemory(Registers.SP.ToUShort()));
        }

        [Test]
        [TestCaseSource("PUSH_rr_Source")]
        public void PUSH_rr_do_not_modify_flags(string reg, byte opcode)
        {
            AssertNoFlagsAreModified(opcode);
        }

        [Test]
        [TestCaseSource("PUSH_rr_Source")]
        public void PUSH_rr_returns_proper_T_states(string reg, byte opcode)
        {
            var states = Execute(opcode);
            Assert.AreEqual(11, states);
        }
    }
}
