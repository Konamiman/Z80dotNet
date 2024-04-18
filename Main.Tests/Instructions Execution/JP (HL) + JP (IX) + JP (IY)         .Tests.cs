using NUnit.Framework;
using AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class JP_aHL_IX_IY_tests : InstructionsExecutionTestsBase
    {
        public static object[] JP_Source =
        {
            new object[] { "HL", (byte)0xE9, (byte?)null },
            new object[] { "IX", (byte)0xE9, (byte?)0xDD },
            new object[] { "IY", (byte)0xE9, (byte?)0xFD }
        };

        [Test]
        [TestCaseSource(nameof(JP_Source))]
        public void JP_aHL_IX_IY_jump_to_address_contained_in_HL(string reg, byte opcode, byte? prefix)
        {
            var instructionAddress = Fixture.Create<ushort>();
            var jumpAddress = Fixture.Create<short>();

            SetReg(reg, jumpAddress);
            ExecuteAt(instructionAddress, opcode, prefix);

            Assert.That(Registers.PC, Is.EqualTo(jumpAddress.ToUShort()));
        }

        [Test]
        [TestCaseSource(nameof(JP_Source))]
        public void JP_aHL_IX_IY_do_not_change_flags(string reg, byte opcode, byte? prefix)
        {
            AssertNoFlagsAreModified(opcode, prefix);
        }

        [Test]
        [TestCaseSource(nameof(JP_Source))]
        public void JP_aHL_IX_IY_return_proper_T_states(string reg, byte opcode, byte? prefix)
        {
            var states = Execute(opcode, prefix);
            Assert.That(states, Is.EqualTo(reg =="HL" ? 4 : 8));
        }
    }
}