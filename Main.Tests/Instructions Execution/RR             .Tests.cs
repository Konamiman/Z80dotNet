using NUnit.Framework;
using AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class RR_tests : InstructionsExecutionTestsBase
    {
        static RR_tests()
        {
            RR_Source = GetBitInstructionsSource(0x18, includeLoadReg: true, loopSevenBits: false);
        }

        public static object[] RR_Source;

        private byte offset;

        [SetUp]
        public void Setup()
        {
            offset = Fixture.Create<byte>();
        }
        
        [Test]
        [TestCaseSource(nameof(RR_Source))]
        public void RR_rotates_byte_and_loads_register_correctly(string reg, string destReg, byte opcode, byte? prefix, int bit)
        {
            var values = new byte[] { 0x60, 0x30, 0x18, 0xC, 0x6, 0x3, 0x1, 0x0 };
            SetupRegOrMem(reg, 0xC0, offset);

            for(var i = 0; i < values.Length; i++)
            {
                ExecuteBit(opcode, prefix, offset);
                Assert.That(ValueOfRegOrMem(reg, offset) & 0x7F, Is.EqualTo(values[i]));
                if(!string.IsNullOrEmpty(destReg))
                    Assert.That(ValueOfRegOrMem(destReg, offset) & 0x7F, Is.EqualTo(values[i]));
            }
        }

        [Test]
        [TestCaseSource(nameof(RR_Source))]
        public void RR_sets_bit_7_from_CF(string reg, string destReg, byte opcode, byte? prefix, int bit)
        {
            SetupRegOrMem(reg, (byte)(Fixture.Create<byte>() | 0X80), offset);
            Registers.CF = 0;
            ExecuteBit(opcode, prefix, offset);
            Assert.That(ValueOfRegOrMem(reg, offset).GetBit(7).Value, Is.EqualTo(0));

            SetupRegOrMem(reg, (byte)(Fixture.Create<byte>() & 0x7f), offset);
            Registers.CF = 1;
            ExecuteBit(opcode, prefix, offset);
            Assert.That(ValueOfRegOrMem(reg, offset).GetBit(7).Value, Is.EqualTo(1));
        }

        [Test]
        [TestCaseSource(nameof(RR_Source))]
        public void RR_sets_CF_correctly(string reg, string destReg, byte opcode, byte? prefix, int bit)
        {
            SetupRegOrMem(reg, 0x06, offset);

            ExecuteBit(opcode, prefix, offset);
            Assert.That(Registers.CF.Value, Is.EqualTo(0));

            ExecuteBit(opcode, prefix, offset);
            Assert.That(Registers.CF.Value, Is.EqualTo(1));

            ExecuteBit(opcode, prefix, offset);
            Assert.That(Registers.CF.Value, Is.EqualTo(1));

            ExecuteBit(opcode, prefix, offset);
            Assert.That(Registers.CF.Value, Is.EqualTo(0));
        }

        [Test]
        [TestCaseSource(nameof(RR_Source))]
        public void RR_resets_H_and_N(string reg, string destReg, byte opcode, byte? prefix, int bit)
        {
            AssertResetsFlags(() => ExecuteBit(opcode, prefix, offset), opcode, prefix, "H", "N");
        }

        [Test]
        [TestCaseSource(nameof(RR_Source))]
        public void RR_sets_SF_appropriately(string reg, string destReg, byte opcode, byte? prefix, int bit)
        {
            Registers.CF = 1;
            ExecuteBit(opcode, prefix, offset);
            Assert.That(Registers.SF.Value, Is.EqualTo(1));

            Registers.CF = 0;
            ExecuteBit(opcode, prefix, offset);
            Assert.That(Registers.SF.Value, Is.EqualTo(0));
        }

        [Test]
        [TestCaseSource(nameof(RR_Source))]
        public void RR_sets_ZF_appropriately(string reg, string destReg, byte opcode, byte? prefix, int bit)
        {
            for(int i=0; i<256; i++) 
            {
                SetupRegOrMem(reg, (byte)i, offset);
                ExecuteBit(opcode, prefix, offset);
                Assert.That((bool)Registers.ZF, Is.EqualTo(ValueOfRegOrMem(reg, offset)==0));
            }
        }

        [Test]
        [TestCaseSource(nameof(RR_Source))]
        public void RR_sets_PV_appropriately(string reg, string destReg, byte opcode, byte? prefix, int bit)
        {
            for(int i=0; i<256; i++) 
            {
                SetupRegOrMem(reg, (byte)i, offset);
                ExecuteBit(opcode, prefix, offset);
                Assert.That(Registers.PF.Value, Is.EqualTo(Parity[ValueOfRegOrMem(reg, offset)]));
            }
        }

        [Test]
        [TestCaseSource(nameof(RR_Source))]
        public void RR_sets_bits_3_and_5_from_result(string reg, string destReg, byte opcode, byte? prefix, int bit)
        {
            foreach (var b in new byte[] {0x00, 0xD7, 0x28, 0xFF})
            {
                SetupRegOrMem(reg, b, offset);
                ExecuteBit(opcode, prefix, offset);
                var value = ValueOfRegOrMem(reg, offset);
                Assert.Multiple(() =>
                {
                    Assert.That(Registers.Flag3, Is.EqualTo(value.GetBit(3)));
                    Assert.That(Registers.Flag5, Is.EqualTo(value.GetBit(5)));
                });
            }
        }

        [Test]
        [TestCaseSource(nameof(RR_Source))]
        public void RR_returns_proper_T_states(string reg, string destReg, byte opcode, byte? prefix, int bit)
        {
            var states = ExecuteBit(opcode, prefix, offset);
            Assert.That(states, Is.EqualTo(reg == "(HL)" ? 15 : reg.StartsWith("(I") ? 23 : 8));
        }
    }
}