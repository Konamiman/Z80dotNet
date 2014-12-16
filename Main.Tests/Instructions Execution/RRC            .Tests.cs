using NUnit.Framework;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class RRC_tests : InstructionsExecutionTestsBase
    {
        private const byte prefix = 0xCB;

        public static object[] RRC_Source =
        {
            new object[] {"A", (byte)0x0F},
            new object[] {"B", (byte)0x08},
            new object[] {"C", (byte)0x09},
            new object[] {"D", (byte)0x0A},
            new object[] {"E", (byte)0x0B},
            new object[] {"H", (byte)0x0C},
            new object[] {"L", (byte)0x0D},
            new object[] {"(HL)", (byte)0x0E},
        };

        
        [Test]
        [TestCaseSource("RRC_Source")]
        public void RRC_rotates_byte_correctly(string reg, byte opcode)
        {
            var values = new byte[] { 0x82, 0x41, 0xA0, 0x50, 0x28, 0x14, 0x0A, 0x05 };
            SetupRegOrMem(reg, 0x05);

            for(var i = 0; i < values.Length; i++)
            {
                Execute(opcode, prefix);
                Assert.AreEqual(values[i], ValueOfRegOrMem(reg));
            }
        }

        [Test]
        [TestCaseSource("RRC_Source")]
        public void RRC_sets_CF_correctly(string reg, byte opcode)
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
        [TestCaseSource("RRC_Source")]
        public void RRC_resets_H_and_N(string reg, byte opcode)
        {
            AssertResetsFlags(opcode, prefix, "H", "N");
        }

        [Test]
        [TestCaseSource("RRC_Source")]
        public void RRC_sets_SF_appropriately(string reg, byte opcode)
        {
            SetupRegOrMem(reg, 0x02);

            Execute(opcode, prefix);
            Assert.AreEqual(0, Registers.SF);

            Execute(opcode, prefix);
            Assert.AreEqual(1, Registers.SF);

            Execute(opcode, prefix);
            Assert.AreEqual(0, Registers.SF);
        }

        [Test]
        [TestCaseSource("RRC_Source")]
        public void RRC_sets_ZF_appropriately(string reg, byte opcode)
        {
            for(int i=0; i<256; i++) 
            {
                SetupRegOrMem(reg, (byte)i);
                Execute(opcode, prefix);
                Assert.AreEqual(ValueOfRegOrMem(reg)==0, (bool)Registers.ZF);
            }
        }

        [Test]
        [TestCaseSource("RRC_Source")]
        public void RRC_sets_PV_appropriately(string reg, byte opcode)
        {
            for(int i=0; i<256; i++) 
            {
                SetupRegOrMem(reg, (byte)i);
                Execute(opcode, prefix);
                Assert.AreEqual(Parity[ValueOfRegOrMem(reg)], Registers.PF);
            }
        }

        [Test]
        [TestCaseSource("RRC_Source")]
        public void RRC_sets_bits_3_and_5_from_A(string reg, byte opcode)
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
        [TestCaseSource("RRC_Source")]
        public void RRC_returns_proper_T_states(string reg, byte opcode)
        {
            var states = Execute(opcode, prefix);
            Assert.AreEqual(reg == "(HL)" ? 15 : 8, states);
        }
    }
}