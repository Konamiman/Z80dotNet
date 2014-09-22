using System.Collections.Generic;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class ADD_A_r_tests : InstructionsExecutionTestsBase
    {
        static ADD_A_r_tests()
        {
            var combinations = new List<object[]>();

            var registers = new[] {"B", "C", "D", "E", "H", "L"};
            for(var src = 0; src<=5; src++)
            {
                var opcode = (byte) (src | 0x80);
                combinations.Add(new object[] {registers[src], opcode});
            }

            ADD_A_r_Source = combinations.ToArray();
        }

        public static object[] ADD_A_r_Source;

        [Test]
        [TestCaseSource("ADD_A_r_Source")]
        [TestCase("A", 0x87)]
        public void ADD_A_r_adds_both_registers(string src, byte opcode)
        {
            var oldValue = Fixture.Create<byte>();
            var valueAdded = src=="A" ? oldValue : Fixture.Create<byte>();

            Registers.A = oldValue;
            if(src != "A")
                SetReg(src, valueAdded);

            Execute(opcode);

            Assert.AreEqual(oldValue.Add(valueAdded), Registers.A);
        }

        [Test]
        [TestCaseSource("ADD_A_r_Source")]
        public void ADD_A_r_sets_SF_appropriately(string src, byte opcode)
        {
            Registers.A = 0xFD;
            SetReg(src, 1);

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
        [TestCaseSource("ADD_A_r_Source")]
        public void ADD_A_r_sets_ZF_appropriately(string src, byte opcode)
        {
            Registers.A = 0xFD;
            SetReg(src, 1);

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
        [TestCaseSource("ADD_A_r_Source")]
        public void ADD_A_r_sets_HF_appropriately(string src, byte opcode)
        {
            foreach(byte b in new byte[] { 0x0E, 0x7E, 0xFE }) 
            {
                Registers.A = b;
                SetReg(src, 1);

                Execute(opcode);
                Assert.AreEqual(0, Registers.HF);

                Execute(opcode);
                Assert.AreEqual(1, Registers.HF);

                Execute(opcode);
                Assert.AreEqual(0, Registers.HF);
            }
        }

        [Test]
        [TestCaseSource("ADD_A_r_Source")]
        public void ADD_A_r_sets_PF_appropriately(string src, byte opcode)
        {
            Registers.A = 0x7E;
            SetReg(src, 1);

            Execute(opcode);
            Assert.AreEqual(0, Registers.PF);

            Execute(opcode);
            Assert.AreEqual(1, Registers.PF);

            Execute(opcode);
            Assert.AreEqual(0, Registers.PF);
        }

        [Test]
        [TestCaseSource("ADD_A_r_Source")]
        [TestCase("A", 0x87)]
        public void ADD_A_r_resets_NF(string src, byte opcode)
        {
            AssertResetsFlags(opcode, null, "N");
        }

        [Test]
        [TestCaseSource("ADD_A_r_Source")]
        public void ADD_A_r_sets_CF_appropriately(string src, byte opcode)
        {
            Registers.A = 0xFE;
            SetReg(src, 1);

            Execute(opcode);
            Assert.AreEqual(0, Registers.CF);

            Execute(opcode);
            Assert.AreEqual(1, Registers.CF);

            Execute(opcode);
            Assert.AreEqual(0, Registers.CF);
        }

        [Test]
        [TestCaseSource("ADD_A_r_Source")]
        public void ADD_A_r_sets_bits_3_and_5_from_result(string src, byte opcode)
        {
            Registers.A = 0;
            SetReg(src, ((byte)0).WithBit(3, 1).WithBit(5, 0));
            Execute(opcode);
            Assert.AreEqual(1, Registers.Flag3);
            Assert.AreEqual(0, Registers.Flag5);

            Registers.A = 0;
            SetReg(src, ((byte)0).WithBit(3, 0).WithBit(5, 1));
            Execute(opcode);
            Assert.AreEqual(0, Registers.Flag3);
            Assert.AreEqual(1, Registers.Flag5);
        }

        [Test]
        [TestCaseSource("ADD_A_r_Source")]
        [TestCase("A", 0x87)]
        public void ADD_A_r_returns_proper_T_states(string src, byte opcode)
        {
            var states = Execute(opcode);
            Assert.AreEqual(4, states);
        }
    }
}