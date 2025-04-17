using NUnit.Framework;
using AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class RET_and_RET_cc_tests : InstructionsExecutionTestsBase
    {
        private const int RETI_opcode = 0x4D ;
        private const int RETI_prefix = 0xED;

        public static object[] RET_cc_Source =
        {
            new object[] {"Z", (byte)0xC0, 0},
            new object[] {"C", (byte)0xD0, 0},
            new object[] {"P", (byte)0xE0, 0},
            new object[] {"S", (byte)0xF0, 0},
            new object[] {"Z", (byte)0xC8, 1},
            new object[] {"C", (byte)0xD8, 1},
            new object[] {"P", (byte)0xE8, 1},
            new object[] {"S", (byte)0xF8, 1},
        };

        public static object[] RET_Source =
        {
            new object[] {null, (byte)0xC9, 0},
        };

        [Test]
        [TestCaseSource(nameof(RET_cc_Source))]
        public void RET_cc_does_not_return_if_flag_not_set(string flagName, byte opcode, int flagValue)
        {
            var instructionAddress = Fixture.Create<ushort>();
            var oldSP = Registers.SP;

            SetFlag(flagName, !(Bit)flagValue);
            ExecuteAt(instructionAddress, opcode);

            Assert.Multiple(() =>
            {
                Assert.That(Registers.PC, Is.EqualTo(instructionAddress.Inc()));
                Assert.That(Registers.SP, Is.EqualTo(oldSP));
            });
        }

        [Test]
        [TestCaseSource(nameof(RET_cc_Source))]
        public void RET_cc_returns_proper_T_states_if_no_jump_is_made(string flagName, byte opcode, int flagValue)
        {
            SetFlag(flagName, !(Bit)flagValue);
            var states = Execute(opcode);

            Assert.That(states, Is.EqualTo(5));
        }

        [Test]
        [TestCaseSource(nameof(RET_cc_Source))]
        [TestCaseSource(nameof(RET_Source))]
        public void RET_cc_returns_to_proper_address_if_flag_is_set_RET_return_always(string flagName, byte opcode, int flagValue)
        {
            var instructionAddress = Fixture.Create<ushort>();
            var returnAddress = Fixture.Create<ushort>();
            var oldSP = Fixture.Create<short>();

            Registers.SP = oldSP;
            SetMemoryContentsAt(oldSP.ToUShort(), returnAddress.GetLowByte());
            SetMemoryContentsAt(oldSP.ToUShort().Inc(), returnAddress.GetHighByte());

            SetFlagIfNotNull(flagName, flagValue);
            ExecuteAt(instructionAddress, opcode);

            Assert.Multiple(() =>
            {
                Assert.That(Registers.PC, Is.EqualTo(returnAddress));
                Assert.That(Registers.SP, Is.EqualTo(oldSP.Add(2)));
            });
        }

        private void SetFlagIfNotNull(string flagName, int flagValue)
        {
            if (flagName != null) SetFlag(flagName, flagValue);
        }
        
        [Test]
        [TestCaseSource(nameof(RET_cc_Source))]
        [TestCaseSource(nameof(RET_Source))]
        public void RET_and_RET_cc_return_proper_T_states_if_jump_is_made(string flagName, byte opcode, int flagValue)
        {
            SetFlagIfNotNull(flagName, flagValue);
            var states = Execute(opcode);

            Assert.That(states, Is.EqualTo(flagName == null ? 10 : 11));
        }

        [Test]
        [TestCaseSource(nameof(RET_cc_Source))]
        [TestCaseSource(nameof(RET_Source))]
        public void RET_and_RET_cc_do_not_modify_flags(string flagName, byte opcode, int flagValue)
        {
            Registers.F = Fixture.Create<byte>();
            SetFlagIfNotNull(flagName, flagValue);
            var value = Registers.F;

            Execute(opcode);

            Assert.That(Registers.F, Is.EqualTo(value));
        }

        [Test]
        [TestCaseSource(nameof(RET_cc_Source))]
        [TestCaseSource(nameof(RET_Source))]
        public void RET_fires_FetchFinished_with_isRet_true_if_flag_is_set(string flagName, byte opcode, int flagValue)
        {
            var eventFired = false;

            Sut.ProcessorAgent.Registers.F = 255;
            Sut.InstructionFetchFinished += (sender, e) =>
            {
                eventFired = true;
                if((opcode & 0x0F) == 0)
                    Assert.That(e.IsRetInstruction, Is.False);
                else
                    Assert.That(e.IsRetInstruction, Is.True);
            };

            Execute(opcode);

            Assert.That(eventFired);
        }

        [Test]
        public void RETI_returns_to_pushed_address()
        {
            var instructionAddress = Fixture.Create<ushort>();
            var returnAddress = Fixture.Create<ushort>();
            var oldSP = Fixture.Create<short>();

            Registers.SP = oldSP;
            SetMemoryContentsAt(oldSP.ToUShort(), returnAddress.GetLowByte());
            SetMemoryContentsAt(oldSP.ToUShort().Inc(), returnAddress.GetHighByte());

            ExecuteAt(instructionAddress, RETI_opcode, RETI_prefix);

            Assert.Multiple(() =>
            {
                Assert.That(Registers.PC, Is.EqualTo(returnAddress));
                Assert.That(Registers.SP, Is.EqualTo(oldSP.Add(2)));
            });
        }

        [Test]
        public void RETI_returns_proper_T_states()
        {
            var states = Execute(RETI_opcode, RETI_prefix);
            Assert.That(states, Is.EqualTo(14));
        }
    }
}