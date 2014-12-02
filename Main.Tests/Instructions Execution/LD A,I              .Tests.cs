using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class LD_A_I_tests : InstructionsExecutionTestsBase
    {
        private const byte opcode = 0x57;
        private const byte prefix = 0xED;

        [Test]
        public void LD_A_I_loads_value_correctly()
        {
            var oldValue = Fixture.Create<byte>();
            var newValue = Fixture.Create<byte>();
            Registers.A = oldValue;
            Registers.I = newValue;

            Execute(opcode, prefix);

            Assert.AreEqual(newValue, Registers.A);
        }

        [Test]
        public void LD_A_I_returns_proper_T_states()
        {
            var states = Execute(opcode, prefix);
            Assert.AreEqual(9, states);
        }

        [Test]
        public void LD_A_I_sets_SF_properly()
        {
            for(int i=0; i<=255; i++)
            {
                var b = (byte)i;
                Registers.I = b;
                Execute(opcode, prefix);
                Assert.AreEqual(b >= 128, (bool)Registers.SF);
            }
        }

        [Test]
        public void LD_A_I_sets_ZF_properly()
        {
            for(int i=0; i<=255; i++)
            {
                var b = (byte)i;
                Registers.I = b;
                Execute(opcode, prefix);
                Assert.AreEqual(b == 0, (bool)Registers.ZF);
            }
        }

        [Test]
        public void LD_A_I_sets_PF_from_IFF2()
        {
            Registers.I = Fixture.Create<byte>();

            Registers.IFF2 = 0;
            Execute(opcode, prefix);
            Assert.AreEqual(0, Registers.PF);

            Registers.IFF2 = 1;
            Execute(opcode, prefix);
            Assert.AreEqual(1, Registers.PF);
        }

        [Test]
        public void LD_A_I_resets_HF_and_NF_properly()
        {
            AssertResetsFlags(opcode, prefix, "H", "N");
        }

        [Test]
        public void LD_A_I_does_not_change_CF()
        {
            AssertDoesNotChangeFlags(opcode, prefix, "C");
        }

        [Test]
        public void LD_A_I_sets_flags_3_5_from_I()
        {
            Registers.I = ((byte)1).WithBit(3, 1).WithBit(5, 0);
            Execute(opcode, prefix);
            Assert.AreEqual(1, Registers.Flag3);
            Assert.AreEqual(0, Registers.Flag5);

            Registers.I = ((byte)1).WithBit(3, 0).WithBit(5, 1);
            Execute(opcode, prefix);
            Assert.AreEqual(0, Registers.Flag3);
            Assert.AreEqual(1, Registers.Flag5);
        }
    }
}