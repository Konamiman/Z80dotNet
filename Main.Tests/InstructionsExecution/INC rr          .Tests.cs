using NUnit.Framework;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public partial class Z80InstructionsExecutor
    {
        [Test]
        [TestCase("BC", 0x03)]
        [TestCase("DE", 0x13)]
        [TestCase("HL", 0x23)]
        [TestCase("SP", 0x33)]
        [TestCase("IX", 0xDD)]
        [TestCase("IY", 0xFD)]
        public void INC_rr_works(string reg, byte opcode)
        {
            SetReg(reg, 0xFFFF.ToShort());
            if(reg.StartsWith("I"))
                SetNextFetches(0x23);
            Sut.Execute(opcode);
            Assert.AreEqual(0, GetReg<short>(reg));
        }
    }
}
