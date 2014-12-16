using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class RLC_tests : InstructionsExecutionTestsBase
    {
        private const byte prefix = 0xCB;

        public static object[] RLC_Source =
        {
            new object[] {"A", (byte)0x07},
            new object[] {"B", (byte)0x00},
            new object[] {"C", (byte)0x01},
            new object[] {"D", (byte)0x02},
            new object[] {"E", (byte)0x03},
            new object[] {"H", (byte)0x04},
            new object[] {"L", (byte)0x05},
            new object[] {"(HL)", (byte)0x06},
        };

        private void Setup(string reg, byte value)
        {
            if(reg == "(HL)") {
                var address = Fixture.Create<ushort>();
                ProcessorAgent.Memory[address] = value;
                Registers.HL = address.ToShort();
            }
            else {
                SetReg(reg, value);
            }
        }

        private byte Value(string reg)
        {
            if(reg == "(HL)") {
                return ProcessorAgent.Memory[Registers.HL];
            }
            else {
                return GetReg<byte>(reg);
            }
        }

        [Test]
        [TestCaseSource("RLC_Source")]
        public void RLC_rotates_byte_correctly(string reg, byte opcode)
        {
            var values = new byte[] {0xA, 0x14, 0x28, 0x50, 0xA0, 0x41, 0x82, 0x05};
            Setup(reg, 0x05);

            for(var i = 0; i < values.Length; i++)
            {
                Execute(opcode, prefix);
                Assert.AreEqual(values[i], Value(reg));
            }
        }

        [Test]
        [TestCaseSource("RLC_Source")]
        public void RLC_sets_CF_correctly(string reg, byte opcode)
        {
            Setup(reg, 0x60);

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
        [TestCaseSource("RLC_Source")]
        public void RLC_resets_H_and_N(string reg, byte opcode)
        {
            AssertResetsFlags(opcode, prefix, "H", "N");
        }

        [Test]
        [TestCaseSource("RLC_Source")]
        public void RLC_sets_SF_appropriately(string reg, byte opcode)
        {
            Setup(reg, 0x20);

            Execute(opcode, prefix);
            Assert.AreEqual(0, Registers.SF);

            Execute(opcode, prefix);
            Assert.AreEqual(1, Registers.SF);

            Execute(opcode, prefix);
            Assert.AreEqual(0, Registers.SF);
        }

        [Test]
        [TestCaseSource("RLC_Source")]
        public void RLC_sets_ZF_appropriately(string reg, byte opcode)
        {
            for(int i=0; i<256; i++) 
            {
                Setup(reg, (byte)i);
                Execute(opcode, prefix);
                Assert.AreEqual(Value(reg)==0, (bool)Registers.ZF);
            }
        }

        [Test]
        [TestCaseSource("RLC_Source")]
        public void RLC_sets_PV_appropriately(string reg, byte opcode)
        {
            for(int i=0; i<256; i++) 
            {
                Setup(reg, (byte)i);
                Execute(opcode, prefix);
                Assert.AreEqual(Parity[Value(reg)], Registers.PF);
            }
        }

        [Test]
        [TestCaseSource("RLC_Source")]
        public void RLC_sets_bits_3_and_5_from_A(string reg, byte opcode)
        {
            foreach (var b in new byte[] {0x00, 0xD7, 0x28, 0xFF})
            {
                Setup(reg, b);
                Execute(opcode, prefix);
                var value = Value(reg);
                Assert.AreEqual(value.GetBit(3), Registers.Flag3);
                Assert.AreEqual(value.GetBit(5), Registers.Flag5);
            }
        }

        [Test]
        [TestCaseSource("RLC_Source")]
        public void RLC_returns_proper_T_states(string reg, byte opcode)
        {
            var states = Execute(opcode, prefix);
            Assert.AreEqual(reg == "(HL)" ? 15 : 8, states);
        }
    }
}