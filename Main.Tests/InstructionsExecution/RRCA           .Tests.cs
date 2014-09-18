using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public partial class Z80InstructionsExecutor
    {
        private const byte RRCA_opcode = 0x0F;

        [Test]
        public void RRCA_rotates_byte_correctly()
        {
            var values = new byte[] { 0x82, 0x41, 0xA0, 0x50, 0x28, 0x14, 0x0A, 0x05 };
            Registers.A = 0x05;

            for(var i = 0; i < values.Length; i++)
            {
                Execute(RRCA_opcode);
                Assert.AreEqual(values[i], Registers.A);
            }
        }

        [Test]
        public void RRCA_sets_CF_correctly()
        {
            Registers.A = 0x06;

            Execute(RRCA_opcode);
            Assert.AreEqual(0, Registers.CF);

            Execute(RRCA_opcode);
            Assert.AreEqual(1, Registers.CF);

            Execute(RRCA_opcode);
            Assert.AreEqual(1, Registers.CF);

            Execute(RRCA_opcode);
            Assert.AreEqual(0, Registers.CF);
        }

        [Test]
        public void RRCA_resets_H_and_N()
        {
            var randomValues = Fixture.Create<byte[]>();

            foreach (var value in randomValues)
            {
                Registers.HF = 1;
                Registers.NF = 1;
                Registers.A = value;

                Execute(RRCA_opcode);

                Assert.AreEqual(0, Registers.HF);
                Assert.AreEqual(0, Registers.NF);
            }
        }

        [Test]
        public void RRCA_does_not_change_SF_ZF_PF()
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
                Execute(RRCA_opcode);

                Assert.AreEqual(randomSF, Registers.SF);
                Assert.AreEqual(randomZF, Registers.ZF);
                Assert.AreEqual(randomPF, Registers.PF);
            }
        }

        [Test]
        public void RRCA_returns_proper_T_states()
        {
            var states = Execute(RRCA_opcode);
            Assert.AreEqual(4, states);
        }
    }
}