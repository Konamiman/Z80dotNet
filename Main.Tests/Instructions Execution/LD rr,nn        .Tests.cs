using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class LD_rr_nn_tests : InstructionsExecutionTestsBase
    {
        public static object[] LD_rr_nn_Source =
        {
            new object[] {"BC", (byte)0x01},
            new object[] {"DE", (byte)0x11},
            new object[] {"HL", (byte)0x21},
            new object[] {"SP", (byte)0x31},
        };

        [Test]
        [TestCaseSource("LD_rr_nn_Source")]
        public void LD_rr_nn_loads_register_with_value(string reg, byte opcode)
        {
            var oldValue = Fixture.Create<short>();
            var newValue = Fixture.Create<short>();

            SetReg(reg, oldValue);

            SetMemoryContents(newValue.GetLowByte(), newValue.GetHighByte());
            Sut.Execute(opcode);

            Assert.AreEqual(newValue, GetReg<short>(reg));
        }

        [Test]
        [TestCaseSource("LD_rr_nn_Source")]
        public void LD_SP_nn_fires_FetchFinished_with_isLdSp_true(string reg, byte opcode)
        {
            var eventFired = false;

            Sut.InstructionFetchFinished += (sender, e) =>
            {
                eventFired = true;
                Assert.True((reg=="SP" && e.IsLdSpInstruction) | (reg != "SP" && !e.IsLdSpInstruction));
            };

            Execute(opcode);

            Assert.IsTrue(eventFired);
        }

        [Test]
        [TestCaseSource("LD_rr_nn_Source")]
        public void LD_rr_nn_do_not_modify_flags(string reg, byte opcode)
        {
            AssertNoFlagsAreModified(opcode);
        }

        [Test]
        [TestCaseSource("LD_rr_nn_Source")]
        public void LD_rr_nn_returns_proper_T_states(string reg, byte opcode)
        {
            var states = Execute(opcode);
            Assert.AreEqual(10, states);
        }
    }
}
