using System.Security.Cryptography;
using NUnit.Framework;
using AutoFixture;

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

        public static object[] LDIR_Source =
        {
            new object[] {"LDI", (byte)0xB0},
        };

        public static object[] LDDR_Source =
        {
            new object[] {"LDD", (byte)0xB8}
        };

        [Test]
        [TestCaseSource(nameof(LDI_Source))]
        [TestCaseSource(nameof(LDD_Source))]
        [TestCaseSource(nameof(LDIR_Source))]
        [TestCaseSource(nameof(LDDR_Source))]
        public void LDI_LDD_LDIR_LDDR_copy_value_correctly(string instr, byte opcode)
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
            Assert.That(newValue, Is.EqualTo(value));
        }

        [Test]
        [TestCaseSource(nameof(LDI_Source))]
        [TestCaseSource(nameof(LDIR_Source))]
        public void LDI_LDIR_increase_DE_and_HL(string instr, byte opcode)
        {
            var srcAddress = Fixture.Create<short>();
            var destAddress = Fixture.Create<short>();

            Registers.HL = srcAddress;
            Registers.DE = destAddress;

            Execute(opcode, prefix);

            Assert.Multiple(() =>
            {
                Assert.That(Registers.HL, Is.EqualTo(srcAddress.Inc()));
                Assert.That(Registers.DE, Is.EqualTo(destAddress.Inc()));
            });
        }

        [Test]
        [TestCaseSource(nameof(LDD_Source))]
        [TestCaseSource(nameof(LDDR_Source))]
        public void LDD_LDDR_decreases_DE_and_HL(string instr, byte opcode)
        {
            var srcAddress = Fixture.Create<short>();
            var destAddress = Fixture.Create<short>();

            Registers.HL = srcAddress;
            Registers.DE = destAddress;

            Execute(opcode, prefix);

            Assert.Multiple(() =>
            {
                Assert.That(Registers.HL, Is.EqualTo(srcAddress.Dec()));
                Assert.That(Registers.DE, Is.EqualTo(destAddress.Dec()));
            });
        }

        [Test]
        [TestCaseSource(nameof(LDI_Source))]
        [TestCaseSource(nameof(LDD_Source))]
        [TestCaseSource(nameof(LDIR_Source))]
        [TestCaseSource(nameof(LDDR_Source))]
        public void LDI_LDD_LDIR_LDDR_decrease_BC(string instr, byte opcode)
        {
            var counter = Fixture.Create<short>();
            Registers.BC = counter;

            Execute(opcode, prefix);

            Assert.That(Registers.BC, Is.EqualTo(counter.Dec()));
        }

        [Test]
        [TestCaseSource(nameof(LDI_Source))]
        [TestCaseSource(nameof(LDD_Source))]
        [TestCaseSource(nameof(LDIR_Source))]
        [TestCaseSource(nameof(LDDR_Source))]
        public void LDI_LDD_LDIR_LDDR_do_not_change_S_Z_C(string instr, byte opcode)
        {
            AssertDoesNotChangeFlags(opcode, prefix, "S", "Z", "C");
        }

        [Test]
        [TestCaseSource(nameof(LDI_Source))]
        [TestCaseSource(nameof(LDD_Source))]
        [TestCaseSource(nameof(LDIR_Source))]
        [TestCaseSource(nameof(LDDR_Source))]
        public void LDI_LDD_LDIR_LDDR_reset_H_N(string instr, byte opcode)
        {
            AssertResetsFlags(opcode, prefix, "H", "N");
        }

        [Test]
        [TestCaseSource(nameof(LDI_Source))]
        [TestCaseSource(nameof(LDD_Source))]
        [TestCaseSource(nameof(LDIR_Source))]
        [TestCaseSource(nameof(LDDR_Source))]
        public void LDI_LDD_LDIR_LDDR_resets_PF_if_BC_reaches_zero(string instr, byte opcode)
        {
            Registers.BC = 128;
            for(int i = 0; i <= 256; i++)
            {
                var oldBC = Registers.BC;
                Execute(opcode, prefix);
                Assert.Multiple(() =>
                {
                    Assert.That(Registers.BC, Is.EqualTo(oldBC.Dec()));
                    Assert.That((bool)Registers.PF, Is.EqualTo(Registers.BC != 0));
                });
            }
        }

        [Test]
        [TestCaseSource(nameof(LDI_Source))]
        [TestCaseSource(nameof(LDD_Source))]
        [TestCaseSource(nameof(LDIR_Source))]
        [TestCaseSource(nameof(LDDR_Source))]
        public void LDI_LDD_LDIR_LDDR_set_Flag3_from_bit_3_of_value_plus_A_and_Flag5_from_bit_1(string instr, byte opcode)
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
                Assert.Multiple(() =>
                {
                    Assert.That(Registers.Flag3, Is.EqualTo(valuePlusA.GetBit(3)));
                    Assert.That(Registers.Flag5, Is.EqualTo(valuePlusA.GetBit(1)));
                });
            }
        }

        [Test]
        [TestCaseSource(nameof(LDIR_Source))]
        [TestCaseSource(nameof(LDDR_Source))]
        public void LDIR_LDDR_decrease_PC_by_two_if_counter_does_not_reach_zero(string instr, byte opcode)
        {
            Registers.BC = 128;
            for(int i = 0; i <= 256; i++)
            {
                var address = Fixture.Create<ushort>();
                var oldPC = Registers.PC;
                var oldBC = Registers.BC;
                ExecuteAt(address, opcode, prefix);
                Assert.Multiple(() =>
                {
                    Assert.That(Registers.BC, Is.EqualTo(oldBC.Dec()));
                    Assert.That(Registers.PC, Is.EqualTo(Registers.BC == 0 ? address.Add(2) : address));
                });
            }
        }

        [Test]
        [TestCaseSource(nameof(LDI_Source))]
        [TestCaseSource(nameof(LDD_Source))]
        public void LDI_LDD_return_proper_T_states(string instr, byte opcode)
        {
            var states = Execute(opcode, prefix);
            Assert.That(states, Is.EqualTo(16));
        }

        [Test]
        [TestCaseSource(nameof(LDIR_Source))]
        [TestCaseSource(nameof(LDDR_Source))]
        public void LDIR_LDDR_return_proper_T_states_depending_of_value_of_BC(string instr, byte opcode)
        {
            Registers.BC = 128;
            for(int i = 0; i <= 256; i++)
            {
                var oldBC = Registers.BC;
                var states = Execute(opcode, prefix);
                Assert.Multiple(() =>
                {
                    Assert.That(Registers.BC, Is.EqualTo(oldBC.Dec()));
                    Assert.That(states, Is.EqualTo(Registers.BC == 0 ? 16 : 21));
                });
            }
        }
    }
}