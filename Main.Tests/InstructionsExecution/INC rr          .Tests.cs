using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public partial class Z80InstructionsExecutor
    {
        static object[] INC_rr_Source =
        {
            new object[] {"BC", (byte)0x03, null},
            new object[] {"DE", (byte)0x13, null},
            new object[] {"HL", (byte)0x23, null},
            new object[] {"SP", (byte)0x33, null},
            new object[] {"IX", (byte)0x23, (byte?)0xDD},
            new object[] {"IY", (byte)0x23, (byte?)0xFD},
        };

        [Test]
        [TestCaseSource("INC_rr_Source")]
        public void INC_rr_increases_register(string reg, byte opcode, byte? prefix)
        {
            SetReg(reg, 0xFFFF.ToShort());
            Execute(opcode, prefix);
            Assert.AreEqual(0, GetReg<short>(reg));
        }

        [Test]
        [TestCaseSource("INC_rr_Source")]
        public void INC_rr_do_not_modify_flags(string reg, byte opcode, byte? prefix)
        {
            var value = Fixture.Create<byte>();
            Registers.F = value;
            Execute(opcode, prefix);

            Assert.AreEqual(value, Registers.F);
        }

        [Test]
        [TestCaseSource("INC_rr_Source")]
        public void INC_rr_returns_proper_T_states(string reg, byte opcode, byte? prefix)
        {
            var states = Execute(opcode, prefix);
            Assert.AreEqual(IfIndexRegister(reg, 10, @else: 6), states);
        }
    }
}
