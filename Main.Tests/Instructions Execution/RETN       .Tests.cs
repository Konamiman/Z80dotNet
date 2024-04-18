using NUnit.Framework;
using AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class RETN_tests : InstructionsExecutionTestsBase
    {
        private const byte opcode = 0x45;
        private const byte prefix = 0xED;

        [Test]
        public void RETN_returns_to_proper_address()
        {
            var instructionAddress = Fixture.Create<ushort>();
            var returnAddress = Fixture.Create<ushort>();
            var oldSP = Fixture.Create<short>();

            Registers.SP = oldSP;
            SetMemoryContentsAt(oldSP.ToUShort(), returnAddress.GetLowByte());
            SetMemoryContentsAt(oldSP.ToUShort().Inc(), returnAddress.GetHighByte());

            ExecuteAt(instructionAddress, opcode, prefix);

            Assert.Multiple(() =>
            {
                Assert.That(Registers.PC, Is.EqualTo(returnAddress));
                Assert.That(Registers.SP, Is.EqualTo(oldSP.Add(2)));
            });
        }

        [Test]
        public void RETN_returns_proper_T_states()
        {
            var states = Execute(opcode, prefix);

            Assert.That(states, Is.EqualTo(14));
        }

        [Test]
        public void RETN_does_not_modify_flags()
        {
            AssertDoesNotChangeFlags(opcode, prefix);
        }

        [Test]
        public void RETN_fires_FetchFinished_with_isRet_true()
        {
            var eventFired = false;

            Sut.InstructionFetchFinished += (sender, e) =>
            {
                eventFired = true;
                Assert.That(e.IsRetInstruction);
            };

            Execute(opcode, prefix);

            Assert.That(eventFired);
        }

        [Test]
        [TestCase(0, 1)]
        [TestCase(1, 0)]
        public void RETN_copies_IFF2_to_IFF1(int initialIFF1, int initialIFF2)
        {
            Registers.IFF1 = initialIFF1;
            Registers.IFF2 = initialIFF2;

            Execute(opcode, prefix);

            Assert.Multiple(() =>
            {
                Assert.That(Registers.IFF2.Value, Is.EqualTo(initialIFF2));
                Assert.That(Registers.IFF1, Is.EqualTo(Registers.IFF2));
            });
        }
    }
}