﻿using NUnit.Framework;
using AutoFixture;
using System.Collections.Generic;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class OR_r_tests : InstructionsExecutionTestsBase
    {
        static OR_r_tests()
        {
            var combinations = new List<object[]>();

            var registers = new[] {"B", "C", "D", "E", "H", "L", "(HL)", "n", "IXH", "IXL", "IYH", "IYL","(IX+n)","(IY+n)"};
            for(var src = 0; src<registers.Length; src++)
            {
                var reg = registers[src];
                var i = src;
                byte? prefix = null;

                ModifyTestCaseCreationForIndexRegs(reg, ref i, out prefix);

                var opcode = (byte)(i==7 ? 0xF6 : (i | 0xB0));
                combinations.Add(new object[] {reg, opcode, prefix});
            }

            OR_r_Source = combinations.ToArray();
        }

        public static object[] OR_r_Source;

        public static object[] OR_A_Source =
        {
            new object[] {"A", (byte)0xB7, (byte?)null},
        };


        [Test]
        [TestCaseSource(nameof(OR_r_Source))]
        public void OR_r_ors_both_registers(string src, byte opcode, byte? prefix)
        {
            var oldValue = Fixture.Create<byte>();
            var valueToOr = Fixture.Create<byte>();

            Setup(src, oldValue, valueToOr);
            Execute(opcode, prefix);

            Assert.That(Registers.A, Is.EqualTo(oldValue | valueToOr));
        }

        [Test]
        [TestCaseSource(nameof(OR_A_Source))]
        public void OR_A_does_not_change_A(string src, byte opcode, byte? prefix)
        {
            var value = Fixture.Create<byte>();

            Registers.A = value;
            Execute(opcode, prefix);

            Assert.That(Registers.A, Is.EqualTo(value));
        }

        private void Setup(string src, byte oldValue, byte valueToOr)
        {
            Registers.A = oldValue;

            if(src == "n") 
            {
                SetMemoryContentsAt(1, valueToOr);
            }
            else if(src == "(HL)") 
            {
                var address = Fixture.Create<ushort>();
                ProcessorAgent.Memory[address] = valueToOr;
                Registers.HL = address.ToShort();
            }
            else if(src.StartsWith("(I")) 
            {
                var address = Fixture.Create<ushort>();
                var offset = Fixture.Create<byte>();
                var realAddress = address.Add(offset.ToSignedByte());
                ProcessorAgent.Memory[realAddress] = valueToOr;
                SetMemoryContentsAt(2, offset);
                SetReg(src.Substring(1,2), address.ToShort());
            }
            else if(src != "A")
            {
                SetReg(src, valueToOr);
            }
        }

        [Test]
        [TestCaseSource(nameof(OR_r_Source))]
        public void OR_r_sets_SF_appropriately(string src, byte opcode, byte? prefix)
        {
            ExecuteCase(src, opcode, 0x00, 0xFF, prefix);
            Assert.That(Registers.SF.Value, Is.EqualTo(1));

            ExecuteCase(src, opcode, 0x00, 0x80, prefix);
            Assert.That(Registers.SF.Value, Is.EqualTo(1));

            ExecuteCase(src, opcode, 0x7F, 0x70, prefix);
            Assert.That(Registers.SF.Value, Is.EqualTo(0));
        }

        private void ExecuteCase(string src, byte opcode, int oldValue, int valueToAnd, byte? prefix)
        {
            Setup(src, (byte)oldValue, (byte)valueToAnd);
            Execute(opcode, prefix);
        }

        [Test]
        [TestCaseSource(nameof(OR_r_Source))]
        public void OR_r_sets_ZF_appropriately(string src, byte opcode, byte? prefix)
        {
            ExecuteCase(src, opcode, 0x00, 0x00, prefix);
            Assert.That(Registers.ZF.Value, Is.EqualTo(1));

            ExecuteCase(src, opcode, 0x01, 0x80, prefix);
            Assert.That(Registers.ZF.Value, Is.EqualTo(0));

            ExecuteCase(src, opcode, 0x7F, 0x7F, prefix);
            Assert.That(Registers.ZF.Value, Is.EqualTo(0));
        }

        [Test]
        [TestCaseSource(nameof(OR_r_Source))]
        public void OR_r_sets_PF_appropriately(string src, byte opcode, byte? prefix)
        {
            ExecuteCase(src, opcode, 0x00, 0x7E, prefix);
            Assert.That(Registers.PF.Value, Is.EqualTo(1));

            ExecuteCase(src, opcode, 0x00, 0x7F, prefix);
            Assert.That(Registers.PF.Value, Is.EqualTo(0));

            ExecuteCase(src, opcode, 0x00, 0x80, prefix);
            Assert.That(Registers.PF.Value, Is.EqualTo(0));

            ExecuteCase(src, opcode, 0x00, 0x81, prefix);
            Assert.That(Registers.PF.Value, Is.EqualTo(1));
        }

        [Test]
        [TestCaseSource(nameof(OR_r_Source))]
        [TestCaseSource(nameof(OR_A_Source))]
        public void OR_r_resets_NF_CF_HF(string src, byte opcode, byte? prefix)
        {
            AssertResetsFlags(opcode, null, "N", "C", "H");
        }

        [Test]
        [TestCaseSource(nameof(OR_r_Source))]
        public void OR_r_sets_bits_3_and_5_from_result(string src, byte opcode, byte? prefix)
        {
            var value = ((byte)0).WithBit(3, 1).WithBit(5, 0);
            Setup(src, value, 0);
            Execute(opcode, prefix);
            Assert.Multiple(() =>
            {
                Assert.That(Registers.Flag3.Value, Is.EqualTo(1));
                Assert.That(Registers.Flag5.Value, Is.EqualTo(0));
            });

            value = ((byte)0).WithBit(3, 0).WithBit(5, 1);
            Setup(src, value, 0);
            Execute(opcode, prefix);
            Assert.Multiple(() =>
            {
                Assert.That(Registers.Flag3.Value, Is.EqualTo(0));
                Assert.That(Registers.Flag5.Value, Is.EqualTo(1));
            });
        }

        [Test]
        [TestCaseSource(nameof(OR_r_Source))]
        [TestCaseSource(nameof(OR_A_Source))]
        public void OR_r_returns_proper_T_states(string src, byte opcode, byte? prefix)
        {
            var states = Execute(opcode, prefix);
            Assert.That(
states, Is.EqualTo((src == "(HL)" || src == "n") ? 7 :
                src.StartsWith("I") ? 8 :
                src.StartsWith(("(I")) ? 19 :
                4));
        }
    }
}