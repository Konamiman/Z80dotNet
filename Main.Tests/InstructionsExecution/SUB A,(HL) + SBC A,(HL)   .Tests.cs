using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class SUBC_A_aHL_tests : InstructionsExecutionTestsBase
    {
        private const byte SUB_A_aHL_opcode = 0x96;
        private const byte SBC_A_aHL_opcode = 0x9E;

        public static object[] SUBC_A_aHL_Source =
        {
            new object[] {SUB_A_aHL_opcode, 0},
            new object[] {SBC_A_aHL_opcode, 0},
            new object[] {SBC_A_aHL_opcode, 1}
        };

        [Test]
        [TestCaseSource("SUBC_A_aHL_Source")]
        public void SUBC_A_aHL_substracts_values_appropriately_with_or_without_CF(byte opcode, int cf)
        {
            var oldValue = Fixture.Create<byte>();
            var valueSubstracted = Fixture.Create<byte>();

            Setup(oldValue, valueSubstracted, cf);
            Execute(opcode);

            Assert.AreEqual(oldValue.Sub(valueSubstracted + cf), Registers.A);
        }

        private void Setup(byte oldValue, byte valueToAdd, int cf = 0)
        {
            Registers.A = oldValue;
            Registers.CF = cf;
            var address = Fixture.Create<ushort>();
            ProcessorAgent.Memory[address] = valueToAdd;
            Registers.HL = address.ToShort();
        }

        [Test]
        [TestCaseSource("SUBC_A_aHL_Source")]
        public void SUBC_A_aHL_sets_SF_appropriately(byte opcode, int cf)
        {
            Setup(0x02, 1);

            ExecuteWithNoCF(opcode);
            Assert.AreEqual(0, Registers.SF);

            ExecuteWithNoCF(opcode);
            Assert.AreEqual(0, Registers.SF);

            ExecuteWithNoCF(opcode);
            Assert.AreEqual(1, Registers.SF);

            ExecuteWithNoCF(opcode);
            Assert.AreEqual(1, Registers.SF);
        }

        [Test]
        [TestCaseSource("SUBC_A_aHL_Source")]
        public void SUBC_A_aHL_sets_ZF_appropriately(byte opcode, int cf)
        {
            Setup(0x03, 1);

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
        [TestCaseSource("SUBC_A_aHL_Source")]
        public void SUBC_A_aHL_sets_HF_appropriately(byte opcode, int cf)
        {
            foreach(byte b in new byte[] { 0x11, 0x81, 0xF1 }) 
            {
                Setup(b, 1);

                ExecuteWithNoCF(opcode);
                Assert.AreEqual(0, Registers.HF);

                ExecuteWithNoCF(opcode);
                Assert.AreEqual(1, Registers.HF);

                ExecuteWithNoCF(opcode);
                Assert.AreEqual(0, Registers.HF);
            }
        }

        [Test]
        [TestCaseSource("SUBC_A_aHL_Source")]
        public void SUBC_A_aHL_sets_PF_appropriately(byte opcode, int cf)
        {
            //http://stackoverflow.com/a/8037485/4574

            TestPF(127, 0, 0);
            TestPF(127, 1, 0);
            TestPF(127, 127, 0);
            TestPF(127, 128, 1);
            TestPF(127, 129, 1);
            TestPF(127, 255, 1);
            TestPF(128, 0, 0);
            TestPF(128, 1, 1);
            TestPF(128, 127, 1);
            TestPF(128, 128, 0);
            TestPF(128, 129, 0);
            TestPF(128, 255, 0);
            TestPF(129, 0, 0);
            TestPF(129, 1, 0);
            TestPF(129, 127, 1);
            TestPF(129, 128, 0);
            TestPF(129, 129, 0);
            TestPF(129, 255, 0);
        }

        void TestPF(int oldValue, int substractedValue, int expectedPF)
        {
            Setup((byte)oldValue, (byte)substractedValue);

            Execute(SUB_A_aHL_opcode);
            Assert.AreEqual(expectedPF, Registers.PF);
        }

        [Test]
        [TestCaseSource("SUBC_A_aHL_Source")]
        public void SUBC_A_aHL_sets_NF(byte opcode, int cf)
        {
            AssertSetsFlags(SUB_A_aHL_opcode, null, "N");
        }

        [Test]
        [TestCaseSource("SUBC_A_aHL_Source")]
        public void SUBC_A_aHL_sets_CF_appropriately(byte opcode, int cf)
        {
            Setup(0x01, 1);

            ExecuteWithNoCF(opcode);
            Assert.AreEqual(0, Registers.CF);

            ExecuteWithNoCF(opcode);
            Assert.AreEqual(1, Registers.CF);

            ExecuteWithNoCF(opcode);
            Assert.AreEqual(0, Registers.CF);
        }

        [Test]
        [TestCaseSource("SUBC_A_aHL_Source")]
        public void SUBC_A_aHL_sets_bits_3_and_5_from_result(byte opcode, int cf)
        {
            Setup((byte)(((byte)0).WithBit(3, 1).WithBit(5, 0) + 1), 1);
            ExecuteWithNoCF(opcode);
            Assert.AreEqual(1, Registers.Flag3);
            Assert.AreEqual(0, Registers.Flag5);

            Setup((byte)(((byte)0).WithBit(3, 0).WithBit(5, 1) + 1), 1);
            ExecuteWithNoCF(opcode);
            Assert.AreEqual(0, Registers.Flag3);
            Assert.AreEqual(1, Registers.Flag5);
        }

        [Test]
        [TestCaseSource("SUBC_A_aHL_Source")]
        public void SUBC_A_aHL_aHLeturns_proper_T_states(byte opcode, int cf)
        {
            var states = Execute(opcode);
            Assert.AreEqual(7, states);
        }

        void ExecuteWithNoCF(byte opcode)
        {
            Registers.CF = 0;
            Execute(opcode);
        }
    }
}