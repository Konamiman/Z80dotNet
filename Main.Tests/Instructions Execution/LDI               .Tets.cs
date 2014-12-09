using System.Security.Cryptography;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class LDI_tests : InstructionsExecutionTestsBase
    {
        private const byte prefix = 0xED;

        public static object[] LDI_Source =
        {
            new object[] {"LDI", (byte)0xA0},
        };

        public static object[] LDD_Source =
        {
            new object[] {"LDD", (byte)0xA8}
        };

        [Test]
        [TestCaseSource("LDI_Source")]
        [TestCaseSource("LDD_Source")]
        public void LDI_LDD_copy_value_correctly(string instr, byte opcode)
        {
            var oldValue = Fixture.Create<byte>();
            var value = Fixture.Create<byte>();
            var srcAddress = Fixture.Create<short>();
            var destAddress = Fixture.Create<short>();

            Registers.HL = srcAddress;
            Registers.DE = destAddress;

            ProcessorAgent.Memory[srcAddress] = value;
            ProcessorAgent.Memory[destAddress] = oldValue;

            Execute(opcode, prefix);

            var newValue = ProcessorAgent.Memory[destAddress];
            Assert.AreEqual(value, newValue);
        }

        [Test]
        [TestCaseSource("LDI_Source")]
        public void LDI_increases_DE_and_HL(string instr, byte opcode)
        {
            var srcAddress = Fixture.Create<short>();
            var destAddress = Fixture.Create<short>();

            Registers.HL = srcAddress;
            Registers.DE = destAddress;

            Execute(opcode, prefix);

            Assert.AreEqual(srcAddress.Inc(), Registers.HL);
            Assert.AreEqual(destAddress.Inc(), Registers.DE);
        }

        [Test]
        [TestCaseSource("LDD_Source")]
        public void LDI_decreases_DE_and_HL(string instr, byte opcode)
        {
            var srcAddress = Fixture.Create<short>();
            var destAddress = Fixture.Create<short>();

            Registers.HL = srcAddress;
            Registers.DE = destAddress;

            Execute(opcode, prefix);

            Assert.AreEqual(srcAddress.Dec(), Registers.HL);
            Assert.AreEqual(destAddress.Dec(), Registers.DE);
        }

        [Test]
        [TestCaseSource("LDI_Source")]
        [TestCaseSource("LDD_Source")]
        public void LDI_LDD_decrease_BC(string instr, byte opcode)
        {
            var counter = Fixture.Create<short>();
            Registers.BC = counter;

            Execute(opcode, prefix);

            Assert.AreEqual(counter.Dec(), Registers.BC);
        }

        [Test]
        [TestCaseSource("LDI_Source")]
        [TestCaseSource("LDD_Source")]
        public void LDI_LDD_do_not_change_S_Z_C(string instr, byte opcode)
        {
            AssertDoesNotChangeFlags(opcode, prefix, "S", "Z", "C");
        }

        [Test]
        [TestCaseSource("LDI_Source")]
        [TestCaseSource("LDD_Source")]
        public void LDI_LDD_reset_H_N(string instr, byte opcode)
        {
            AssertResetsFlags(opcode, prefix, "H", "N");
        }

        [Test]
        [TestCaseSource("LDI_Source")]
        [TestCaseSource("LDD_Source")]
        public void LDI_LDD_resets_PF_if_BC_reaches_zero(string instr, byte opcode)
        {
            Registers.BC = 128;
            for(int i = 0; i <= 256; i++)
            {
                var oldBC = Registers.BC;
                Execute(opcode, prefix);
                Assert.AreEqual(oldBC.Dec(), Registers.BC);
                Assert.AreEqual(Registers.BC != 0, (bool)Registers.PF);
            }
        }

        [Test]
        [TestCaseSource("LDI_Source")]
        [TestCaseSource("LDD_Source")]
        public void LDI_LDD_set_Flag3_from_bit_3_of_value_plus_A_and_Flag5_from_bit_1(string instr, byte opcode)
        {
            var value = Fixture.Create<byte>();
            var srcAddress = Fixture.Create<short>();

            for(int i = 0; i < 256; i++)
            {
                var valueOfA = (byte)i;
                Registers.A = valueOfA;
                Registers.HL = srcAddress;
                ProcessorAgent.Memory[srcAddress] = value;

                Execute(opcode, prefix);

                var valuePlusA = value.Add(valueOfA).GetLowByte();
                Assert.AreEqual(valuePlusA.GetBit(3), Registers.Flag3);
                Assert.AreEqual(valuePlusA.GetBit(1), Registers.Flag5);
            }
        }

        [Test]
        [TestCaseSource("LDI_Source")]
        [TestCaseSource("LDD_Source")]
        public void LDI_LDD_return_proper_T_states(string instr, byte opcode)
        {
            var states = Execute(opcode, prefix);
            Assert.AreEqual(16, states);
        }
    }
}