using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class RR_tests : InstructionsExecutionTestsBase
    {
        private const byte prefix = 0xCB;

        public static object[] RR_Source =
        {
            new object[] {"A", (byte)0x1F},
            new object[] {"B", (byte)0x18},
            new object[] {"C", (byte)0x19},
            new object[] {"D", (byte)0x1A},
            new object[] {"E", (byte)0x1B},
            new object[] {"H", (byte)0x1C},
            new object[] {"L", (byte)0x1D},
            new object[] {"(HL)", (byte)0x1E},
        };

        
        [Test]
        [TestCaseSource("RR_Source")]
        public void RR_rotates_byte_correctly(string reg, byte opcode)
        {
            var values = new byte[] { 0x60, 0x30, 0x18, 0xC, 0x6, 0x3, 0x1, 0x0 };
            SetupRegOrMem(reg, 0xC0);

            for(var i = 0; i < values.Length; i++)
            {
                Execute(opcode, prefix);
                Assert.AreEqual(values[i], ValueOfRegOrMem(reg) & 0x7F);
            }
        }

        [Test]
        [TestCaseSource("RR_Source")]
        public void RR_sets_bit_7_from_CF(string reg, byte opcode)
        {
            SetupRegOrMem(reg, (byte)(Fixture.Create<byte>() | 0X80));
            Registers.CF = 0;
            Execute(opcode, prefix);
            Assert.AreEqual(0, ValueOfRegOrMem(reg).GetBit(7));

            SetupRegOrMem(reg, (byte)(Fixture.Create<byte>() & 0x7f));
            Registers.CF = 1;
            Execute(opcode, prefix);
            Assert.AreEqual(1, ValueOfRegOrMem(reg).GetBit(7));
        }

        [Test]
        [TestCaseSource("RR_Source")]
        public void RR_sets_CF_correctly(string reg, byte opcode)
        {
            SetupRegOrMem(reg, 0x06);

            Execute(opcode, prefix);
            Assert.AreEqual(0, Registers.CF);

            Execute(opcode, prefix);
            Assert.AreEqual(1, Registers.CF);

            Execute(opcode, prefix);
            Assert.AreEqual(1, Registers.CF);

            Execute(opcode, prefix);
            Assert.AreEqual(0, Registers.CF);
        }

        [Test]
        [TestCaseSource("RR_Source")]
        public void RR_resets_H_and_N(string reg, byte opcode)
        {
            AssertResetsFlags(opcode, prefix, "H", "N");
        }

        [Test]
        [TestCaseSource("RR_Source")]
        public void RR_sets_SF_appropriately(string reg, byte opcode)
        {
            Registers.CF = 1;
            Execute(opcode, prefix);
            Assert.AreEqual(1, Registers.SF);

            Registers.CF = 0;
            Execute(opcode, prefix);
            Assert.AreEqual(0, Registers.SF);
        }

        [Test]
        [TestCaseSource("RR_Source")]
        public void RR_sets_ZF_appropriately(string reg, byte opcode)
        {
            for(int i=0; i<256; i++) 
            {
                SetupRegOrMem(reg, (byte)i);
                Execute(opcode, prefix);
                Assert.AreEqual(ValueOfRegOrMem(reg)==0, (bool)Registers.ZF);
            }
        }

        [Test]
        [TestCaseSource("RR_Source")]
        public void RR_sets_PV_appropriately(string reg, byte opcode)
        {
            for(int i=0; i<256; i++) 
            {
                SetupRegOrMem(reg, (byte)i);
                Execute(opcode, prefix);
                Assert.AreEqual(Parity[ValueOfRegOrMem(reg)], Registers.PF);
            }
        }

        [Test]
        [TestCaseSource("RR_Source")]
        public void RR_sets_bits_3_and_5_from_result(string reg, byte opcode)
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
        [TestCaseSource("RR_Source")]
        public void RR_returns_proper_T_states(string reg, byte opcode)
        {
            var states = Execute(opcode, prefix);
            Assert.AreEqual(reg == "(HL)" ? 15 : 8, states);
        }
    }
}