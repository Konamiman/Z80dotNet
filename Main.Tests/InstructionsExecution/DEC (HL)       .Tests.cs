using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class DEC_aHL_tests : InstructionsExecutionTestsBase
    {
        private const byte DEC_aHL_opcode = 0x35;

        [Test]
        public void DEC_aHL_decreases_value_appropriately()
        {
            var oldValue = Fixture.Create<byte>();
            var address = Setup(oldValue);

            Execute(DEC_aHL_opcode);

            AssertMemoryContents(address, oldValue.Dec());
        }

        private ushort Setup(byte value)
        {
            var address = Fixture.Create<ushort>();
            ProcessorAgent.Memory[address] = value;
            Registers.HL = address.ToShort();
            return address;
        }

        private void AssertMemoryContents(ushort address, byte expected)
        {
            Assert.AreEqual(expected, ProcessorAgent.Memory[address]);
        }

        [Test]
        public void DEC_aHL_sets_SF_appropriately()
        {
            Setup(0x02);

            Execute(DEC_aHL_opcode);
            Assert.AreEqual(0, Registers.SF);

            Execute(DEC_aHL_opcode);
            Assert.AreEqual(0, Registers.SF);

            Execute(DEC_aHL_opcode);
            Assert.AreEqual(1, Registers.SF);

            Execute(DEC_aHL_opcode);
            Assert.AreEqual(1, Registers.SF);
        }

        [Test]
        public void DEC_aHL_sets_ZF_appropriately()
        {
            Setup(0x03);

            Execute(DEC_aHL_opcode);
            Assert.AreEqual(0, Registers.ZF);

            Execute(DEC_aHL_opcode);
            Assert.AreEqual(0, Registers.ZF);

            Execute(DEC_aHL_opcode);
            Assert.AreEqual(1, Registers.ZF);

            Execute(DEC_aHL_opcode);
            Assert.AreEqual(0, Registers.ZF);
        }

        [Test]
        public void DEC_aHL_sets_HF_appropriately()
        {
            foreach(byte b in new byte[] { 0x11, 0x81, 0xF1 })
            {
                Setup(b);

                Execute(DEC_aHL_opcode);
                Assert.AreEqual(0, Registers.HF);

                Execute(DEC_aHL_opcode);
                Assert.AreEqual(1, Registers.HF);

                Execute(DEC_aHL_opcode);
                Assert.AreEqual(0, Registers.HF);
            }
        }

        [Test]
        public void DEC_aHL_sets_PF_appropriately()
        {
            Setup(0x81);

            Execute(DEC_aHL_opcode);
            Assert.AreEqual(0, Registers.PF);

            Execute(DEC_aHL_opcode);
            Assert.AreEqual(1, Registers.PF);

            Execute(DEC_aHL_opcode);
            Assert.AreEqual(0, Registers.PF);
        }

        [Test]
        public void DEC_aHL_sets_NF()
        {
            AssertSetsFlags(DEC_aHL_opcode, null, "N");
        }

        [Test]
        public void DEC_aHL_does_not_chance_CF()
        {
            var randomValues = Fixture.Create<byte[]>();

            foreach (var value in randomValues)
            {
                Setup(value);

                Registers.CF = 0;
                Execute(DEC_aHL_opcode);
                Assert.AreEqual(0, Registers.CF);

                Registers.CF = 1;
                Execute(DEC_aHL_opcode);
                Assert.AreEqual(1, Registers.CF);
            }
        }

        [Test]
        public void DEC_aHL_sets_bits_3_and_5_from_result()
        {
            Setup(((byte)1).WithBit(3, 1).WithBit(5, 0));
            Execute(DEC_aHL_opcode);
            Assert.AreEqual(1, Registers.Flag3);
            Assert.AreEqual(0, Registers.Flag5);

            Setup(((byte)1).WithBit(3, 0).WithBit(5, 1));
            Execute(DEC_aHL_opcode);
            Assert.AreEqual(0, Registers.Flag3);
            Assert.AreEqual(1, Registers.Flag5);
        }

        [Test]
        public void DEC_aHL_returns_proper_T_states()
        {
            var states = Execute(DEC_aHL_opcode);
            Assert.AreEqual(11, states);
        }
    }
}
