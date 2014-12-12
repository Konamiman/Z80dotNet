using System;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class INI_tests : InstructionsExecutionTestsBase
    {
        private const byte prefix = 0xED;

        public static object[] INI_Source =
        {
            new object[] { (byte)0xA2 }
        };

        public static object[] IND_Source =
        {
            new object[] { (byte)0xAA }
        };

        [Test]
        [TestCaseSource("INI_Source")]
        [TestCaseSource("IND_Source")]
        public void INI_IND_reads_value_from_port_aC_into_aHL(byte opcode)
        {
            var portNumber = Fixture.Create<byte>();
            var value = Fixture.Create<byte>();
            var oldValue = Fixture.Create<byte>();
            var address = Fixture.Create<ushort>();

            Registers.HL = address.ToShort();
            ProcessorAgent.Memory[address] = oldValue;

            ExecuteRead(opcode, portNumber, value);

            var actual = ProcessorAgent.Memory[address];
            Assert.AreEqual(value, actual);
        }

        [Test]
        [TestCaseSource("INI_Source")]
        public void INI_increases_HL(byte opcode)
        {
            var address = Fixture.Create<ushort>();

            Registers.HL = address.ToShort();

            ExecuteRead(opcode);

            var expected = address.Inc();
            Assert.AreEqual(expected, Registers.HL);
        }

        [Test]
        [TestCaseSource("IND_Source")]
        public void IND_decreases_HL(byte opcode)
        {
            var address = Fixture.Create<ushort>();

            Registers.HL = address.ToShort();

            ExecuteRead(opcode);

            var expected = address.Dec();
            Assert.AreEqual(expected, Registers.HL);
        }

        [Test]
        [TestCaseSource("INI_Source")]
        [TestCaseSource("IND_Source")]
        public void INI_IND_decrease_B(byte opcode)
        {
            var counter = Fixture.Create<byte>();

            Registers.B = counter;

            ExecuteRead(opcode);

            var expected = counter.Dec();
            Assert.AreEqual(expected, Registers.B);
        }

        [Test]
        [TestCaseSource("INI_Source")]
        [TestCaseSource("IND_Source")]
        public void INI_IND_set_Z_if_B_reaches_zero(byte opcode)
        {
            for (int i = 0; i < 256; i++)
            {
                byte b = (byte)i;
                Registers.B = b;

                ExecuteRead(opcode);

                Assert.AreEqual(b-1 == 0, (bool)Registers.ZF);
            }
        }

        [Test]
        [TestCaseSource("INI_Source")]
        [TestCaseSource("IND_Source")]
        public void INI_IND_set_NF(byte opcode)
        {
            AssertSetsFlags(opcode, prefix, "N");
        }

        [Test]
        [TestCaseSource("INI_Source")]
        [TestCaseSource("IND_Source")]
        public void INI_IND_do_not_change_CF(byte opcode)
        {
            AssertDoesNotChangeFlags(opcode, prefix, "C");
        }

        [Test]
        [TestCaseSource("INI_Source")]
        [TestCaseSource("IND_Source")]
        public void INI_IND_set_bits_3_and_5_from_result_of_decrementing_B(byte opcode)
        {
            Registers.B = ((byte)1).WithBit(3, 1).WithBit(5, 0);
            Execute(opcode, prefix);
            Assert.AreEqual(1, Registers.Flag3);
            Assert.AreEqual(0, Registers.Flag5);

            Registers.B = ((byte)1).WithBit(3, 0).WithBit(5, 1);
            Execute(opcode, prefix);
            Assert.AreEqual(0, Registers.Flag3);
            Assert.AreEqual(1, Registers.Flag5);
        }

        [Test]
        [TestCaseSource("INI_Source")]
        [TestCaseSource("IND_Source")]
        public void INI_IND_set_SF_appropriately(byte opcode)
        {
            Registers.B = 0x02;

            Execute(opcode, prefix);
            Assert.AreEqual(0, Registers.SF);

            Execute(opcode, prefix);
            Assert.AreEqual(0, Registers.SF);

            Execute(opcode, prefix);
            Assert.AreEqual(1, Registers.SF);

            Execute(opcode, prefix);
            Assert.AreEqual(1, Registers.SF);
        }

        [Test]
        [TestCaseSource("INI_Source")]
        [TestCaseSource("IND_Source")]
        public void INI_IND_return_proper_T_states(byte opcode)
        {
            var states = Execute(opcode, prefix);
            Assert.AreEqual(16, states);
        }

        private int ExecuteRead(byte opcode, byte portNumber = 0, byte portValue = 0)
        {
            Registers.C = portNumber;
            SetPortValue(portNumber, portValue);
            return Execute(opcode, prefix);
        }
    }
}