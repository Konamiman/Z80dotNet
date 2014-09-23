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

            var registers = new[] {"B", "C", "D", "E", "H", "L"};
            for(var src = 0; src<=5; src++)
            {
                var ADD_opcode = (byte) (src | 0x80);
                var ADC_opcode = (byte) (src | 0x88);
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
            var valueAdded = src=="A" ? oldValue : Fixture.Create<byte>();

            Registers.CF = cf;
            Registers.A = oldValue;
            if(src != "A")
                SetReg(src, valueAdded);

            Execute(opcode);

            Assert.AreEqual(oldValue.Add(valueAdded + cf), Registers.A);
        }

        [Test]
        [TestCaseSource("ADDC_A_r_Source")]
        public void ADDC_A_r_sets_SF_appropriately(string src, byte opcode, int cf)
        {
            Registers.A = 0xFD;
            SetReg(src, 1);

            ExecuteWithNoCF(opcode);
            Assert.AreEqual(1, Registers.SF);

            ExecuteWithNoCF(opcode);
            Assert.AreEqual(1, Registers.SF);

            ExecuteWithNoCF(opcode);
            Assert.AreEqual(0, Registers.SF);

            ExecuteWithNoCF(opcode);
            Assert.AreEqual(0, Registers.SF);
        }

        [Test]
        [TestCaseSource("ADDC_A_r_Source")]
        public void ADDC_A_r_sets_ZF_appropriately(string src, byte opcode, int cf)
        {
            Registers.A = 0xFD;
            SetReg(src, 1);

            ExecuteWithNoCF(opcode);
            Assert.AreEqual(0, Registers.ZF);

            ExecuteWithNoCF(opcode);
            Assert.AreEqual(0, Registers.ZF);

            ExecuteWithNoCF(opcode);
            Assert.AreEqual(1, Registers.ZF);

            ExecuteWithNoCF(opcode);
            Assert.AreEqual(0, Registers.ZF);
        }

        [Test]
        [TestCaseSource("ADDC_A_r_Source")]
        public void ADDC_A_r_sets_HF_appropriately(string src, byte opcode, int cf)
        {
            foreach(byte b in new byte[] { 0x0E, 0x7E, 0xFE }) 
            {
                Registers.A = b;
                SetReg(src, 1);

                ExecuteWithNoCF(opcode);
                Assert.AreEqual(0, Registers.HF);

                ExecuteWithNoCF(opcode);
                Assert.AreEqual(1, Registers.HF);

                ExecuteWithNoCF(opcode);
                Assert.AreEqual(0, Registers.HF);
            }
        }

        [Test]
        [TestCaseSource("ADDC_A_r_Source")]
        public void ADDC_A_r_sets_PF_appropriately(string src, byte opcode, int cf)
        {
            Registers.A = 0x7E;
            SetReg(src, 1);

            ExecuteWithNoCF(opcode);
            Assert.AreEqual(0, Registers.PF);

            ExecuteWithNoCF(opcode);
            Assert.AreEqual(1, Registers.PF);

            ExecuteWithNoCF(opcode);
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
            Registers.A = 0xFE;
            SetReg(src, 1);

            ExecuteWithNoCF(opcode);
            Assert.AreEqual(0, Registers.CF);

            ExecuteWithNoCF(opcode);
            Assert.AreEqual(1, Registers.CF);

            ExecuteWithNoCF(opcode);
            Assert.AreEqual(0, Registers.CF);
        }

        [Test]
        [TestCaseSource("ADDC_A_r_Source")]
        public void ADDC_A_r_sets_bits_3_and_5_from_result(string src, byte opcode, int cf)
        {
            Registers.A = 0;
            SetReg(src, ((byte)0).WithBit(3, 1).WithBit(5, 0));
            ExecuteWithNoCF(opcode);
            Assert.AreEqual(1, Registers.Flag3);
            Assert.AreEqual(0, Registers.Flag5);

            Registers.A = 0;
            SetReg(src, ((byte)0).WithBit(3, 0).WithBit(5, 1));
            ExecuteWithNoCF(opcode);
            Assert.AreEqual(0, Registers.Flag3);
            Assert.AreEqual(1, Registers.Flag5);
        }

        [Test]
        [TestCaseSource("ADDC_A_r_Source")]
        [TestCaseSource("ADDC_A_A_Source")]
        public void ADDC_A_r_returns_proper_T_states(string src, byte opcode, int cf)
        {
            var states = Execute(opcode);
            Assert.AreEqual(4, states);
        }

        void ExecuteWithNoCF(byte opcode)
        {
            Registers.CF = 0;
            Execute(opcode);
        }
    }
}