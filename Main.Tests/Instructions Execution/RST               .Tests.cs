﻿using NUnit.Framework;
using AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class RST_tests : InstructionsExecutionTestsBase
    {
        public static object[] RST_Source =
        {
            new object[] {0x00, (byte)0xC7},
            new object[] {0x08, (byte)0xCF},
            new object[] {0x10, (byte)0xD7},
            new object[] {0x18, (byte)0xDF},
            new object[] {0x20, (byte)0xE7},
            new object[] {0x28, (byte)0xEF},
            new object[] {0x30, (byte)0xF7},
            new object[] {0x38, (byte)0xFF},
        };

        [Test]
        [TestCaseSource(nameof(RST_Source))]
        public void RST_pushes_SP_and_jumps_to_proper_address(int address, byte opcode)
        {
            var instructionAddress = Fixture.Create<ushort>();
            var oldSP = Fixture.Create<short>();
            Registers.SP = oldSP;
            
            ExecuteAt(instructionAddress, opcode);

            Assert.Multiple(() =>
            {
                Assert.That(Registers.PC, Is.EqualTo((ushort)address));
                Assert.That(Registers.SP, Is.EqualTo(oldSP.Sub(2)));
                Assert.That(ReadShortFromMemory(Registers.SP.ToUShort()), Is.EqualTo(instructionAddress.Inc().ToShort()));
            });
        }

        [Test]
        [TestCaseSource(nameof(RST_Source))]
        public void RST_return_proper_T_states(int address, byte opcode)
        {
            var states = Execute(opcode);

            Assert.That(states, Is.EqualTo(11));
        }

        [Test]
        [TestCaseSource(nameof(RST_Source))]
        public void RST_do_not_modify_flags(int address, byte opcode)
        {
            AssertDoesNotChangeFlags(opcode);
        }
    }
}