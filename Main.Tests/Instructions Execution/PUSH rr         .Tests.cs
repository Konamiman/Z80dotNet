using NUnit.Framework;
using AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class PUSH_rr_tests : InstructionsExecutionTestsBase
    {
        public static object[] PUSH_rr_Source =
        {
            new object[] {"BC", (byte)0xC5, (byte?)null},
            new object[] {"DE", (byte)0xD5, (byte?)null},
            new object[] {"HL", (byte)0xE5, (byte?)null},
            new object[] {"AF", (byte)0xF5, (byte?)null},
            new object[] {"IX", (byte)0xE5, (byte?)0xDD},
            new object[] {"IY", (byte)0xE5, (byte?)0xFD},
        };

        [Test]
        [TestCaseSource(nameof(PUSH_rr_Source))]
        public void PUSH_rr_loads_stack_with_value_and_decreases_SP(string reg, byte opcode, byte? prefix)
        {
            var value = Fixture.Create<short>();
            SetReg(reg, value);
            var oldSP = Fixture.Create<short>();
            Registers.SP = oldSP;
            
            Execute(opcode, prefix);

            Assert.Multiple(() =>
            {
                Assert.That(Registers.SP, Is.EqualTo(oldSP.Sub(2)));
                Assert.That(ReadShortFromMemory(Registers.SP.ToUShort()), Is.EqualTo(value));
            });
        }

        [Test]
        [TestCaseSource(nameof(PUSH_rr_Source))]
        public void PUSH_rr_do_not_modify_flags(string reg, byte opcode, byte? prefix)
        {
            AssertNoFlagsAreModified(opcode);
        }

        [Test]
        [TestCaseSource(nameof(PUSH_rr_Source))]
        public void PUSH_rr_returns_proper_T_states(string reg, byte opcode, byte? prefix)
        {
            var states = Execute(opcode, prefix);
            Assert.That(states, Is.EqualTo(reg.StartsWith("I") ? 15 : 11));
        }
    }
}
