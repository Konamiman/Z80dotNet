using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class ADC_A_aHL_tests : InstructionsExecutionTestsBase
    {
        private const byte ADC_A_aHL_opcode = 0x8E;

        [Test]
        public void ADC_A_aHL_adds_values_appropriately_when_CF_is_zero()
        {
            var oldValue = Fixture.Create<byte>();
            var valueAdded = Fixture.Create<byte>();

            Setup(oldValue, valueAdded);
            Execute(ADC_A_aHL_opcode);

            Assert.AreEqual(oldValue.Add(valueAdded), Registers.A);
        }

        [Test]
        public void ADC_A_aHL_adds_values_appropriately_when_CF_is_one()
        {
            var oldValue = Fixture.Create<byte>();
            var valueAdded = Fixture.Create<byte>();

            Setup(oldValue, valueAdded, cf: 1);
            Execute(ADC_A_aHL_opcode);

            Assert.AreEqual(oldValue.Add(valueAdded + 1), Registers.A);
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
        public void ADC_A_aHL_sets_SF_appropriately()
        {
            Setup(0xFD, 1);

            Execute(ADC_A_aHL_opcode);
            Assert.AreEqual(1, Registers.SF);

            Execute(ADC_A_aHL_opcode);
            Assert.AreEqual(1, Registers.SF);

            Execute(ADC_A_aHL_opcode);
            Assert.AreEqual(0, Registers.SF);

            Execute(ADC_A_aHL_opcode);
            Assert.AreEqual(0, Registers.SF);
        }

        [Test]
        public void ADC_A_aHL_sets_ZF_appropriately()
        {
            Setup(0xFD, 1);

            Execute(ADC_A_aHL_opcode);
            Assert.AreEqual(0, Registers.ZF);

            Execute(ADC_A_aHL_opcode);
            Assert.AreEqual(0, Registers.ZF);

            Execute(ADC_A_aHL_opcode);
            Assert.AreEqual(1, Registers.ZF);

            Execute(ADC_A_aHL_opcode);
            Assert.AreEqual(0, Registers.ZF);
        }

        [Test]
        public void ADC_A_aHL_sets_HF_appropriately()
        {
            foreach(byte b in new byte[] { 0x0E, 0x7E, 0xFE }) 
            {
                Setup(b, 1);

                Execute(ADC_A_aHL_opcode);
                Assert.AreEqual(0, Registers.HF);

                Execute(ADC_A_aHL_opcode);
                Assert.AreEqual(1, Registers.HF);

                Execute(ADC_A_aHL_opcode);
                Assert.AreEqual(0, Registers.HF);
            }
        }

        [Test]
        public void ADC_A_aHL_sets_PF_appropriately()
        {
            Setup(0x7E, 1);

            Execute(ADC_A_aHL_opcode);
            Assert.AreEqual(0, Registers.PF);

            Execute(ADC_A_aHL_opcode);
            Assert.AreEqual(1, Registers.PF);

            Execute(ADC_A_aHL_opcode);
            Assert.AreEqual(0, Registers.PF);
        }

        [Test]
        public void ADC_A_aHL_aHLesets_NF()
        {
            AssertResetsFlags(ADC_A_aHL_opcode, null, "N");
        }

        [Test]
        public void ADC_A_aHL_sets_CF_appropriately()
        {
            Setup(0xFE, 1);

            Execute(ADC_A_aHL_opcode);
            Assert.AreEqual(0, Registers.CF);

            Execute(ADC_A_aHL_opcode);
            Assert.AreEqual(1, Registers.CF);

            Execute(ADC_A_aHL_opcode);
            Assert.AreEqual(0, Registers.CF);
        }

        [Test]
        public void ADC_A_aHL_sets_bits_3_and_5_from_aHLesult()
        {
            Setup(0, ((byte)0).WithBit(3, 1).WithBit(5, 0));
            Execute(ADC_A_aHL_opcode);
            Assert.AreEqual(1, Registers.Flag3);
            Assert.AreEqual(0, Registers.Flag5);

            Registers.A = 0;
            Setup(0, ((byte)0).WithBit(3, 0).WithBit(5, 1));
            Execute(ADC_A_aHL_opcode);
            Assert.AreEqual(0, Registers.Flag3);
            Assert.AreEqual(1, Registers.Flag5);
        }

        [Test]
        public void ADC_A_aHL_aHLeturns_proper_T_states()
        {
            var states = Execute(ADC_A_aHL_opcode);
            Assert.AreEqual(7, states);
        }
    }
}