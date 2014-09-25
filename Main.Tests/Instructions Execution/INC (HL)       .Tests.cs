using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class INC_aHL_tests : InstructionsExecutionTestsBase
    {
        private const byte INC_aHL_opcode = 0x34;

        [Test]
        public void INC_aHL_increases_value_appropriately()
        {
            var oldValue = Fixture.Create<byte>();
            var address = Setup(oldValue);

            Execute(INC_aHL_opcode);

            AssertMemoryContents(address, oldValue.Inc());
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
        public void INC_aHL_sets_SF_appropriately()
        {
            Setup(0xFD);

            Execute(INC_aHL_opcode);
            Assert.AreEqual(1, Registers.SF);

            Execute(INC_aHL_opcode);
            Assert.AreEqual(1, Registers.SF);

            Execute(INC_aHL_opcode);
            Assert.AreEqual(0, Registers.SF);

            Execute(INC_aHL_opcode);
            Assert.AreEqual(0, Registers.SF);
        }

        [Test]
        public void INC_aHL_sets_ZF_appropriately()
        {
            Setup(0xFD);

            Execute(INC_aHL_opcode);
            Assert.AreEqual(0, Registers.ZF);

            Execute(INC_aHL_opcode);
            Assert.AreEqual(0, Registers.ZF);

            Execute(INC_aHL_opcode);
            Assert.AreEqual(1, Registers.ZF);

            Execute(INC_aHL_opcode);
            Assert.AreEqual(0, Registers.ZF);
        }

        [Test]
        public void INC_aHL_sets_HF_appropriately()
        {
            foreach(byte b in new byte[] { 0x0E, 0x7E, 0xFE })
            {
                Setup(b);

                Execute(INC_aHL_opcode);
                Assert.AreEqual(0, Registers.HF);

                Execute(INC_aHL_opcode);
                Assert.AreEqual(1, Registers.HF);

                Execute(INC_aHL_opcode);
                Assert.AreEqual(0, Registers.HF);
            }
        }

        [Test]
        public void INC_aHL_sets_PF_appropriately()
        {
            Setup(0x7E);

            Execute(INC_aHL_opcode);
            Assert.AreEqual(0, Registers.PF);

            Execute(INC_aHL_opcode);
            Assert.AreEqual(1, Registers.PF);

            Execute(INC_aHL_opcode);
            Assert.AreEqual(0, Registers.PF);
        }

        [Test]
        public void INC_aHL_resets_NF()
        {
            AssertResetsFlags(INC_aHL_opcode, null, "N");
        }

        [Test]
        public void INC_aHL_does_not_chance_CF()
        {
            var randomValues = Fixture.Create<byte[]>();

            foreach (var value in randomValues)
            {
                Setup(value);

                Registers.CF = 0;
                Execute(INC_aHL_opcode);
                Assert.AreEqual(0, Registers.CF);

                Registers.CF = 1;
                Execute(INC_aHL_opcode);
                Assert.AreEqual(1, Registers.CF);
            }
        }

        [Test]
        public void INC_aHL_sets_bits_3_and_5_from_result()
        {
            Setup(((byte)0).WithBit(3, 1).WithBit(5, 0));
            Execute(INC_aHL_opcode);
            Assert.AreEqual(1, Registers.Flag3);
            Assert.AreEqual(0, Registers.Flag5);

            Setup(((byte)0).WithBit(3, 0).WithBit(5, 1));
            Execute(INC_aHL_opcode);
            Assert.AreEqual(0, Registers.Flag3);
            Assert.AreEqual(1, Registers.Flag5);
        }

        [Test]
        public void INC_aHL_returns_proper_T_states()
        {
            var states = Execute(INC_aHL_opcode);
            Assert.AreEqual(11, states);
        }
    }
}
