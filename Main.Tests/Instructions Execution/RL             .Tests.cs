﻿using NUnit.Framework;
using AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class RL_tests : InstructionsExecutionTestsBase
    {
        static RL_tests()
        {
            RL_Source = GetBitInstructionsSource(0x10, includeLoadReg: true, loopSevenBits: false);
        }

        public static object[] RL_Source;

        private byte offset;

        [SetUp]
        public void Setup()
        {
            offset = Fixture.Create<byte>();
        }

        [Test]
        [TestCaseSource(nameof(RL_Source))]
        public void RL_rotates_byte_correctly(string reg, string destReg, byte opcode, byte? prefix, int bit)
        {
            var values = new byte[] { 0x6, 0xC, 0x18, 0x30, 0x60, 0xC0, 0x80, 0 };
            SetupRegOrMem(reg, 0x03, offset);

            foreach (byte value in values)
            {
                ExecuteBit(opcode, prefix, offset);
                Assert.That(ValueOfRegOrMem(reg, offset) & 0xFE, Is.EqualTo(value));
            }
        }

        [Test]
        [TestCaseSource(nameof(RL_Source))]
        public void RL_sets_bit_0_from_CF(string reg, string destReg, byte opcode, byte? prefix, int bit)
        {
            SetupRegOrMem(reg, (byte)(Fixture.Create<byte>() | 1), offset);
            Registers.CF = 0;
            ExecuteBit(opcode, prefix, offset);
            Assert.That(ValueOfRegOrMem(reg, offset).GetBit(0).Value, Is.EqualTo(0));

            SetupRegOrMem(reg, (byte)(Fixture.Create<byte>() & 0xFE), offset);
            Registers.CF = 1;
            ExecuteBit(opcode, prefix, offset);
            Assert.That(ValueOfRegOrMem(reg, offset).GetBit(0).Value, Is.EqualTo(1));
        }

        [Test]
        [TestCaseSource(nameof(RL_Source))]
        public void RL_sets_CF_correctly(string reg, string destReg, byte opcode, byte? prefix, int bit)
        {
            SetupRegOrMem(reg, 0x60, offset);

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
        [TestCaseSource(nameof(RL_Source))]
        public void RL_resets_H_and_N(string reg, string destReg, byte opcode, byte? prefix, int bit)
        {
            AssertResetsFlags(() => ExecuteBit(opcode, prefix, offset), opcode, prefix, "H", "N");
        }

        [Test]
        [TestCaseSource(nameof(RL_Source))]
        public void RL_sets_SF_appropriately(string reg, string destReg, byte opcode, byte? prefix, int bit)
        {
            SetupRegOrMem(reg, 0x20, offset);

            ExecuteBit(opcode, prefix, offset);
            Assert.That(Registers.SF.Value, Is.EqualTo(0));

            ExecuteBit(opcode, prefix, offset);
            Assert.That(Registers.SF.Value, Is.EqualTo(1));

            ExecuteBit(opcode, prefix, offset);
            Assert.That(Registers.SF.Value, Is.EqualTo(0));
        }

        [Test]
        [TestCaseSource(nameof(RL_Source))]
        public void RL_sets_ZF_appropriately(string reg, string destReg, byte opcode, byte? prefix, int bit)
        {
            for(int i=0; i<256; i++) 
            {
                SetupRegOrMem(reg, (byte)i, offset);
                ExecuteBit(opcode, prefix, offset);
                Assert.That((bool)Registers.ZF, Is.EqualTo(ValueOfRegOrMem(reg, offset)==0));
            }
        }

        [Test]
        [TestCaseSource(nameof(RL_Source))]
        public void RL_sets_PV_appropriately(string reg, string destReg, byte opcode, byte? prefix, int bit)
        {
            for(int i=0; i<256; i++) 
            {
                SetupRegOrMem(reg, (byte)i, offset);
                ExecuteBit(opcode, prefix, offset);
                Assert.That(Registers.PF.Value, Is.EqualTo(Parity[ValueOfRegOrMem(reg, offset)]));
            }
        }

        [Test]
        [TestCaseSource(nameof(RL_Source))]
        public void RL_sets_bits_3_and_5_from_result(string reg, string destReg, byte opcode, byte? prefix, int bit)
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
        [TestCaseSource(nameof(RL_Source))]
        public void RL_returns_proper_T_states(string reg, string destReg, byte opcode, byte? prefix, int bit)
        {
            var states = ExecuteBit(opcode, prefix, offset);
            Assert.That(states, Is.EqualTo(reg == "(HL)" ? 15 : reg.StartsWith("(I") ? 23 : 8));
        }
    }
}