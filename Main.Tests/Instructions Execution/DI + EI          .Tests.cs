using NUnit.Framework;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class DI_EI_tests : InstructionsExecutionTestsBase
    {
        private const byte DI_opcode = 0xF3;
        private const byte EI_opcode = 0xFB;

        [Test]
        public void DI_resets_IFF()
        {
            Registers.IFF1 = 1;
            Registers.IFF2 = 1;

            Execute(DI_opcode);

            Assert.Multiple(() =>
            {
                Assert.That(Registers.IFF1.Value, Is.EqualTo(0));
                Assert.That(Registers.IFF2.Value, Is.EqualTo(0));
            });
        }

        [Test]
        public void EI_sets_IFF()
        {
            Registers.IFF1 = 0;
            Registers.IFF2 = 0;

            Execute(EI_opcode);

            Assert.Multiple(() =>
            {
                Assert.That(Registers.IFF1.Value, Is.EqualTo(1));
                Assert.That(Registers.IFF2.Value, Is.EqualTo(1));
            });
        }

        [Test]
        [TestCase(EI_opcode)]
        [TestCase(DI_opcode)]
        public void EI_fires_FetchFinished_with_isEiOrDi_true(byte opcode)
        {
            var eventFired = false;

            Sut.InstructionFetchFinished += (sender, e) =>
            {
                eventFired = true;
                Assert.That(e.IsEiOrDiInstruction);
            };

            Execute(opcode);

            Assert.That(eventFired);
        }

        [Test]
        [TestCase(DI_opcode)]
        [TestCase(EI_opcode)]
        public void DI_EI_do_not_change_flags(byte opcode)
        {
            AssertNoFlagsAreModified(opcode);
        }

        [Test]
        [TestCase(DI_opcode)]
        [TestCase(EI_opcode)]
        public void DI_EI_return_proper_T_states(byte opcode)
        {
            var states = Execute(opcode);
            Assert.That(states, Is.EqualTo(4));
        }
    }
}