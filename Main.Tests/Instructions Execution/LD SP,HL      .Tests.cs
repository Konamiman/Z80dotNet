using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class LD_SP_HL_tests : InstructionsExecutionTestsBase
    {
        private const byte LD_SP_HL_opcode = 0xF9;

        [Test]
        public void LD_SP_HL_loads_SP_correctly()
        {
            var HL = Fixture.Create<short>();
            var oldSP = Fixture.Create<short>();

            Registers.HL = HL;
            Registers.SP = oldSP;

            Execute(LD_SP_HL_opcode);

            Assert.AreEqual(HL, Registers.HL);
            Assert.AreEqual(HL, Registers.SP);
        }

        [Test]
        public void LD_SP_HL_fires_FetchFinished_with_isLdSp_true()
        {
            var eventFired = false;

            Sut.InstructionFetchFinished += (sender, e) =>
            {
                eventFired = true;
                Assert.True(e.IsLdSpInstruction);
            };

            Execute(LD_SP_HL_opcode);

            Assert.IsTrue(eventFired);
        }

        [Test]
        public void LD_SP_HL_does_not_change_flags()
        {
            AssertNoFlagsAreModified(LD_SP_HL_opcode);
        }

        [Test]
        public void LD_SP_HL_returns_proper_T_states()
        {
            var states = Execute(LD_SP_HL_opcode);
            Assert.AreEqual(6, states);
        }
    }
}