using NUnit.Framework;
using AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class INC_r_tests : InstructionsExecutionTestsBase
    {
        public static object[] INC_r_Source =
        {
            new object[] {"A",   (byte)0x3C, (byte?)null},
            new object[] {"B",   (byte)0x04, (byte?)null},
            new object[] {"C",   (byte)0x0C, (byte?)null},
            new object[] {"D",   (byte)0x14, (byte?)null},
            new object[] {"E",   (byte)0x1C, (byte?)null},
            new object[] {"H",   (byte)0x24, (byte?)null},
            new object[] {"L",   (byte)0x2C, (byte?)null},
            new object[] {"IXH", (byte)0x24, (byte?)0xDD},
            new object[] {"IXL", (byte)0x2C, (byte?)0xDD},
            new object[] {"IYH", (byte)0x24, (byte?)0xFD},
            new object[] {"IYL", (byte)0x2C, (byte?)0xFD}
        };

        [Test]
        [TestCaseSource(nameof(INC_r_Source))]
        public void INC_r_increases_value_appropriately(string reg, byte opcode, byte? prefix)
        {
            SetReg(reg, 0xFE);
            Execute(opcode, prefix);
            Assert.That(GetReg<byte>(reg), Is.EqualTo(0xFF));

            Execute(opcode, prefix);
            Assert.That(GetReg<byte>(reg), Is.EqualTo(0x00));

            Execute(opcode, prefix);
            Assert.That(GetReg<byte>(reg), Is.EqualTo(0x01));
        }

        [Test]
        [TestCaseSource(nameof(INC_r_Source))]
        public void INC_r_sets_SF_appropriately(string reg, byte opcode, byte? prefix)
        {
            SetReg(reg, 0xFD);

            Execute(opcode, prefix);
            Assert.That(Registers.SF.Value, Is.EqualTo(1));

            Execute(opcode, prefix);
            Assert.That(Registers.SF.Value, Is.EqualTo(1));

            Execute(opcode, prefix);
            Assert.That(Registers.SF.Value, Is.EqualTo(0));

            Execute(opcode, prefix);
            Assert.That(Registers.SF.Value, Is.EqualTo(0));
        }

        [Test]
        [TestCaseSource(nameof(INC_r_Source))]
        public void INC_r_sets_ZF_appropriately(string reg, byte opcode, byte? prefix)
        {
            SetReg(reg, 0xFD);

            Execute(opcode, prefix);
            Assert.That(Registers.ZF.Value, Is.EqualTo(0));

            Execute(opcode, prefix);
            Assert.That(Registers.ZF.Value, Is.EqualTo(0));

            Execute(opcode, prefix);
            Assert.That(Registers.ZF.Value, Is.EqualTo(1));

            Execute(opcode, prefix);
            Assert.That(Registers.ZF.Value, Is.EqualTo(0));
        }

        [Test]
        [TestCaseSource(nameof(INC_r_Source))]
        public void INC_r_sets_HF_appropriately(string reg, byte opcode, byte? prefix)
        {
            foreach(byte b in new byte[] { 0x0E, 0x7E, 0xFE }) 
            {
                SetReg(reg, b);

                Execute(opcode, prefix);
                Assert.That(Registers.HF.Value, Is.EqualTo(0));

                Execute(opcode, prefix);
                Assert.That(Registers.HF.Value, Is.EqualTo(1));

                Execute(opcode, prefix);
                Assert.That(Registers.HF.Value, Is.EqualTo(0));
            }
        }

        [Test]
        [TestCaseSource(nameof(INC_r_Source))]
        public void INC_r_sets_PF_appropriately(string reg, byte opcode, byte? prefix)
        {
            SetReg(reg, 0x7E);

            Execute(opcode, prefix);
            Assert.That(Registers.PF.Value, Is.EqualTo(0));

            Execute(opcode, prefix);
            Assert.That(Registers.PF.Value, Is.EqualTo(1));

            Execute(opcode, prefix);
            Assert.That(Registers.PF.Value, Is.EqualTo(0));
        }

        [Test]
        [TestCaseSource(nameof(INC_r_Source))]
        public void INC_r_resets_NF(string reg, byte opcode, byte? prefix)
        {
            AssertResetsFlags(opcode, prefix, "N");
        }

        [Test]
        [TestCaseSource(nameof(INC_r_Source))]
        public void INC_r_does_not_change_CF(string reg, byte opcode, byte? prefix)
        {
            var randomValues = Fixture.Create<byte[]>();

            foreach (var value in randomValues)
            {
                SetReg(reg, value);

                Registers.CF = 0;
                Execute(opcode, prefix);
                Assert.That(Registers.CF.Value, Is.EqualTo(0));

                Registers.CF = 1;
                Execute(opcode, prefix);
                Assert.That(Registers.CF.Value, Is.EqualTo(1));
            }
        }

        [Test]
        [TestCaseSource(nameof(INC_r_Source))]
        public void INC_r_sets_bits_3_and_5_from_result(string reg, byte opcode, byte? prefix)
        {
            SetReg(reg, ((byte)0).WithBit(3, 1).WithBit(5, 0));
            Execute(opcode, prefix);
            Assert.Multiple(() =>
            {
                Assert.That(Registers.Flag3.Value, Is.EqualTo(1));
                Assert.That(Registers.Flag5.Value, Is.EqualTo(0));
            });

            SetReg(reg, ((byte)0).WithBit(3, 0).WithBit(5, 1));
            Execute(opcode, prefix);
            Assert.Multiple(() =>
            {
                Assert.That(Registers.Flag3.Value, Is.EqualTo(0));
                Assert.That(Registers.Flag5.Value, Is.EqualTo(1));
            });
        }

        [Test]
        [TestCaseSource(nameof(INC_r_Source))]
        public void INC_r_returns_proper_T_states(string reg, byte opcode, byte? prefix)
        {
            var states = Execute(opcode, prefix);
            Assert.That(states, Is.EqualTo(IfIndexRegister(reg, 8, @else: 4)));
        }
    }
}
