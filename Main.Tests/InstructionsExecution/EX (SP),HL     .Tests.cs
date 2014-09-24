using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class EX_SP_HL_tests : InstructionsExecutionTestsBase
    {
        private const byte EX_SP_HL_opcode = 0xE3;

        [Test]
        public void EX_SP_HL_exchanges_HL_and_pushed_value()
        {
            var HL = Fixture.Create<short>();
            var pushedValue = Fixture.Create<short>();
            var SP = Fixture.Create<ushort>();

            Registers.HL = HL;
            Registers.SP = SP.ToShort();
            WriteShortToMemory(SP, pushedValue);

            Execute(EX_SP_HL_opcode);

            Assert.AreEqual(HL, ReadShortFromMemory(SP));
            Assert.AreEqual(pushedValue, Registers.HL);
        }

        [Test]
        public void EX_SP_HL_do_not_change_flags()
        {
            AssertNoFlagsAreModified(EX_SP_HL_opcode);
        }

        [Test]
        public void EX_SP_HL_return_proper_T_states()
        {
            var states = Execute(EX_SP_HL_opcode);
            Assert.AreEqual(19, states);
        }
    }
}