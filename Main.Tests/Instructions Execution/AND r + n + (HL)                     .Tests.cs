﻿using NUnit.Framework;
using AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class AND_r_tests : InstructionsExecutionTestsBase
    {
        static AND_r_tests()
        {
            var combinations = new List<object[]>();

            var registers = new[] {"B", "C", "D", "E", "H", "L", "(HL)", "n", "IXH", "IXL", "IYH", "IYL","(IX+n)","(IY+n)"};
            for(var src = 0; src<registers.Length; src++)
            {
                var reg = registers[src];
                var i = src;
                byte? prefix = null;

                ModifyTestCaseCreationForIndexRegs(reg, ref i, out prefix);

                var opcode = (byte)(i==7 ? 0xE6 : (i | 0xA0));
                combinations.Add(new object[] {reg, opcode, prefix});
            }

            AND_r_Source = combinations.ToArray();
        }

        public static object[] AND_r_Source;

        public static object[] AND_A_Source =
        {
            new object[] {"A", (byte)0xA7, null},
        };


        [Test]
        [TestCaseSource("AND_r_Source")]
        public void AND_r_ands_both_registers(string src, byte opcode, byte? prefix)
        {
            var oldValue = Fixture.Create<byte>();
            var valueToAnd = Fixture.Create<byte>();

            Setup(src, oldValue, valueToAnd);
            Execute(opcode, prefix);

            Assert.AreEqual(oldValue & valueToAnd, (int)Registers.A);
        }

        [Test]
        [TestCaseSource("AND_A_Source")]
        public void AND_A_does_not_change_A(string src, byte opcode, byte? prefix)
        {
            var value = Fixture.Create<byte>();

            Registers.A = value;
            Execute(opcode, prefix);

            Assert.AreEqual(value, (int)Registers.A);
        }

        private void Setup(string src, byte oldValue, byte valueToAnd)
        {
            Registers.A = oldValue;

            if(src == "n") 
            {
                SetMemoryContentsAt(1, valueToAnd);
            }
            else if(src == "(HL)") 
            {
                var address = Fixture.Create<ushort>();
                ProcessorAgent.Memory[address] = valueToAnd;
                Registers.HL = address.ToShort();
            }
            else if(src.StartsWith("(I")) 
            {
                var address = Fixture.Create<ushort>();
                var offset = Fixture.Create<byte>();
                var realAddress = address.Add(offset.ToSignedByte());
                ProcessorAgent.Memory[realAddress] = valueToAnd;
                SetMemoryContentsAt(2, offset);
                SetReg(src.Substring(1,2), address.ToShort());
            }
            else if(src != "A")
            {
                SetReg(src, valueToAnd);
            }
        }

        [Test]
        [TestCaseSource("AND_r_Source")]
        public void AND_r_sets_SF_appropriately(string src, byte opcode, byte? prefix)
        {
            ExecuteCase(src, opcode, 0xFF, 0xFF, prefix);
            Assert.AreEqual(1, (int)Registers.SF);

            ExecuteCase(src, opcode, 0xFF, 0x80, prefix);
            Assert.AreEqual(1, (int)Registers.SF);

            ExecuteCase(src, opcode, 0xFF, 0, prefix);
            Assert.AreEqual(0, (int)Registers.SF);
        }

        private void ExecuteCase(string src, byte opcode, int oldValue, int valueToAnd, byte? prefix)
        {
            Setup(src, (byte)oldValue, (byte)valueToAnd);
            Execute(opcode, prefix);
        }

        [Test]
        [TestCaseSource("AND_r_Source")]
        public void AND_r_sets_ZF_appropriately(string src, byte opcode, byte? prefix)
        {
            ExecuteCase(src, opcode, 0xFF, 0xFF, prefix);
            Assert.AreEqual(0, (int)Registers.ZF);

            ExecuteCase(src, opcode, 0xFF, 0x80, prefix);
            Assert.AreEqual(0, (int)Registers.ZF);

            ExecuteCase(src, opcode, 0xFF, 0, prefix);
            Assert.AreEqual(1, (int)Registers.ZF);
        }

        [Test]
        [TestCaseSource("AND_r_Source")]
        public void AND_r_sets_HF(string src, byte opcode, byte? prefix)
        {
            AssertSetsFlags(opcode, null, "H");
        }

        [Test]
        [TestCaseSource("AND_r_Source")]
        public void AND_r_sets_PF_appropriately(string src, byte opcode, byte? prefix)
        {
            ExecuteCase(src, opcode, 0xFF, 0x7E, prefix);
            Assert.AreEqual(1, (int)Registers.PF);

            ExecuteCase(src, opcode, 0xFF, 0x7F, prefix);
            Assert.AreEqual(0, (int)Registers.PF);

            ExecuteCase(src, opcode, 0xFF, 0x80, prefix);
            Assert.AreEqual(0, (int)Registers.PF);

            ExecuteCase(src, opcode, 0xFF, 0x81, prefix);
            Assert.AreEqual(1, (int)Registers.PF);
        }

        [Test]
        [TestCaseSource("AND_r_Source")]
        [TestCaseSource("AND_A_Source")]
        public void AND_r_resets_NF_and_CF(string src, byte opcode, byte? prefix)
        {
            AssertResetsFlags(opcode, null, "N", "C");
        }

        [Test]
        [TestCaseSource("AND_r_Source")]
        public void AND_r_sets_bits_3_and_5_from_result(string src, byte opcode, byte? prefix)
        {
            var value = ((byte)0).WithBit(3, 1).WithBit(5, 0);
            Setup(src, value, value);
            Execute(opcode, prefix);
            Assert.AreEqual(1, (int)Registers.Flag3);
            Assert.AreEqual(0, (int)Registers.Flag5);

            value = ((byte)0).WithBit(3, 0).WithBit(5, 1);
            Setup(src, value, value);
            Execute(opcode, prefix);
            Assert.AreEqual(0, (int)Registers.Flag3);
            Assert.AreEqual(1, (int)Registers.Flag5);
        }

        [Test]
        [TestCaseSource("AND_r_Source")]
        [TestCaseSource("AND_A_Source")]
        public void AND_r_returns_proper_T_states(string src, byte opcode, byte? prefix)
        {
            var states = Execute(opcode, prefix);
            Assert.AreEqual(
                (src == "(HL)" || src == "n") ? 7 :
                src.StartsWith("I") ? 8 :
                src.StartsWith(("(I")) ? 19 :
                4, states);
        }
    }
}