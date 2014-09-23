using System.Collections.Generic;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class SUBC_A_r_tests : InstructionsExecutionTestsBase
    {
        static SUBC_A_r_tests()
        {
            var combinations = new List<object[]>();

            var registers = new[] {"B", "C", "D", "E", "H", "L", "(HL)", "n"};
            for(var src = 0; src<=6; src++)
            {
                var SUB_opcode = (byte)(src==7 ? 0xD6 : (src | 0x90));
                var SBC_opcode = (byte)(src==7 ? 0xDE : (src | 0x98));
                combinations.Add(new object[] {registers[src], SUB_opcode, 0});
                combinations.Add(new object[] {registers[src], SBC_opcode, 0});
                combinations.Add(new object[] {registers[src], SBC_opcode, 1});
            }

            SUBC_A_r_Source = combinations.ToArray();
        }

        public static object[] SUBC_A_r_Source;

        public static object[] SUBC_A_A_Source =
        {
            new object[] {"A", (byte)0x97, 0},
            new object[] {"A", (byte)0x9F, 0},
            new object[] {"A", (byte)0x9F, 1},
        };


        [Test]
        [TestCaseSource("SUBC_A_r_Source")]
        [TestCaseSource("SUBC_A_A_Source")]
        public void SUBC_A_r_substracts_both_registers_with_or_without_carry(string src, byte opcode, int cf)
        {
            var oldValue = Fixture.Create<byte>();
            var valueToAdd = src=="A" ? oldValue : Fixture.Create<byte>();

            Setup(src, oldValue, valueToAdd, cf);
            Execute(opcode);

            Assert.AreEqual(oldValue.Sub(valueToAdd + cf), Registers.A);
        }

        private void Setup(string src, byte oldValue, byte valueToSubstract, int cf = 0)
        {
            Registers.A = oldValue;
            Registers.CF = cf;

            if(src == "n") 
            {
                SetMemoryContentsAt(1, valueToSubstract);
            }
            else if(src == "(HL)") 
            {
                var address = Fixture.Create<ushort>();
                ProcessorAgent.Memory[address] = valueToSubstract;
                Registers.HL = address.ToShort();
            }
            else if(src != "A")
            {
                SetReg(src, valueToSubstract);
            }
        }

        [Test]
        [TestCaseSource("SUBC_A_r_Source")]
        public void SUBC_A_r_sets_SF_appropriately(string src, byte opcode, int cf)
        {
            Setup(src, 0x02, 1);

            Execute(opcode);
            Assert.AreEqual(0, Registers.SF);

            Execute(opcode);
            Assert.AreEqual(0, Registers.SF);

            Execute(opcode);
            Assert.AreEqual(1, Registers.SF);

            Execute(opcode);
            Assert.AreEqual(1, Registers.SF);
        }

        [Test]
        [TestCaseSource("SUBC_A_r_Source")]
        public void SUBC_A_r_sets_ZF_appropriately(string src, byte opcode, int cf)
        {
            Setup(src, 0x03, 1);

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
        [TestCaseSource("SUBC_A_r_Source")]
        public void SUBC_A_r_sets_HF_appropriately(string src, byte opcode, int cf)
        {
            foreach(byte b in new byte[] { 0x11, 0x81, 0xF1 }) 
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
        [TestCaseSource("SUBC_A_r_Source")]
        public void SUBC_A_r_sets_PF_appropriately(string src, byte opcode, int cf)
        {
            //http://stackoverflow.com/a/8037485/4574

            TestPF(src, opcode, 127, 0, 0);
            TestPF(src, opcode, 127, 1, 0);
            TestPF(src, opcode, 127, 127, 0);
            TestPF(src, opcode, 127, 128, 1);
            TestPF(src, opcode, 127, 129, 1);
            TestPF(src, opcode, 127, 255, 1);
            TestPF(src, opcode, 128, 0, 0);
            TestPF(src, opcode, 128, 1, 1);
            TestPF(src, opcode, 128, 127, 1);
            TestPF(src, opcode, 128, 128, 0);
            TestPF(src, opcode, 128, 129, 0);
            TestPF(src, opcode, 128, 255, 0);
            TestPF(src, opcode, 129, 0, 0);
            TestPF(src, opcode, 129, 1, 0);
            TestPF(src, opcode, 129, 127, 1);
            TestPF(src, opcode, 129, 128, 0);
            TestPF(src, opcode, 129, 129, 0);
            TestPF(src, opcode, 129, 255, 0);
        }

        void TestPF(string src, byte opcode, int oldValue, int substractedValue, int expectedPF)
        {
            Setup(src, (byte)oldValue, (byte)substractedValue);

            Execute(opcode);
            Assert.AreEqual(expectedPF, Registers.PF);
        }

        [Test]
        [TestCaseSource("SUBC_A_r_Source")]
        [TestCaseSource("SUBC_A_A_Source")]
        public void SUBC_A_r_sets_NF(string src, byte opcode, int cf)
        {
            AssertSetsFlags(opcode, null, "N");
        }

        [Test]
        [TestCaseSource("SUBC_A_r_Source")]
        public void SUBC_A_r_sets_CF_appropriately(string src, byte opcode, int cf)
        {
            Setup(src, 0x01, 1);

            Execute(opcode);
            Assert.AreEqual(0, Registers.CF);

            Execute(opcode);
            Assert.AreEqual(1, Registers.CF);

            Execute(opcode);
            Assert.AreEqual(0, Registers.CF);
        }

        [Test]
        [TestCaseSource("SUBC_A_r_Source")]
        public void SUBC_A_r_sets_bits_3_and_5_from_result(string src, byte opcode, int cf)
        {
            Setup(src, (byte)(((byte)0).WithBit(3, 1).WithBit(5, 0) + 1), 1);
            Execute(opcode);
            Assert.AreEqual(1, Registers.Flag3);
            Assert.AreEqual(0, Registers.Flag5);

            Setup(src, (byte)(((byte)0).WithBit(3, 0).WithBit(5, 1) + 1), 1);
            Execute(opcode);
            Assert.AreEqual(0, Registers.Flag3);
            Assert.AreEqual(1, Registers.Flag5);
        }

        [Test]
        [TestCaseSource("SUBC_A_r_Source")]
        [TestCaseSource("SUBC_A_A_Source")]
        public void SUBC_A_r_returns_proper_T_states(string src, byte opcode, int cf)
        {
            var states = Execute(opcode);
            Assert.AreEqual(src == "(HL)" ? 7 : 4, states);
        }
    }
}