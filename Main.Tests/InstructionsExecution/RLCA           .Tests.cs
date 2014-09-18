using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public partial class Z80InstructionsExecutor
    {
        private const byte RLCA_opcode = 0x07;

        [Test]
        public void RLCA_rotates_byte_correctly()
        {
            var values = new byte[] {0xA, 0x14, 0x28, 0x50, 0xA0, 0x41, 0x82, 0x05};
            Registers.Main.A = 0x05;

            for(var i = 0; i < values.Length; i++)
            {
                Execute(RLCA_opcode);
                Assert.AreEqual(values[i], Registers.Main.A);
            }
        }

        [Test]
        public void RLCA_sets_CF_correctly()
        {
            Registers.Main.A = 0x60;

            Execute(RLCA_opcode);
            Assert.AreEqual(0, Registers.Main.CF);

            Execute(RLCA_opcode);
            Assert.AreEqual(1, Registers.Main.CF);

            Execute(RLCA_opcode);
            Assert.AreEqual(1, Registers.Main.CF);

            Execute(RLCA_opcode);
            Assert.AreEqual(0, Registers.Main.CF);
        }

        [Test]
        public void RLCA_resets_H_and_N()
        {
            var randomValues = Fixture.Create<byte[]>();

            foreach (var value in randomValues)
            {
                Registers.Main.HF = 1;
                Registers.Main.NF = 1;
                Registers.Main.A = value;

                Execute(RLCA_opcode);

                Assert.AreEqual(0, Registers.Main.HF);
                Assert.AreEqual(0, Registers.Main.NF);
            }
        }

        [Test]
        public void RLCA_does_not_change_SF_ZF_PF()
        {
            var randomValues = Fixture.Create<byte[]>();
            var randomSF = Fixture.Create<Bit>();
            var randomZF = Fixture.Create<Bit>();
            var randomPF = Fixture.Create<Bit>();

            Registers.Main.SF = randomSF;
            Registers.Main.ZF = randomZF;
            Registers.Main.PF = randomPF;

            foreach (var value in randomValues)
            {
                Execute(RLCA_opcode);

                Assert.AreEqual(randomSF, Registers.Main.SF);
                Assert.AreEqual(randomZF, Registers.Main.ZF);
                Assert.AreEqual(randomPF, Registers.Main.PF);
            }
        }

        [Test]
        public void RLCA_returns_proper_T_states()
        {
            var states = Execute(RLCA_opcode);
            Assert.AreEqual(4, states);
        }
    }
}