using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class SRL_tests : InstructionsExecutionTestsBase
    {
        private const byte prefix = 0xCB;

        public static object[] SRL_Source =
        {
            new object[] {"A", (byte)0x3F},
            new object[] {"B", (byte)0x38},
            new object[] {"C", (byte)0x39},
            new object[] {"D", (byte)0x3A},
            new object[] {"E", (byte)0x3B},
            new object[] {"H", (byte)0x3C},
            new object[] {"L", (byte)0x3D},
            new object[] {"(HL)", (byte)0x3E},
        };

        
        [Test]
        [TestCaseSource("SRL_Source")]
        public void SRL_shifts_byte_correctly(string reg, byte opcode)
        {
            var values = new byte[] { 0x7F, 0x3F, 0x1F, 0x0F, 0x07, 0x03, 0x01, 0 };
            SetupRegOrMem(reg, 0xFF);

            for(var i = 0; i < values.Length; i++)
            {
                Registers.CF = 1;
                Execute(opcode, prefix);
                Assert.AreEqual(values[i], ValueOfRegOrMem(reg));
            }
        }

        [Test]
        [TestCaseSource("SRL_Source")]
        public void SRL_sets_CF_from_bit_0(string reg, byte opcode)
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
        [TestCaseSource("SRL_Source")]
        public void SRL_resets_H_N_and_S(string reg, byte opcode)
        {
            AssertResetsFlags(opcode, prefix, "H", "N", "S");
        }

        [Test]
        [TestCaseSource("SRL_Source")]
        public void SRL_sets_ZF_appropriately(string reg, byte opcode)
        {
            for(int i=0; i<256; i++) 
            {
                SetupRegOrMem(reg, (byte)i);
                Execute(opcode, prefix);
                Assert.AreEqual(ValueOfRegOrMem(reg)==0, (bool)Registers.ZF);
            }
        }

        [Test]
        [TestCaseSource("SRL_Source")]
        public void SRL_sets_PV_appropriately(string reg, byte opcode)
        {
            for(int i=0; i<256; i++) 
            {
                SetupRegOrMem(reg, (byte)i);
                Execute(opcode, prefix);
                Assert.AreEqual(Parity[ValueOfRegOrMem(reg)], Registers.PF);
            }
        }

        [Test]
        [TestCaseSource("SRL_Source")]
        public void SRL_sets_bits_3_and_5_from_result(string reg, byte opcode)
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
        [TestCaseSource("SRL_Source")]
        public void SRL_returns_proper_T_states(string reg, byte opcode)
        {
            var states = Execute(opcode, prefix);
            Assert.AreEqual(reg == "(HL)" ? 15 : 8, states);
        }
    }
}