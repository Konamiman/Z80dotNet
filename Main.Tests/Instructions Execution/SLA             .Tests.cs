using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class SLA_tests : InstructionsExecutionTestsBase
    {
        private const byte prefix = 0xCB;

        public static object[] SLA_Source =
        {
            new object[] {"A", (byte)0x27},
            new object[] {"B", (byte)0x20},
            new object[] {"C", (byte)0x21},
            new object[] {"D", (byte)0x22},
            new object[] {"E", (byte)0x23},
            new object[] {"H", (byte)0x24},
            new object[] {"L", (byte)0x25},
            new object[] {"(HL)", (byte)0x26},
        };

        
        [Test]
        [TestCaseSource("SLA_Source")]
        public void SLA_shifts_byte_correctly(string reg, byte opcode)
        {
            var values = new byte[] { 0xFE, 0xFC, 0xF8, 0xF0, 0xE0, 0xC0, 0x80, 0 };
            SetupRegOrMem(reg, 0xFF);

            for(var i = 0; i < values.Length; i++)
            {
                Registers.CF = 1;
                Execute(opcode, prefix);
                Assert.AreEqual(values[i], ValueOfRegOrMem(reg));
            }
        }

        [Test]
        [TestCaseSource("SLA_Source")]
        public void SLA_sets_CF_from_bit_7(string reg, byte opcode)
        {
            SetupRegOrMem(reg, (byte)(Fixture.Create<byte>() | 0x80));
            Registers.CF = 0;
            Execute(opcode, prefix);
            Assert.AreEqual(1, Registers.CF);

            SetupRegOrMem(reg, (byte)(Fixture.Create<byte>() & 0x7F));
            Registers.CF = 1;
            Execute(opcode, prefix);
            Assert.AreEqual(0, Registers.CF);
        }

        [Test]
        [TestCaseSource("SLA_Source")]
        public void SLA_resets_H_and_N(string reg, byte opcode)
        {
            AssertResetsFlags(opcode, prefix, "H", "N");
        }

        [Test]
        [TestCaseSource("SLA_Source")]
        public void SLA_sets_SF_appropriately(string reg, byte opcode)
        {
            SetupRegOrMem(reg, 0x20);

            Execute(opcode, prefix);
            Assert.AreEqual(0, Registers.SF);

            Execute(opcode, prefix);
            Assert.AreEqual(1, Registers.SF);

            Execute(opcode, prefix);
            Assert.AreEqual(0, Registers.SF);
        }

        [Test]
        [TestCaseSource("SLA_Source")]
        public void SLA_sets_ZF_appropriately(string reg, byte opcode)
        {
            for(int i=0; i<256; i++) 
            {
                SetupRegOrMem(reg, (byte)i);
                Execute(opcode, prefix);
                Assert.AreEqual(ValueOfRegOrMem(reg)==0, (bool)Registers.ZF);
            }
        }

        [Test]
        [TestCaseSource("SLA_Source")]
        public void SLA_sets_PV_appropriately(string reg, byte opcode)
        {
            for(int i=0; i<256; i++) 
            {
                SetupRegOrMem(reg, (byte)i);
                Execute(opcode, prefix);
                Assert.AreEqual(Parity[ValueOfRegOrMem(reg)], Registers.PF);
            }
        }

        [Test]
        [TestCaseSource("SLA_Source")]
        public void SLA_sets_bits_3_and_5_from_result(string reg, byte opcode)
        {
            foreach (var b in new byte[] {0x00, 0xD7, 0x28, 0xFF})
            {
                SetupRegOrMem(reg, b);
                Execute(opcode, prefix);
                var value = ValueOfRegOrMem(reg);
                Assert.AreEqual(value.GetBit(3), Registers.Flag3);
                Assert.AreEqual(value.GetBit(5), Registers.Flag5);
            }
        }

        [Test]
        [TestCaseSource("SLA_Source")]
        public void SLA_returns_proper_T_states(string reg, byte opcode)
        {
            var states = Execute(opcode, prefix);
            Assert.AreEqual(reg == "(HL)" ? 15 : 8, states);
        }
    }
}