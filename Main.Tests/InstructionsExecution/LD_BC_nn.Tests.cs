using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public partial class Z80InstructionsExecutor
    {
        [Test]
        public void LD_BC_nn_works()
        {
            var oldValue = Fixture.Create<short>();
            var newValue = Fixture.Create<short>();

            Registers.Main.BC = oldValue;

            SetNextFetches(newValue.GetLowByte(), newValue.GetHighByte());
            Sut.Execute(0x01);

            Assert.AreEqual(newValue, Registers.Main.BC);
        }
    }
}
