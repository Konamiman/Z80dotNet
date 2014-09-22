using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class ADD_A_aHL_tests : InstructionsExecutionTestsBase
    {
        private const byte ADD_A_aHL_opcode = 0x86;

        [Test]
        public void ADD_A_aHL_adds_values_appropriately()
        {
            var oldValue = Fixture.Create<byte>();
            var valueAdded = Fixture.Create<byte>();

            Setup(oldValue, valueAdded);
            Execute(ADD_A_aHL_opcode);

            Assert.AreEqual(oldValue.Add(valueAdded), Registers.A);
        }

        private void Setup(byte oldValue, byte valueToAdd)
        {
            Registers.A = oldValue;
            var address = Fixture.Create<ushort>();
            ProcessorAgent.Memory[address] = valueToAdd;
            Registers.HL = address.ToShort();
        }

        [Test]
        public void ADD_A_aHL_sets_SF_appropriately()
        {
            Setup(0xFD, 1);

            Execute(ADD_A_aHL_opcode);
            Assert.AreEqual(1, Registers.SF);

            Execute(ADD_A_aHL_opcode);
            Assert.AreEqual(1, Registers.SF);

            Execute(ADD_A_aHL_opcode);
            Assert.AreEqual(0, Registers.SF);

            Execute(ADD_A_aHL_opcode);
            Assert.AreEqual(0, Registers.SF);
        }

        [Test]
        public void ADD_A_aHL_sets_ZF_appropriately()
        {
            Setup(0xFD, 1);

            Execute(ADD_A_aHL_opcode);
            Assert.AreEqual(0, Registers.ZF);

            Execute(ADD_A_aHL_opcode);
            Assert.AreEqual(0, Registers.ZF);

            Execute(ADD_A_aHL_opcode);
            Assert.AreEqual(1, Registers.ZF);

            Execute(ADD_A_aHL_opcode);
            Assert.AreEqual(0, Registers.ZF);
        }

        [Test]
        public void ADD_A_aHL_sets_HF_appropriately()
        {
            foreach(byte b in new byte[] { 0x0E, 0x7E, 0xFE }) 
            {
                Setup(b, 1);

                Execute(ADD_A_aHL_opcode);
                Assert.AreEqual(0, Registers.HF);

                Execute(ADD_A_aHL_opcode);
                Assert.AreEqual(1, Registers.HF);

                Execute(ADD_A_aHL_opcode);
                Assert.AreEqual(0, Registers.HF);
            }
        }

        [Test]
        public void ADD_A_aHL_sets_PF_appropriately()
        {
            Setup(0x7E, 1);

            Execute(ADD_A_aHL_opcode);
            Assert.AreEqual(0, Registers.PF);

            Execute(ADD_A_aHL_opcode);
            Assert.AreEqual(1, Registers.PF);

            Execute(ADD_A_aHL_opcode);
            Assert.AreEqual(0, Registers.PF);
        }

        [Test]
        public void ADD_A_aHL_aHLesets_NF()
        {
            AssertResetsFlags(ADD_A_aHL_opcode, null, "N");
        }

        [Test]
        public void ADD_A_aHL_sets_CF_appropriately()
        {
            Setup(0xFE, 1);

            Execute(ADD_A_aHL_opcode);
            Assert.AreEqual(0, Registers.CF);

            Execute(ADD_A_aHL_opcode);
            Assert.AreEqual(1, Registers.CF);

            Execute(ADD_A_aHL_opcode);
            Assert.AreEqual(0, Registers.CF);
        }

        [Test]
        public void ADD_A_aHL_sets_bits_3_and_5_from_aHLesult()
        {
            Setup(0, ((byte)0).WithBit(3, 1).WithBit(5, 0));
            Execute(ADD_A_aHL_opcode);
            Assert.AreEqual(1, Registers.Flag3);
            Assert.AreEqual(0, Registers.Flag5);

            Registers.A = 0;
            Setup(0, ((byte)0).WithBit(3, 0).WithBit(5, 1));
            Execute(ADD_A_aHL_opcode);
            Assert.AreEqual(0, Registers.Flag3);
            Assert.AreEqual(1, Registers.Flag5);
        }

        [Test]
        public void ADD_A_aHL_aHLeturns_proper_T_states()
        {
            var states = Execute(ADD_A_aHL_opcode);
            Assert.AreEqual(7, states);
        }
    }
}