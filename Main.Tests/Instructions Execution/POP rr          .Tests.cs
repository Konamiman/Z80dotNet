using NUnit.Framework;
using AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class POP_rr_tests : InstructionsExecutionTestsBase
    {
        public static object[] POP_rr_Source =
        {
            new object[] {"BC", (byte)0xC1, (byte?)null},
            new object[] {"DE", (byte)0xD1, (byte?)null},
            new object[] {"HL", (byte)0xE1, (byte?)null},
            new object[] {"AF", (byte)0xF1, (byte?)null},
            new object[] {"IX", (byte)0xE1, (byte?)0xDD},
            new object[] {"IY", (byte)0xE1, (byte?)0xFD},
        };

        [Test]
        [TestCaseSource(nameof(POP_rr_Source))]
        public void POP_rr_loads_register_with_value_and_increases_SP(string reg, byte opcode, byte? prefix)
        {
            var instructionAddress = Fixture.Create<ushort>();
            var value = Fixture.Create<ushort>();
            var oldSP = Fixture.Create<short>();

            Registers.SP = oldSP;
            SetMemoryContentsAt(oldSP.ToUShort(), value.GetLowByte());
            SetMemoryContentsAt(oldSP.ToUShort().Inc(), value.GetHighByte());

            ExecuteAt(instructionAddress, opcode, prefix);

            Assert.Multiple(() =>
            {
                Assert.That(GetReg<short>(reg), Is.EqualTo(value));
                Assert.That(Registers.SP, Is.EqualTo(oldSP.Add(2)));
            });
        }

        [Test]
        [TestCaseSource(nameof(POP_rr_Source))]
        public void POP_rr_do_not_modify_flags_unless_AF_is_popped(string reg, byte opcode, byte? prefix)
        {
            if(reg!="AF")
                AssertNoFlagsAreModified(opcode, prefix);
        }

        [Test]
        [TestCaseSource(nameof(POP_rr_Source))]
        public void POP_rr_returns_proper_T_states(string reg, byte opcode, byte? prefix)
        {
            var states = Execute(opcode, prefix);
            Assert.That(states, Is.EqualTo(reg.StartsWith("I") ? 14 : 10));
        }
    }
}
