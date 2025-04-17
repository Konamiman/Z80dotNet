using NUnit.Framework;
using AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class LD_A_I_R_tests : InstructionsExecutionTestsBase
    {
        private const byte prefix = 0xED;

        public static object[] LD_A_R_I_Source =
        {
            new object[] {"I", (byte)0x57},
            new object[] {"R", (byte)0x5F}
        };

        [Test]
        [TestCaseSource(nameof(LD_A_R_I_Source))]
        public void LD_A_I_R_loads_value_correctly(string reg, byte opcode)
        {
            var oldValue = Fixture.Create<byte>();
            var newValue = Fixture.Create<byte>();
            Registers.A = oldValue;
            SetReg(reg, newValue);

            Execute(opcode, prefix);

            //Account for R being increased on instruction execution
            if(reg == "R")
                newValue = newValue.Inc7Bits().Inc7Bits();

            Assert.That(Registers.A, Is.EqualTo(newValue));
        }

        [Test]
        [TestCaseSource(nameof(LD_A_R_I_Source))]
        public void LD_A_I_R_returns_proper_T_states(string reg, byte opcode)
        {
            var states = Execute(opcode, prefix);
            Assert.That(states, Is.EqualTo(9));
        }

        [Test]
        [TestCaseSource(nameof(LD_A_R_I_Source))]
        public void LD_A_I_R_sets_SF_properly(string reg, byte opcode)
        {
            for(int i=0; i<=255; i++)
            {
                var b = (byte)i;
                SetReg(reg, b);
                Execute(opcode, prefix);
                Assert.That((bool)Registers.SF, Is.EqualTo(b >= 128));
            }
        }

        [Test]
        [TestCaseSource(nameof(LD_A_R_I_Source))]
        public void LD_A_I_R_sets_ZF_properly(string reg, byte opcode)
        {
            for(int i=0; i<=255; i++)
            {
                var b = (byte)i;
                SetReg(reg, b);
                Execute(opcode, prefix);

                //Account for R being increased on instruction execution
                if(reg == "R")
                    b = b.Inc7Bits().Inc7Bits();

                Assert.That((bool)Registers.ZF, Is.EqualTo(b == 0));
            }
        }

        [Test]
        [TestCaseSource(nameof(LD_A_R_I_Source))]
        public void LD_A_I_R_sets_PF_from_IFF2(string reg, byte opcode)
        {
            SetReg(reg, Fixture.Create<byte>());

            Registers.IFF2 = 0;
            Execute(opcode, prefix);
            Assert.That(Registers.PF.Value, Is.EqualTo(0));

            Registers.IFF2 = 1;
            Execute(opcode, prefix);
            Assert.That(Registers.PF.Value, Is.EqualTo(1));
        }

        [Test]
        [TestCaseSource(nameof(LD_A_R_I_Source))]
        public void LD_A_I_R_resets_HF_and_NF_properly(string reg, byte opcode)
        {
            AssertResetsFlags(opcode, prefix, "H", "N");
        }

        [Test]
        [TestCaseSource(nameof(LD_A_R_I_Source))]
        public void LD_A_I_R_does_not_change_CF(string reg, byte opcode)
        {
            AssertDoesNotChangeFlags(opcode, prefix, "C");
        }

        [Test]
        [TestCaseSource(nameof(LD_A_R_I_Source))]
        public void LD_A_I_R_sets_flags_3_5_from_I(string reg, byte opcode)
        {
            SetReg(reg, ((byte)1).WithBit(3, 1).WithBit(5, 0));
            Execute(opcode, prefix);
            Assert.Multiple(() =>
            {
                Assert.That(Registers.Flag3.Value, Is.EqualTo(1));
                Assert.That(Registers.Flag5.Value, Is.EqualTo(0));
            });

            SetReg(reg, ((byte)1).WithBit(3, 0).WithBit(5, 1));
            Execute(opcode, prefix);
            Assert.Multiple(() =>
            {
                Assert.That(Registers.Flag3.Value, Is.EqualTo(0));
                Assert.That(Registers.Flag5.Value, Is.EqualTo(1));
            });
        }
    }
}