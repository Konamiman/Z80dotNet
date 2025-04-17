﻿using NUnit.Framework;
using AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class DEC_r_tests : InstructionsExecutionTestsBase
    {
        public static object[] DEC_r_Source =
        {
            new object[] {"A",   (byte)0x3D, (byte?)null},
            new object[] {"B",   (byte)0x05, (byte?)null},
            new object[] {"C",   (byte)0x0D, (byte?)null},
            new object[] {"D",   (byte)0x15, (byte?)null},
            new object[] {"E",   (byte)0x1D, (byte?)null},
            new object[] {"H",   (byte)0x25, (byte?)null},
            new object[] {"L",   (byte)0x2D, (byte?)null},
            new object[] {"IXH", (byte)0x25, (byte?)0xDD},
            new object[] {"IXL", (byte)0x2D, (byte?)0xDD},
            new object[] {"IYH", (byte)0x25, (byte?)0xFD},
            new object[] {"IYL", (byte)0x2D, (byte?)0xFD}
        };

        [Test]
        [TestCaseSource(nameof(DEC_r_Source))]
        public void DEC_r_decreases_value_appropriately(string reg, byte opcode, byte? prefix)
        {
            SetReg(reg, 0x01);
            Execute(opcode, prefix);
            Assert.That(GetReg<byte>(reg), Is.EqualTo(0x00));

            Execute(opcode, prefix);
            Assert.That(GetReg<byte>(reg), Is.EqualTo(0xFF));

            Execute(opcode, prefix);
            Assert.That(GetReg<byte>(reg), Is.EqualTo(0xFE));
        }

        [Test]
        [TestCaseSource(nameof(DEC_r_Source))]
        public void DEC_r_sets_SF_appropriately(string reg, byte opcode, byte? prefix)
        {
            SetReg(reg, 0x02);

            Execute(opcode, prefix);
            Assert.That(Registers.SF.Value, Is.EqualTo(0));

            Execute(opcode, prefix);
            Assert.That(Registers.SF.Value, Is.EqualTo(0));

            Execute(opcode, prefix);
            Assert.That(Registers.SF.Value, Is.EqualTo(1));

            Execute(opcode, prefix);
            Assert.That(Registers.SF.Value, Is.EqualTo(1));
        }

        [Test]
        [TestCaseSource(nameof(DEC_r_Source))]
        public void DEC_r_sets_ZF_appropriately(string reg, byte opcode, byte? prefix)
        {
            SetReg(reg, 0x03);

            Execute(opcode, prefix);
            Assert.That(Registers.ZF.Value, Is.EqualTo(0));

            Execute(opcode, prefix);
            Assert.That(Registers.ZF.Value, Is.EqualTo(0));

            Execute(opcode, prefix);
            Assert.That(Registers.ZF.Value, Is.EqualTo(1));

            Execute(opcode, prefix);
            Assert.That(Registers.ZF.Value, Is.EqualTo(0));
        }

        [Test]
        [TestCaseSource(nameof(DEC_r_Source))]
        public void DEC_r_sets_HF_appropriately(string reg, byte opcode, byte? prefix)
        {
            foreach(byte b in new byte[] { 0x11, 0x81, 0xF1 }) 
            {
                SetReg(reg, b);

                Execute(opcode, prefix);
                Assert.That(Registers.HF.Value, Is.EqualTo(0));

                Execute(opcode, prefix);
                Assert.That(Registers.HF.Value, Is.EqualTo(1));

                Execute(opcode, prefix);
                Assert.That(Registers.HF.Value, Is.EqualTo(0));
            }
        }

        [Test]
        [TestCaseSource(nameof(DEC_r_Source))]
        public void DEC_r_sets_PF_appropriately(string reg, byte opcode, byte? prefix)
        {
            SetReg(reg, 0x81);

            Execute(opcode, prefix);
            Assert.That(Registers.PF.Value, Is.EqualTo(0));

            Execute(opcode, prefix);
            Assert.That(Registers.PF.Value, Is.EqualTo(1));

            Execute(opcode, prefix);
            Assert.That(Registers.PF.Value, Is.EqualTo(0));
        }

        [Test]
        [TestCaseSource(nameof(DEC_r_Source))]
        public void DEC_r_sets_NF(string reg, byte opcode, byte? prefix)
        {
            var randomValues = Fixture.Create<byte[]>();

            foreach (var value in randomValues)
            {
                SetReg(reg, value);
                Registers.NF = 0;

                Execute(opcode, prefix);
                Assert.That(Registers.NF.Value, Is.EqualTo(1));
            }
        }

        [Test]
        [TestCaseSource(nameof(DEC_r_Source))]
        public void DEC_r_does_not_chance_CF(string reg, byte opcode, byte? prefix)
        {
            var randomValues = Fixture.Create<byte[]>();

            foreach (var value in randomValues)
            {
                SetReg(reg, value);

                Registers.CF = 0;
                Execute(opcode, prefix);
                Assert.That(Registers.CF.Value, Is.EqualTo(0));

                Registers.CF = 1;
                Execute(opcode, prefix);
                Assert.That(Registers.CF.Value, Is.EqualTo(1));
            }
        }

        [Test]
        [TestCaseSource(nameof(DEC_r_Source))]
        public void DEC_r_sets_bits_3_and_5_from_result(string reg, byte opcode, byte? prefix)
        {
            SetReg(reg, ((byte)1).WithBit(3, 1).WithBit(5, 0));
            Execute(opcode, prefix);
            Assert.Multiple(() =>
            {
                Assert.That(Registers.Flag3.Value, Is.EqualTo(1));
                Assert.That(Registers.Flag5.Value, Is.EqualTo(0));
            });

            SetReg(reg, ((byte)1).WithBit(3, 0).WithBit(5, 1));
            Execute(opcode, prefix);
            Assert.Multiple(() =>
            {
                Assert.That(Registers.Flag3.Value, Is.EqualTo(0));
                Assert.That(Registers.Flag5.Value, Is.EqualTo(1));
            });
        }

        [Test]
        [TestCaseSource(nameof(DEC_r_Source))]
        public void DEC_r_returns_proper_T_states(string reg, byte opcode, byte? prefix)
        {
            var states = Execute(opcode, prefix);
            Assert.That(states, Is.EqualTo(IfIndexRegister(reg, 8, @else: 4)));
        }
    }
}
