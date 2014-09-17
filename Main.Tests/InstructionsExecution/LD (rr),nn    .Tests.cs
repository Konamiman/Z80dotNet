using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public partial class Z80InstructionsExecutor
    {
        static object[] LD_rr_nn_Source =
        {
            new object[] {"BC", (byte)0x01},
            new object[] {"DE", (byte)0x11},
            new object[] {"HL", (byte)0x21},
            new object[] {"SP", (byte)0x31},
        };

        [Test]
        [TestCaseSource("LD_rr_nn_Source")]
        public void LD_rr_nn_works(string reg, byte opcode)
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
        public void LD_rr_nn_do_not_modify_flags(string reg, byte opcode)
        {
            var value = Fixture.Create<byte>();
            Registers.Main.F = value;
            Execute(opcode);

            Assert.AreEqual(value, Registers.Main.F);
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
