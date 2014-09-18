using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public partial class Z80InstructionsExecutor
    {
        private static object[] LD_r_n_Source =
        {
            new object[] {"A", (byte)0x3E, (byte?)null},
            new object[] {"B", (byte)0x06, (byte?)null},
            new object[] {"C", (byte)0x0E, (byte?)null},
            new object[] {"D", (byte)0x16, (byte?)null},
            new object[] {"E", (byte)0x1E, (byte?)null},
            new object[] {"H", (byte)0x26, (byte?)null},
            new object[] {"L", (byte)0x2E, (byte?)null},
            new object[] {"IXH", (byte)0x26, (byte?)0xDD},
            new object[] {"IXL", (byte)0x2E, (byte?)0xDD},
            new object[] {"IYH", (byte)0x26, (byte?)0xFD},
            new object[] {"IYL", (byte)0x2E, (byte?)0xFD},
        };

        [Test]
        [TestCaseSource("LD_r_n_Source")]
        public void LD_r_n_works(string reg, byte opcode, byte? prefix)
        {
            var oldValue = Fixture.Create<byte>();
            var newValue = Fixture.Create<byte>();

            SetReg(reg, oldValue);

            Execute(opcode, prefix, newValue);

            Assert.AreEqual(newValue, GetReg<byte>(reg));
        }

        [Test]
        [TestCaseSource("LD_r_n_Source")]
        public void LD_r_n_do_not_modify_flags(string reg, byte opcode, byte? prefix)
        {
            var value = Fixture.Create<byte>();
            Registers.F = value;
            Execute(opcode, prefix);

            Assert.AreEqual(value, Registers.F);
        }

        [Test]
        [TestCaseSource("LD_r_n_Source")]
        public void LD_r_n_returns_proper_T_states(string reg, byte opcode, byte? prefix)
        {
            var states = Execute(opcode, prefix);
            Assert.AreEqual(IfIndexRegister(reg, 11, @else: 7), states);
        }
    }
}