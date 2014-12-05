using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class RRD_tests : InstructionsExecutionTestsBase
    {
        private const byte opcode = 0x67;
        private const byte prefix = 0xED;

        [Test]
        public void RRD_moves_data_appropriately()
        {
            var oldHLContents = "1001 0110".AsBinaryByte();
            var oldAValue = "0011 1010".AsBinaryByte();
            var address = Setup(oldHLContents, oldAValue);
            
            Execute(opcode, prefix);

            var expectedHLContents = "1010 1001".AsBinaryByte();
            var expectedAValue = "0011 0110".AsBinaryByte();

            AssertMemoryContents(address, expectedHLContents);
            Assert.AreEqual(expectedAValue, Registers.A);
        }

        private ushort Setup(byte HLcontents, byte Avalue)
        {
            Registers.A = Avalue;
            var address = Fixture.Create<ushort>();
            ProcessorAgent.Memory[address] = HLcontents;
            Registers.HL = address.ToShort();
            return address;
        }

        private void AssertMemoryContents(ushort address, byte expected)
        {
            Assert.AreEqual(expected, ProcessorAgent.Memory[address]);
        }

        [Test]
        public void RRD_sets_SF_appropriately()
        {
            for(int i=0; i<=255; i++)
            {
                var b = (byte)i;
                Registers.A = b;
                Execute(opcode, prefix);
                Assert.AreEqual(b >= 128, (bool)Registers.SF);
            }
        }

        [Test]
        public void RRD_sets_ZF_appropriately()
        {
            for(int i=0; i<=255; i++)
            {
                var b = (byte)i;
                Setup(b, 0);
                Execute(opcode, prefix);
                Assert.AreEqual(Registers.A == 0, (bool)Registers.ZF);
            }
        }

        [Test]
        public void RRD_resets_HF()
        {
            AssertResetsFlags(opcode, prefix, "H");
        }

        [Test]
        public void RRD_sets_PF_as_parity()
        {
            for(int i=0; i<=255; i++)
            {
                var b = (byte)i;
                Setup(Fixture.Create<byte>(), b);
                Execute(opcode, prefix);
                Assert.AreEqual(Parity[Registers.A], Registers.PF);
            }
        }

        [Test]
        public void RRD_sets_NF()
        {
            AssertSetsFlags(opcode, prefix, "N");
        }

        [Test]
        public void RRD_does_not_chance_CF()
        {
            AssertDoesNotChangeFlags(opcode, prefix, "C");
        }

        [Test]
        public void DEC_aHL_sets_bits_3_and_5_from_result()
        {
            for(int i=0; i<=255; i++)
            {
                var b = (byte)i;
                Setup(Fixture.Create<byte>(), b);
                Execute(opcode, prefix);
                Assert.AreEqual(Registers.A.GetBit(3), Registers.Flag3);
                Assert.AreEqual(Registers.A.GetBit(5), Registers.Flag5);
            }
        }

        [Test]
        public void RRD_returns_proper_T_states()
        {
            var states = Execute(opcode, prefix);
            Assert.AreEqual(18, states);
        }
    }
}
