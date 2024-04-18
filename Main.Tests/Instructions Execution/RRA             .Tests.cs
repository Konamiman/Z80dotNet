﻿using NUnit.Framework;
using AutoFixture;

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
                Assert.That(Registers.A & 0x7F, Is.EqualTo(values[i]));
            }
        }

        [Test]
        public void RLA_sets_bit_7_from_CF()
        {
            Registers.A = (byte)(Fixture.Create<byte>() | 0x80);
            Registers.CF = 0;
            Execute(RRA_opcode);
            Assert.That(Registers.A.GetBit(7).Value, Is.EqualTo(0));

            Registers.A = (byte)(Fixture.Create<byte>() & 0x7F);
            Registers.CF = 1;
            Execute(RRA_opcode);
            Assert.That(Registers.A.GetBit(7).Value, Is.EqualTo(1));
        }

        [Test]
        public void RRA_sets_CF_correctly()
        {
            Registers.A = 0x06;

            Execute(RRA_opcode);
            Assert.That(Registers.CF.Value, Is.EqualTo(0));

            Execute(RRA_opcode);
            Assert.That(Registers.CF.Value, Is.EqualTo(1));

            Execute(RRA_opcode);
            Assert.That(Registers.CF.Value, Is.EqualTo(1));

            Execute(RRA_opcode);
            Assert.That(Registers.CF.Value, Is.EqualTo(0));
        }

        [Test]
        public void RRA_resets_H_and_N()
        {
            AssertResetsFlags(RRA_opcode, null, "H", "N");
        }

        [Test]
        public void RRA_does_not_change_SF_ZF_PF()
        {
            AssertDoesNotChangeFlags(RRA_opcode, null, "S", "Z", "P");
        }

        [Test]
        [TestCase(0x00)]
        [TestCase(0xD7)]
        [TestCase(0x28)]
        [TestCase(0xFF)]
        public void RRA_sets_bits_3_and_5_from_A(int value)
        {
            Registers.A = (byte)value;
            Execute(RRA_opcode);
            Assert.Multiple(() =>
            {
                Assert.That(Registers.Flag3, Is.EqualTo(Registers.A.GetBit(3)));
                Assert.That(Registers.Flag5, Is.EqualTo(Registers.A.GetBit(5)));
            });
        }

        [Test]
        public void RRA_returns_proper_T_states()
        {
            var states = Execute(RRA_opcode);
            Assert.That(states, Is.EqualTo(4));
        }
    }
}