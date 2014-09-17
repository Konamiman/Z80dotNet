using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public partial class Z80InstructionsExecutor
    {
        [Test]
        [TestCase("BC", 0x01)]
        [TestCase("DE", 0x11)]
        [TestCase("HL", 0x21)]
        [TestCase("SP", 0x31)]
        public void LD_rr_nn_works(string reg, byte opcode)
        {
            var oldValue = Fixture.Create<short>();
            var newValue = Fixture.Create<short>();

            SetReg(reg, oldValue);

            SetNextFetches(newValue.GetLowByte(), newValue.GetHighByte());
            Sut.Execute(opcode);

            Assert.AreEqual(newValue, GetReg<short>(reg));
        }
    }
}
