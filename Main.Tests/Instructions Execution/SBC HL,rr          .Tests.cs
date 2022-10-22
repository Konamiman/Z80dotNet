using NUnit.Framework;
using AutoFixture;


namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class SBC_HL_rr_tests : InstructionsExecutionTestsBase
    {
        private const int prefix = 0xED;

        public static object[] SBC_HL_rr_Source =
        {
            new object[] {"BC", (byte)0x42},
            new object[] {"DE", (byte)0x52},
            new object[] {"SP", (byte)0x72},
        };

        public static object[] SBC_HL_HL_Source =
        {
            new object[] {"HL", (byte)0x62}
        };

        [Test]
        [TestCaseSource("SBC_HL_rr_Source")]
        [TestCaseSource("SBC_HL_HL_Source")]
        public void SBC_HL_rr_substracts_both_registers_with_and_without_carry(string src, byte opcode)
        {
            for(var cf = 0; cf <= 1; cf++)
            {
                var value1 = Fixture.Create<short>();
                var value2 = (src == "HL") ? value1 : Fixture.Create<short>();

                Registers.HL = value1;
                Registers.CF = cf;
                if (src != "HL")
                    SetReg(src, value2);

                Execute(opcode, prefix);

                Assert.AreEqual(value1.Sub(value2).Sub((short) cf), (int)Registers.HL);
                if (src != "HL")
                    Assert.AreEqual(value2, (int)GetReg<short>(src));
            }
        }

        [Test]
        [TestCaseSource("SBC_HL_rr_Source")]
        public void SUB_HL_rr_sets_SF_appropriately(string src, byte opcode)
        {
            Setup(src, 0x02, 1);
            Execute(opcode, prefix);
            Assert.AreEqual(0, (int)Registers.SF);

            Setup(src, 0x01, 1);
            Execute(opcode, prefix);
            Assert.AreEqual(0, (int)Registers.SF);

            Setup(src, 0x00, 1);
            Execute(opcode, prefix);
            Assert.AreEqual(1, (int)Registers.SF);

            Setup(src, 0xFFFF.ToShort(), 1);
            Execute(opcode, prefix);
            Assert.AreEqual(1, (int)Registers.SF);
        }

        private void Setup(string src, short oldValue, short valueToSubstract)
        {
            Registers.HL = oldValue;
            Registers.CF = 0;

            if(src != "HL")
            {
                SetReg(src, valueToSubstract);
            }
        }

        [Test]
        [TestCaseSource("SBC_HL_rr_Source")]
        public void SUB_HL_rr_sets_ZF_appropriately(string src, byte opcode)
        {
            Setup(src, 0x03, 1);
            Execute(opcode, prefix);
            Assert.AreEqual(0, (int)Registers.ZF);

            Setup(src, 0x02, 1);
            Execute(opcode, prefix);
            Assert.AreEqual(0, (int)Registers.ZF);

            Setup(src, 0x01, 1);
            Execute(opcode, prefix);
            Assert.AreEqual(1, (int)Registers.ZF);

            Setup(src, 0x00, 1);
            Execute(opcode, prefix);
            Assert.AreEqual(0, (int)Registers.ZF);
        }

        [Test]
        [TestCaseSource("SBC_HL_rr_Source")]
        public void SUB_HL_rr_sets_HF_appropriately(string src, byte opcode)
        {
            foreach(int i in new int[] { 0x1001, 0x8001, 0xF001 })
            {
                short b = i.ToShort();

                Setup(src, b, 1);
                Execute(opcode, prefix);
                Assert.AreEqual(0, (int)Registers.HF);

                Setup(src, (byte)(b-1), 1);
                Execute(opcode, prefix);
                Assert.AreEqual(1, (int)Registers.HF);

                Setup(src, (byte)(b-2), 1);
                Execute(opcode, prefix);
                Assert.AreEqual(0, (int)Registers.HF);
            }
        }

        [Test]
        [TestCaseSource("SBC_HL_rr_Source")]
        public void SUB_HL_rr_sets_CF_appropriately(string src, byte opcode)
        {
            Setup(src, 0x01, 1);
            Execute(opcode, prefix);
            Assert.AreEqual(0, (int)Registers.CF);

            Setup(src, 0x00, 1);
            Execute(opcode, prefix);
            Assert.AreEqual(1, (int)Registers.CF);

            Setup(src, 0xFF, 1);
            Execute(opcode, prefix);
            Assert.AreEqual(0, (int)Registers.CF);
        }

        [Test]
        [TestCaseSource("SBC_HL_rr_Source")]
        public void SBC_HL_rr_sets_PF_appropriately(string src, byte opcode)
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
            Setup(src, NumberUtils.CreateShort(0, (byte)oldValue), NumberUtils.CreateShort(0, (byte)substractedValue));

            Execute(opcode, prefix);
            Assert.AreEqual(expectedPF, (int)Registers.PF);
        }

        [Test]
        [TestCaseSource("SBC_HL_rr_Source")]
        public void SBC_HL_rr_sets_NF(string src, byte opcode)
        {
            AssertSetsFlags(opcode, prefix, "N");
        }

        [Test]
        [TestCaseSource("SBC_HL_rr_Source")]
        public void SBC_HL_rr_sets_bits_3_and_5_from_high_byte_of_result(string src, byte opcode)
        {
            Registers.HL = NumberUtils.CreateShort(0, ((byte)0).WithBit(3, 1).WithBit(5, 0));
            SetReg(src, 0);
            Execute(opcode, prefix);
            Assert.AreEqual(1, (int)Registers.Flag3);
            Assert.AreEqual(0, (int)Registers.Flag5);

            Registers.HL = NumberUtils.CreateShort(0, ((byte)0).WithBit(3, 0).WithBit(5, 1));
            Execute(opcode, prefix);
            Assert.AreEqual(0, (int)Registers.Flag3);
            Assert.AreEqual(1, (int)Registers.Flag5);
        }

        [Test]
        [TestCaseSource("SBC_HL_rr_Source")]
        public void SBC_HL_rr_returns_proper_T_states(string src, byte opcode)
        {
            var states = Execute(opcode, prefix);
            Assert.AreEqual(15, states);
        }
    }
}
