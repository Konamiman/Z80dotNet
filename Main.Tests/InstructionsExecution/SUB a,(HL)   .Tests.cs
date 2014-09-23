using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class SUB_A_aHL_tests : InstructionsExecutionTestsBase
    {
        private const byte SUB_A_aHL_opcode = 0x96;

        [Test]
        public void SUB_A_aHL_substracts_value_from_memory()
        {
            var oldValue = Fixture.Create<byte>();
            var valueSubstracted = Fixture.Create<byte>();

            Setup(oldValue, valueSubstracted);
            Execute(SUB_A_aHL_opcode);

            Assert.AreEqual(oldValue.Sub(valueSubstracted), Registers.A);
        }

        private void Setup(byte oldValue, byte valueToSub)
        {
            Registers.A = oldValue;
            var address = Fixture.Create<ushort>();
            ProcessorAgent.Memory[address] = valueToSub;
            Registers.HL = address.ToShort();
        }

        [Test]
        public void SUB_A_aHL_sets_SF_appropriately()
        {
            Setup(0x02, 1);

            Execute(SUB_A_aHL_opcode);
            Assert.AreEqual(0, Registers.SF);

            Execute(SUB_A_aHL_opcode);
            Assert.AreEqual(0, Registers.SF);

            Execute(SUB_A_aHL_opcode);
            Assert.AreEqual(1, Registers.SF);

            Execute(SUB_A_aHL_opcode);
            Assert.AreEqual(1, Registers.SF);
        }

        [Test]
        public void SUB_A_aHL_sets_ZF_appropriately()
        {
            Setup(0x03, 1);

            Execute(SUB_A_aHL_opcode);
            Assert.AreEqual(0, Registers.ZF);

            Execute(SUB_A_aHL_opcode);
            Assert.AreEqual(0, Registers.ZF);

            Execute(SUB_A_aHL_opcode);
            Assert.AreEqual(1, Registers.ZF);

            Execute(SUB_A_aHL_opcode);
            Assert.AreEqual(0, Registers.ZF);
        }

        [Test]
        public void SUB_A_aHL_sets_HF_appropriately()
        {
            foreach(byte b in new byte[] { 0x11, 0x81, 0xF1 }) 
            {
                Setup(b, 1);

                Execute(SUB_A_aHL_opcode);
                Assert.AreEqual(0, Registers.HF);

                Execute(SUB_A_aHL_opcode);
                Assert.AreEqual(1, Registers.HF);

                Execute(SUB_A_aHL_opcode);
                Assert.AreEqual(0, Registers.HF);
            }
        }

        [Test]
        public void SUB_A_aHL_sets_PF_appropriately()
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
        public void SUB_A_aHL_sets_NF()
        {
            AssertSetsFlags(SUB_A_aHL_opcode, null, "N");
        }

        [Test]
        public void SUB_A_aHL_sets_CF_appropriately()
        {
            Setup(0x01, 1);

            Execute(SUB_A_aHL_opcode);
            Assert.AreEqual(0, Registers.CF);

            Execute(SUB_A_aHL_opcode);
            Assert.AreEqual(1, Registers.CF);

            Execute(SUB_A_aHL_opcode);
            Assert.AreEqual(0, Registers.CF);
        }

        [Test]
        public void SUB_A_aHL_sets_bits_3_and_5_from_result()
        {
            Setup((byte)(((byte)0).WithBit(3, 1).WithBit(5, 0) + 1), 1);
            Execute(SUB_A_aHL_opcode);
            Assert.AreEqual(1, Registers.Flag3);
            Assert.AreEqual(0, Registers.Flag5);

            Setup((byte)(((byte)0).WithBit(3, 0).WithBit(5, 1) + 1), 1);
            Execute(SUB_A_aHL_opcode);
            Assert.AreEqual(0, Registers.Flag3);
            Assert.AreEqual(1, Registers.Flag5);
        }

        [Test]
        public void SUB_A_aHL_returns_proper_T_states()
        {
            var states = Execute(SUB_A_aHL_opcode);
            Assert.AreEqual(7, states);
        }
    }
}