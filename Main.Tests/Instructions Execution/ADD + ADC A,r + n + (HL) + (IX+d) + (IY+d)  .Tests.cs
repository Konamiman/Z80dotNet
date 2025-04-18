﻿using NUnit.Framework;
using AutoFixture;
using System.Collections.Generic;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class ADDC_A_r_tests : InstructionsExecutionTestsBase
    {
        static ADDC_A_r_tests()
        {
            var combinations = new List<object[]>();

            var registers = new[] {"B", "C", "D", "E", "H", "L", "(HL)", "n", "IXH", "IXL", "IYH", "IYL","(IX+n)","(IY+n)"};
            for(var src = 0; src<registers.Length; src++)
            {
                var reg = registers[src];
                var i = src;
                byte? prefix = null;

                ModifyTestCaseCreationForIndexRegs(reg, ref i, out prefix);

                var ADD_opcode = (byte)(i==7 ? 0xC6 : (i | 0x80));
                var ADC_opcode = (byte)(i==7 ? 0xCE : (i | 0x88));
                combinations.Add(new object[] {reg, ADD_opcode, 0, prefix});
                combinations.Add(new object[] {reg, ADC_opcode, 0, prefix});
                combinations.Add(new object[] {reg, ADC_opcode, 1, prefix});
            }

            ADDC_A_r_Source = combinations.ToArray();
        }

        public static object[] ADDC_A_r_Source;

        public static object[] ADDC_A_A_Source =
        {
            new object[] {"A", (byte)0x87, 0, (byte?)null},
            new object[] {"A", (byte)0x8F, 0, (byte?)null},
            new object[] {"A", (byte)0x8F, 1, (byte?)null}
        };


        [Test]
        [TestCaseSource(nameof(ADDC_A_r_Source))]
        [TestCaseSource(nameof(ADDC_A_A_Source))]
        public void ADDC_A_r_adds_both_registers_with_or_without_carry(string src, byte opcode, int cf, byte? prefix)
        {
            var oldValue = Fixture.Create<byte>();
            var valueToAdd = src=="A" ? oldValue : Fixture.Create<byte>();

            Setup(src, oldValue, valueToAdd, cf);
            Execute(opcode, prefix);

            Assert.That(Registers.A, Is.EqualTo(oldValue.Add(valueToAdd + cf)));
        }

        private void Setup(string src, byte oldValue, byte valueToAdd, int cf = 0)
        {
            Registers.A = oldValue;
            Registers.CF = cf;

            if(src == "n") 
            {
                SetMemoryContentsAt(1, valueToAdd);
            }
            else if(src == "(HL)") 
            {
                var address = Fixture.Create<ushort>();
                ProcessorAgent.Memory[address] = valueToAdd;
                Registers.HL = address.ToShort();
            }
            else if(src.StartsWith("(I")) 
            {
                var address = Fixture.Create<ushort>();
                var offset = Fixture.Create<byte>();
                var realAddress = address.Add(offset.ToSignedByte());
                ProcessorAgent.Memory[realAddress] = valueToAdd;
                SetMemoryContentsAt(2, offset);
                SetReg(src.Substring(1,2), address.ToShort());
            }
            else if(src != "A")
            {
                SetReg(src, valueToAdd);
            }
        }

        [Test]
        [TestCaseSource(nameof(ADDC_A_r_Source))]
        public void ADDC_A_r_sets_SF_appropriately(string src, byte opcode, int cf, byte? prefix)
        {
            Setup(src, 0xFD, 1);

            Execute(opcode, prefix);
            Assert.That(Registers.SF.Value, Is.EqualTo(1));

            Execute(opcode, prefix);
            Assert.That(Registers.SF.Value, Is.EqualTo(1));

            Execute(opcode, prefix);
            Assert.That(Registers.SF.Value, Is.EqualTo(0));

            Execute(opcode, prefix);
            Assert.That(Registers.SF.Value, Is.EqualTo(0));
        }

        [Test]
        [TestCaseSource(nameof(ADDC_A_r_Source))]
        public void ADDC_A_r_sets_ZF_appropriately(string src, byte opcode, int cf, byte? prefix)
        {
            Setup(src, 0xFD, 1);

            Execute(opcode, prefix);
            Assert.That(Registers.ZF.Value, Is.EqualTo(0));

            Execute(opcode, prefix);
            Assert.That(Registers.ZF.Value, Is.EqualTo(0));

            Execute(opcode, prefix);
            Assert.That(Registers.ZF.Value, Is.EqualTo(1));

            Execute(opcode, prefix);
            Assert.That(Registers.ZF.Value, Is.EqualTo(0));
        }

        [Test]
        [TestCaseSource(nameof(ADDC_A_r_Source))]
        public void ADDC_A_r_sets_HF_appropriately(string src, byte opcode, int cf, byte? prefix)
        {
            foreach(byte b in new byte[] { 0x0E, 0x7E, 0xFE }) 
            {
                Setup(src, b, 1);

                Execute(opcode, prefix);
                Assert.That(Registers.HF.Value, Is.EqualTo(0));

                Execute(opcode, prefix);
                Assert.That(Registers.HF.Value, Is.EqualTo(1));

                Execute(opcode, prefix);
                Assert.That(Registers.HF.Value, Is.EqualTo(0));
            }
        }

        [Test]
        [TestCaseSource(nameof(ADDC_A_r_Source))]
        public void ADDC_A_r_sets_PF_appropriately(string src, byte opcode, int cf, byte? prefix)
        {
            Setup(src, 0x7E, 1);

            Execute(opcode, prefix);
            Assert.That(Registers.PF.Value, Is.EqualTo(0));

            Execute(opcode, prefix);
            Assert.That(Registers.PF.Value, Is.EqualTo(1));

            Execute(opcode, prefix);
            Assert.That(Registers.PF.Value, Is.EqualTo(0));
        }

        [Test]
        [TestCaseSource(nameof(ADDC_A_r_Source))]
        [TestCaseSource(nameof(ADDC_A_A_Source))]
        public void ADDC_A_r_resets_NF(string src, byte opcode, int cf, byte? prefix)
        {
            AssertResetsFlags(opcode, null, "N");
        }

        [Test]
        [TestCaseSource(nameof(ADDC_A_r_Source))]
        public void ADDC_A_r_sets_CF_appropriately(string src, byte opcode, int cf, byte? prefix)
        {
            Setup(src, 0xFE, 1);

            Execute(opcode, prefix);
            Assert.That(Registers.CF.Value, Is.EqualTo(0));

            Execute(opcode, prefix);
            Assert.That(Registers.CF.Value, Is.EqualTo(1));

            Execute(opcode, prefix);
            Assert.That(Registers.CF.Value, Is.EqualTo(0));
        }

        [Test]
        [TestCaseSource(nameof(ADDC_A_r_Source))]
        public void ADDC_A_r_sets_bits_3_and_5_from_result(string src, byte opcode, int cf, byte? prefix)
        {
            Setup(src, ((byte)0).WithBit(3, 1).WithBit(5, 0), 0);
            Execute(opcode, prefix);
            Assert.Multiple(() =>
            {
                Assert.That(Registers.Flag3.Value, Is.EqualTo(1));
                Assert.That(Registers.Flag5.Value, Is.EqualTo(0));
            });

            Setup(src, ((byte)0).WithBit(3, 0).WithBit(5, 1), 0);
            Execute(opcode, prefix);
            Assert.Multiple(() =>
            {
                Assert.That(Registers.Flag3.Value, Is.EqualTo(0));
                Assert.That(Registers.Flag5.Value, Is.EqualTo(1));
            });
        }

        [Test]
        [TestCaseSource(nameof(ADDC_A_r_Source))]
        [TestCaseSource(nameof(ADDC_A_A_Source))]
        public void ADDC_A_r_returns_proper_T_states(string src, byte opcode, int cf, byte? prefix)
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