using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public partial class Z80InstructionsExecutor
    {
        [Test]
        [TestCase("BC", "A", 0x02)]
        [TestCase("DE", "A", 0x12)]
        [TestCase("HL", "A", 0x77)]
        [TestCase("HL", "B", 0x70)]
        [TestCase("HL", "C", 0x71)]
        [TestCase("HL", "D", 0x72)]
        [TestCase("HL", "E", 0x73)]
        [TestCase("HL", "H", 0x74)]
        [TestCase("HL", "L", 0x75)]
        public void LD_arr_r_works(string destPointerReg, string srcReg, byte opcode)
        {
            var isHorL = (srcReg == "H" || srcReg == "L");

            var address = Fixture.Create<ushort>();
            var oldValue = Fixture.Create<byte>();
            var newValue = Fixture.Create<byte>();

            SetReg(destPointerReg, address.ToShort());
            ProcessorAgent.Memory[address] = oldValue;
            if(!isHorL)
                SetReg(srcReg, newValue);

            Sut.Execute(opcode);

            var expected = isHorL ? GetReg<byte>(srcReg) : newValue;
            Assert.AreEqual(expected, ProcessorAgent.Memory[address]);
        }
    }
}