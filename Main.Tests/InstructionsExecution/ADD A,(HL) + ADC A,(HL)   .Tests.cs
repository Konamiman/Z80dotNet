using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class ADC_A_aHL_tests : InstructionsExecutionTestsBase
    {
        private const byte ADD_A_aHL_opcode = 0x86;
        private const byte ADC_A_aHL_opcode = 0x8E;

        public static object[] ADDC_A_aHL_Source =
        {
            new object[] {ADD_A_aHL_opcode, 0},
            new object[] {ADC_A_aHL_opcode, 0},
            new object[] {ADC_A_aHL_opcode, 1}
        };

        [Test]
        [TestCaseSource("ADDC_A_aHL_Source")]
        public void ADC_A_aHL_adds_values_appropriately_with_or_without_CF(byte opcode, int cf)
        {
            var oldValue = Fixture.Create<byte>();
            var valueAdded = Fixture.Create<byte>();

            Setup(oldValue, valueAdded, cf);
            Execute(opcode);

            Assert.AreEqual(oldValue.Add(valueAdded + cf), Registers.A);
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
        [TestCaseSource("ADDC_A_aHL_Source")]
        public void ADC_A_aHL_sets_SF_appropriately(byte opcode, int cf)
        {
            Setup(0xFD, 1);

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
        [TestCaseSource("ADDC_A_aHL_Source")]
        public void ADC_A_aHL_sets_ZF_appropriately(byte opcode, int cf)
        {
            Setup(0xFD, 1);

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
        [TestCaseSource("ADDC_A_aHL_Source")]
        public void ADC_A_aHL_sets_HF_appropriately(byte opcode, int cf)
        {
            foreach(byte b in new byte[] { 0x0E, 0x7E, 0xFE }) 
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
        [TestCaseSource("ADDC_A_aHL_Source")]
        public void ADC_A_aHL_sets_PF_appropriately(byte opcode, int cf)
        {
            Setup(0x7E, 1);

            ExecuteWithNoCF(opcode);
            Assert.AreEqual(0, Registers.PF);

            ExecuteWithNoCF(opcode);
            Assert.AreEqual(1, Registers.PF);

            ExecuteWithNoCF(opcode);
            Assert.AreEqual(0, Registers.PF);
        }

        [Test]
        public void ADC_A_aHL_aHLesets_NF()
        {
            AssertResetsFlags(ADC_A_aHL_opcode, null, "N");
        }

        [Test]
        [TestCaseSource("ADDC_A_aHL_Source")]
        public void ADC_A_aHL_sets_CF_appropriately(byte opcode, int cf)
        {
            Setup(0xFE, 1);

            ExecuteWithNoCF(opcode);
            Assert.AreEqual(0, Registers.CF);

            ExecuteWithNoCF(opcode);
            Assert.AreEqual(1, Registers.CF);

            ExecuteWithNoCF(opcode);
            Assert.AreEqual(0, Registers.CF);
        }

        [Test]
        [TestCaseSource("ADDC_A_aHL_Source")]
        public void ADC_A_aHL_sets_bits_3_and_5_from_aHLesult(byte opcode, int cf)
        {
            Setup(0, ((byte)0).WithBit(3, 1).WithBit(5, 0));
            ExecuteWithNoCF(opcode);
            Assert.AreEqual(1, Registers.Flag3);
            Assert.AreEqual(0, Registers.Flag5);

            Registers.A = 0;
            Setup(0, ((byte)0).WithBit(3, 0).WithBit(5, 1));
            ExecuteWithNoCF(opcode);
            Assert.AreEqual(0, Registers.Flag3);
            Assert.AreEqual(1, Registers.Flag5);
        }

        [Test]
        [TestCaseSource("ADDC_A_aHL_Source")]
        public void ADC_A_aHL_aHLeturns_proper_T_states(byte opcode, int cf)
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