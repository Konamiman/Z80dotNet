using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class RL_tests : InstructionsExecutionTestsBase
    {
        private const byte prefix = 0xCB;

        public static object[] RL_Source =
        {
            new object[] {"A", (byte)0x17},
            new object[] {"B", (byte)0x10},
            new object[] {"C", (byte)0x11},
            new object[] {"D", (byte)0x12},
            new object[] {"E", (byte)0x13},
            new object[] {"H", (byte)0x14},
            new object[] {"L", (byte)0x15},
            new object[] {"(HL)", (byte)0x16},
        };

        
        [Test]
        [TestCaseSource("RL_Source")]
        public void RL_rotates_byte_correctly(string reg, byte opcode)
        {
            var values = new byte[] { 0x6, 0xC, 0x18, 0x30, 0x60, 0xC0, 0x80, 0 };
            SetupRegOrMem(reg, 0x03);

            for(var i = 0; i < values.Length; i++)
            {
                Execute(opcode, prefix);
                Assert.AreEqual(values[i], ValueOfRegOrMem(reg) & 0xFE);
            }
        }

        [Test]
        [TestCaseSource("RL_Source")]
        public void RL_sets_bit_0_from_CF(string reg, byte opcode)
        {
            SetupRegOrMem(reg, (byte)(Fixture.Create<byte>() | 1));
            Registers.CF = 0;
            Execute(opcode, prefix);
            Assert.AreEqual(0, ValueOfRegOrMem(reg).GetBit(0));

            SetupRegOrMem(reg, (byte)(Fixture.Create<byte>() & 0xFE));
            Registers.CF = 1;
            Execute(opcode, prefix);
            Assert.AreEqual(1, ValueOfRegOrMem(reg).GetBit(0));
        }

        [Test]
        [TestCaseSource("RL_Source")]
        public void RL_sets_CF_correctly(string reg, byte opcode)
        {
            SetupRegOrMem(reg, 0x60);

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
        [TestCaseSource("RL_Source")]
        public void RL_resets_H_and_N(string reg, byte opcode)
        {
            AssertResetsFlags(opcode, prefix, "H", "N");
        }

        [Test]
        [TestCaseSource("RL_Source")]
        public void RL_sets_SF_appropriately(string reg, byte opcode)
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
        [TestCaseSource("RL_Source")]
        public void RL_sets_ZF_appropriately(string reg, byte opcode)
        {
            for(int i=0; i<256; i++) 
            {
                SetupRegOrMem(reg, (byte)i);
                Execute(opcode, prefix);
                Assert.AreEqual(ValueOfRegOrMem(reg)==0, (bool)Registers.ZF);
            }
        }

        [Test]
        [TestCaseSource("RL_Source")]
        public void RL_sets_PV_appropriately(string reg, byte opcode)
        {
            for(int i=0; i<256; i++) 
            {
                SetupRegOrMem(reg, (byte)i);
                Execute(opcode, prefix);
                Assert.AreEqual(Parity[ValueOfRegOrMem(reg)], Registers.PF);
            }
        }

        [Test]
        [TestCaseSource("RL_Source")]
        public void RL_sets_bits_3_and_5_from_result(string reg, byte opcode)
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
        [TestCaseSource("RL_Source")]
        public void RL_returns_proper_T_states(string reg, byte opcode)
        {
            var states = Execute(opcode, prefix);
            Assert.AreEqual(reg == "(HL)" ? 15 : 8, states);
        }
    }
}