using System;
using Konamiman.Z80dotNet.Tests.InstructionsExecution;
using NUnit.Framework;
using AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class IN_r_C : InstructionsExecutionTestsBase
    {
        public static object[] IN_r_C_Source =
        {
            new Object[] { "A", (byte)0x78 },
            new Object[] { "B", (byte)0x40 },
            new Object[] { "C", (byte)0x48 },
            new Object[] { "D", (byte)0x50 },
            new Object[] { "E", (byte)0x58 },
            new Object[] { "H", (byte)0x60 },
            new Object[] { "L", (byte)0x68 }
        };

        public static object[] IN_F_C_Source =
        {
            new Object[] { "F", (byte)0x70 },
        
        };

        [Test]
        [TestCaseSource(nameof(IN_r_C_Source))]
        public void IN_r_C_reads_value_from_port(string reg, byte opcode)
        {
            var portNumberLow = Fixture.Create<byte>();
            var portNumberHigh = Fixture.Create<byte>();
            var value = Fixture.Create<byte>();
            var oldValue = Fixture.Create<byte>();

            if(reg != "C")
                SetReg(reg, oldValue);

            Execute(opcode, portNumberLow, portNumberHigh, value);

            Assert.That(GetReg<byte>(reg), Is.EqualTo(value));
        }

        [Test]
        [TestCaseSource(nameof(IN_r_C_Source))]
        [TestCaseSource(nameof(IN_F_C_Source))]
        public void IN_r_C_sets_SF_appropriately(string reg, byte opcode)
        {
            var portNumberLow = Fixture.Create<byte>();
            var portNumberHigh = Fixture.Create<byte>();

            Execute(opcode, portNumberLow, portNumberHigh, 0xFE);
            Assert.That(Registers.SF.Value, Is.EqualTo(1));

            Execute(opcode, portNumberLow, portNumberHigh, 0xFF);
            Assert.That(Registers.SF.Value, Is.EqualTo(1));

            Execute(opcode, portNumberLow, portNumberHigh, 0);
            Assert.That(Registers.SF.Value, Is.EqualTo(0));

            Execute(opcode, portNumberLow, portNumberHigh, 1);
            Assert.That(Registers.SF.Value, Is.EqualTo(0));
        }

        [Test]
        [TestCaseSource(nameof(IN_r_C_Source))]
        [TestCaseSource(nameof(IN_F_C_Source))]
        public void IN_r_C_sets_ZF_appropriately(string reg, byte opcode)
        {
            var portNumberLow = Fixture.Create<byte>();
            var portNumberHigh = Fixture.Create<byte>();

            Execute(opcode, portNumberLow, portNumberHigh, 0xFF);
            Assert.That(Registers.ZF.Value, Is.EqualTo(0));

            Execute(opcode, portNumberLow, portNumberHigh, 0);
            Assert.That(Registers.ZF.Value, Is.EqualTo(1));

            Execute(opcode, portNumberLow, portNumberHigh, 1);
            Assert.That(Registers.ZF.Value, Is.EqualTo(0));
        }

        [Test]
        [TestCaseSource(nameof(IN_r_C_Source))]
        [TestCaseSource(nameof(IN_F_C_Source))]
        public void IN_r_C_resets_HF_NF(string reg, byte opcode)
        {
            AssertResetsFlags(opcode, 0xED, "H", "N");
        }

        [Test]
        [TestCaseSource(nameof(IN_r_C_Source))]
        [TestCaseSource(nameof(IN_F_C_Source))]
        public void IN_r_C_does_not_change_CF(string reg, byte opcode)
        {
            var randomValues = Fixture.Create<byte[]>();
            var portNumberLow = Fixture.Create<byte>();
            var portNumberHigh = Fixture.Create<byte>();

            foreach (var value in randomValues)
            {
                Registers.CF = 0;
                Execute(opcode, portNumberLow, portNumberHigh, value);
                Assert.That(Registers.CF.Value, Is.EqualTo(0));

                Registers.CF = 1;
                Execute(opcode, portNumberLow, portNumberHigh, value);
                Assert.That(Registers.CF.Value, Is.EqualTo(1));
            }
        }

        [Test]
        [TestCaseSource(nameof(IN_r_C_Source))]
        [TestCaseSource(nameof(IN_F_C_Source))]
        public void IN_r_C_sets_PF_as_parity(string reg, byte opcode)
        {
            var randomValues = Fixture.Create<byte[]>();
            var portNumberLow = Fixture.Create<byte>();
            var portNumberHigh = Fixture.Create<byte>();

            foreach (var value in randomValues)
            {
                Execute(opcode, portNumberLow, portNumberHigh, value);
                Assert.That(Registers.PF.Value, Is.EqualTo(Parity[value]));
            }
        }

        [Test]
        [TestCaseSource(nameof(IN_r_C_Source))]
        [TestCaseSource(nameof(IN_F_C_Source))]
        public void IN_r_C_sets_bits_3_and_5_from_result(string reg, byte opcode)
        {
            var portNumberLow = Fixture.Create<byte>();
            var portNumberHigh = Fixture.Create<byte>();
            var value = ((byte)0).WithBit(3, 1).WithBit(5, 0);
            Execute(opcode, portNumberLow, portNumberHigh, value);
            Assert.Multiple(() =>
            {
                Assert.That(Registers.Flag3.Value, Is.EqualTo(1));
                Assert.That(Registers.Flag5.Value, Is.EqualTo(0));
            });

            value = ((byte)0).WithBit(3, 0).WithBit(5, 1);
            Execute(opcode, portNumberLow, portNumberHigh, value);
            Assert.Multiple(() =>
            {
                Assert.That(Registers.Flag3.Value, Is.EqualTo(0));
                Assert.That(Registers.Flag5.Value, Is.EqualTo(1));
            });
        }

        [Test]
        [TestCaseSource(nameof(IN_r_C_Source))]
        [TestCaseSource(nameof(IN_F_C_Source))]
        public void IN_r_C_returns_proper_T_states(string reg, byte opcode)
        {
            var portNumberLow = Fixture.Create<byte>();
            var portNumberHigh = Fixture.Create<byte>();
            var value = Fixture.Create<byte>();
            var states = Execute(opcode, portNumberLow, portNumberHigh, value);
            Assert.That(states, Is.EqualTo(12));
        }

        private int Execute(byte opcode, byte portNumberLow, byte portNumberHigh, byte value)
        {
            Registers.C = portNumberLow;
            Registers.B = portNumberHigh;
            SetPortValue(NumberUtils.CreateUshort(portNumberLow, portNumberHigh), value);
            return Execute(opcode, 0xED);
        }
    }
}
