using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class RLA_tests : InstructionsExecutionTestsBase
    {
        private const byte RLA_opcode = 0x17;

        [Test]
        public void RLA_rotates_byte_correctly()
        {
            var values = new byte[] { 0x6, 0xC, 0x18, 0x30, 0x60, 0xC0, 0x80, 0 };
            Registers.A = 0x03;

            for(var i = 0; i < values.Length; i++)
            {
                Execute(RLA_opcode);
                Assert.AreEqual(values[i], Registers.A & 0xFE);
            }
        }

        [Test]
        public void RLA_sets_bit_0_from_CF()
        {
            Registers.A = (byte)(Fixture.Create<byte>() | 1);
            Registers.CF = 0;
            Execute(RLA_opcode);
            Assert.AreEqual(0, Registers.A.GetBit(0));

            Registers.A = (byte)(Fixture.Create<byte>() & 0xFE);
            Registers.CF = 1;
            Execute(RLA_opcode);
            Assert.AreEqual(1, Registers.A.GetBit(0));
        }

        [Test]
        public void RLA_sets_CF_correctly()
        {
            Registers.A = 0x60;

            Execute(RLA_opcode);
            Assert.AreEqual(0, Registers.CF);

            Execute(RLA_opcode);
            Assert.AreEqual(1, Registers.CF);

            Execute(RLA_opcode);
            Assert.AreEqual(1, Registers.CF);

            Execute(RLA_opcode);
            Assert.AreEqual(0, Registers.CF);
        }

        [Test]
        public void RLA_resets_H_and_N()
        {
            var randomValues = Fixture.Create<byte[]>();

            foreach (var value in randomValues)
            {
                Registers.HF = 1;
                Registers.NF = 1;
                Registers.A = value;

                Execute(RLA_opcode);

                Assert.AreEqual(0, Registers.HF);
                Assert.AreEqual(0, Registers.NF);
            }
        }

        [Test]
        public void RLA_does_not_change_SF_ZF_PF()
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
                Execute(RLA_opcode);

                Assert.AreEqual(randomSF, Registers.SF);
                Assert.AreEqual(randomZF, Registers.ZF);
                Assert.AreEqual(randomPF, Registers.PF);
            }
        }

        [Test]
        public void RLA_returns_proper_T_states()
        {
            var states = Execute(RLA_opcode);
            Assert.AreEqual(4, states);
        }
    }
}