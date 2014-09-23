using System.Collections.Generic;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class ADDC_A_r_tests : InstructionsExecutionTestsBase
    {
        static ADDC_A_r_tests()
        {
            var combinations = new List<object[]>();

            var registers = new[] {"B", "C", "D", "E", "H", "L", "(HL)", "n"};
            for(var src = 0; src<=7; src++)
            {
                var ADD_opcode = (byte)(src==7 ? 0xC6 : (src | 0x80));
                var ADC_opcode = (byte)(src==7 ? 0xCE : (src | 0x88));
                combinations.Add(new object[] {registers[src], ADD_opcode, 0});
                combinations.Add(new object[] {registers[src], ADC_opcode, 0});
                combinations.Add(new object[] {registers[src], ADC_opcode, 1});
            }

            ADDC_A_r_Source = combinations.ToArray();
        }

        public static object[] ADDC_A_r_Source;

        public static object[] ADDC_A_A_Source =
        {
            new object[] {"A", (byte)0x87, 0},
            new object[] {"A", (byte)0x8F, 0},
            new object[] {"A", (byte)0x8F, 1},
        };


        [Test]
        [TestCaseSource("ADDC_A_r_Source")]
        [TestCaseSource("ADDC_A_A_Source")]
        public void ADDC_A_r_adds_both_registers_with_or_without_carry(string src, byte opcode, int cf)
        {
            var oldValue = Fixture.Create<byte>();
            var valueToAdd = src=="A" ? oldValue : Fixture.Create<byte>();

            Setup(src, oldValue, valueToAdd, cf);
            Execute(opcode);

            Assert.AreEqual(oldValue.Add(valueToAdd + cf), Registers.A);
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
            else if(src != "A")
            {
                SetReg(src, valueToAdd);
            }
        }

        [Test]
        [TestCaseSource("ADDC_A_r_Source")]
        public void ADDC_A_r_sets_SF_appropriately(string src, byte opcode, int cf)
        {
            Setup(src, 0xFD, 1);

            Execute(opcode);
            Assert.AreEqual(1, Registers.SF);

            Execute(opcode);
            Assert.AreEqual(1, Registers.SF);

            Execute(opcode);
            Assert.AreEqual(0, Registers.SF);

            Execute(opcode);
            Assert.AreEqual(0, Registers.SF);
        }

        [Test]
        [TestCaseSource("ADDC_A_r_Source")]
        public void ADDC_A_r_sets_ZF_appropriately(string src, byte opcode, int cf)
        {
            Setup(src, 0xFD, 1);

            Execute(opcode);
            Assert.AreEqual(0, Registers.ZF);

            Execute(opcode);
            Assert.AreEqual(0, Registers.ZF);

            Execute(opcode);
            Assert.AreEqual(1, Registers.ZF);

            Execute(opcode);
            Assert.AreEqual(0, Registers.ZF);
        }

        [Test]
        [TestCaseSource("ADDC_A_r_Source")]
        public void ADDC_A_r_sets_HF_appropriately(string src, byte opcode, int cf)
        {
            foreach(byte b in new byte[] { 0x0E, 0x7E, 0xFE }) 
            {
                Setup(src, b, 1);

                Execute(opcode);
                Assert.AreEqual(0, Registers.HF);

                Execute(opcode);
                Assert.AreEqual(1, Registers.HF);

                Execute(opcode);
                Assert.AreEqual(0, Registers.HF);
            }
        }

        [Test]
        [TestCaseSource("ADDC_A_r_Source")]
        public void ADDC_A_r_sets_PF_appropriately(string src, byte opcode, int cf)
        {
            Setup(src, 0x7E, 1);

            Execute(opcode);
            Assert.AreEqual(0, Registers.PF);

            Execute(opcode);
            Assert.AreEqual(1, Registers.PF);

            Execute(opcode);
            Assert.AreEqual(0, Registers.PF);
        }

        [Test]
        [TestCaseSource("ADDC_A_r_Source")]
        [TestCaseSource("ADDC_A_A_Source")]
        public void ADDC_A_r_resets_NF(string src, byte opcode, int cf)
        {
            AssertResetsFlags(opcode, null, "N");
        }

        [Test]
        [TestCaseSource("ADDC_A_r_Source")]
        public void ADDC_A_r_sets_CF_appropriately(string src, byte opcode, int cf)
        {
            Setup(src, 0xFE, 1);

            Execute(opcode);
            Assert.AreEqual(0, Registers.CF);

            Execute(opcode);
            Assert.AreEqual(1, Registers.CF);

            Execute(opcode);
            Assert.AreEqual(0, Registers.CF);
        }

        [Test]
        [TestCaseSource("ADDC_A_r_Source")]
        public void ADDC_A_r_sets_bits_3_and_5_from_result(string src, byte opcode, int cf)
        {
            Setup(src, ((byte)0).WithBit(3, 1).WithBit(5, 0), 0);
            Execute(opcode);
            Assert.AreEqual(1, Registers.Flag3);
            Assert.AreEqual(0, Registers.Flag5);

            Setup(src, ((byte)0).WithBit(3, 0).WithBit(5, 1), 0);
            Execute(opcode);
            Assert.AreEqual(0, Registers.Flag3);
            Assert.AreEqual(1, Registers.Flag5);
        }

        [Test]
        [TestCaseSource("ADDC_A_r_Source")]
        [TestCaseSource("ADDC_A_A_Source")]
        public void ADDC_A_r_returns_proper_T_states(string src, byte opcode, int cf)
        {
            var states = Execute(opcode);
            Assert.AreEqual(src == "(HL)" || src == "n" ? 7 : 4, states);
        }
    }
}