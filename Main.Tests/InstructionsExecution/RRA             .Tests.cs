using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class RRA_tests : InstructionsExecutionTestsBase
    {
        private const byte RRA_opcode = 0x1F;

        [Test]
        public void RRA_rotates_byte_correctly()
        {
            var values = new byte[] { 0x60, 0x30, 0x18, 0xC, 0x6, 0x3, 0x1, 0x0 };
            Registers.A = 0xC0;

            for(var i = 0; i < values.Length; i++)
            {
                Execute(RRA_opcode);
                Assert.AreEqual(values[i], Registers.A & 0x7F);
            }
        }

        [Test]
        public void RLA_sets_bit_7_from_CF()
        {
            Registers.A = (byte)(Fixture.Create<byte>() | 0x80);
            Registers.CF = 0;
            Execute(RRA_opcode);
            Assert.AreEqual(0, Registers.A.GetBit(7));

            Registers.A = (byte)(Fixture.Create<byte>() & 0x7F);
            Registers.CF = 1;
            Execute(RRA_opcode);
            Assert.AreEqual(1, Registers.A.GetBit(7));
        }

        [Test]
        public void RRA_sets_CF_correctly()
        {
            Registers.A = 0x06;

            Execute(RRA_opcode);
            Assert.AreEqual(0, Registers.CF);

            Execute(RRA_opcode);
            Assert.AreEqual(1, Registers.CF);

            Execute(RRA_opcode);
            Assert.AreEqual(1, Registers.CF);

            Execute(RRA_opcode);
            Assert.AreEqual(0, Registers.CF);
        }

        [Test]
        public void RRA_resets_H_and_N()
        {
            var randomValues = Fixture.Create<byte[]>();

            foreach (var value in randomValues)
            {
                Registers.HF = 1;
                Registers.NF = 1;
                Registers.A = value;

                Execute(RRA_opcode);

                Assert.AreEqual(0, Registers.HF);
                Assert.AreEqual(0, Registers.NF);
            }
        }

        [Test]
        public void RRA_does_not_change_SF_ZF_PF()
        {
            var randomValues = Fixture.Create<byte[]>();
            var randomSF = Fixture.Create<Bit>();
            var randomZF = Fixture.Create<Bit>();
            var randomPF = Fixture.Create<Bit>();

            Registers.SF = randomSF;
            Registers.ZF = randomZF;
            Registers.PF = randomPF;

            foreach (var value in randomValues)
            {
                Execute(RRA_opcode);

                Assert.AreEqual(randomSF, Registers.SF);
                Assert.AreEqual(randomZF, Registers.ZF);
                Assert.AreEqual(randomPF, Registers.PF);
            }
        }

        [Test]
        public void RRA_returns_proper_T_states()
        {
            var states = Execute(RRA_opcode);
            Assert.AreEqual(4, states);
        }
    }
}