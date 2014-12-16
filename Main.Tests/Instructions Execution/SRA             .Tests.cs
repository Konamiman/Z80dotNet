using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class SRA_tests : InstructionsExecutionTestsBase
    {
        private const byte prefix = 0xCB;

        public static object[] SRA_Source =
        {
            new object[] {"A", (byte)0x2F},
            new object[] {"B", (byte)0x28},
            new object[] {"C", (byte)0x29},
            new object[] {"D", (byte)0x2A},
            new object[] {"E", (byte)0x2B},
            new object[] {"H", (byte)0x2C},
            new object[] {"L", (byte)0x2D},
            new object[] {"(HL)", (byte)0x2E},
        };

        
        [Test]
        [TestCaseSource("SRA_Source")]
        public void SRA_shifts_negative_byte_correctly(string reg, byte opcode)
        {
            var values = new byte[] { 0xC0, 0xE0, 0xF0, 0xF8, 0xFC, 0xFE, 0xFF };
            SetupRegOrMem(reg, 0x80);

            for(var i = 0; i < values.Length; i++)
            {
                Registers.CF = 1;
                Execute(opcode, prefix);
                Assert.AreEqual(values[i], ValueOfRegOrMem(reg));
            }
        }

        [Test]
        [TestCaseSource("SRA_Source")]
        public void SRA_shifts_positive_byte_correctly(string reg, byte opcode)
        {
            var values = new byte[] { 0x20, 0x10, 0x08, 0x04, 0x02, 0x01, 0 };
            SetupRegOrMem(reg, 0x40);

            for(var i = 0; i < values.Length; i++)
            {
                Registers.CF = 1;
                Execute(opcode, prefix);
                Assert.AreEqual(values[i], ValueOfRegOrMem(reg));
            }
        }

        [Test]
        [TestCaseSource("SRA_Source")]
        public void SRA_sets_CF_from_bit_0(string reg, byte opcode)
        {
            SetupRegOrMem(reg, (byte)(Fixture.Create<byte>() | 0x01));
            Registers.CF = 0;
            Execute(opcode, prefix);
            Assert.AreEqual(1, Registers.CF);

            SetupRegOrMem(reg, (byte)(Fixture.Create<byte>() & 0xFE));
            Registers.CF = 1;
            Execute(opcode, prefix);
            Assert.AreEqual(0, Registers.CF);
        }

        [Test]
        [TestCaseSource("SRA_Source")]
        public void SRA_resets_H_and_N(string reg, byte opcode)
        {
            AssertResetsFlags(opcode, prefix, "H", "N");
        }

        [Test]
        [TestCaseSource("SRA_Source")]
        public void SRA_sets_SF_appropriately(string reg, byte opcode)
        {
            SetupRegOrMem(reg, (byte)(Fixture.Create<byte>() | 0x80));
            Execute(opcode, prefix);
            Assert.AreEqual(1, Registers.SF);

            SetupRegOrMem(reg, (byte)(Fixture.Create<byte>() & 0x7F));
            Execute(opcode, prefix);
            Assert.AreEqual(0, Registers.SF);
        }

        [Test]
        [TestCaseSource("SRA_Source")]
        public void SRA_sets_ZF_appropriately(string reg, byte opcode)
        {
            for(int i=0; i<256; i++) 
            {
                SetupRegOrMem(reg, (byte)i);
                Execute(opcode, prefix);
                Assert.AreEqual(ValueOfRegOrMem(reg)==0, (bool)Registers.ZF);
            }
        }

        [Test]
        [TestCaseSource("SRA_Source")]
        public void SRA_sets_PV_appropriately(string reg, byte opcode)
        {
            for(int i=0; i<256; i++) 
            {
                SetupRegOrMem(reg, (byte)i);
                Execute(opcode, prefix);
                Assert.AreEqual(Parity[ValueOfRegOrMem(reg)], Registers.PF);
            }
        }

        [Test]
        [TestCaseSource("SRA_Source")]
        public void SRA_sets_bits_3_and_5_from_result(string reg, byte opcode)
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
        [TestCaseSource("SRA_Source")]
        public void SRA_returns_proper_T_states(string reg, byte opcode)
        {
            var states = Execute(opcode, prefix);
            Assert.AreEqual(reg == "(HL)" ? 15 : 8, states);
        }
    }
}