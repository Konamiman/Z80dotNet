using NUnit.Framework;
using AutoFixture;

namespace Konamiman.Z80dotNet.Tests
{
    public class Z80ProcessorTests_Utils
    {
        public Fixture Fixture { get; set; }

        public Z80Processor Sut { get; set; }

        [SetUp]
        public void SetUp()
        {
            Fixture = new Fixture();
            Sut = new Z80Processor();
        }

        [Test]
        public void ExecuteCall_pushes_PC_and_jumps()
        {
            var oldPC = Fixture.Create<ushort>();
            var callAddress = Fixture.Create<ushort>();
            var oldSP = Fixture.Create<short>();
            Sut.Registers.SP = oldSP;
            Sut.Registers.PC = oldPC;

            Sut.ExecuteCall(callAddress);

            Assert.Multiple(() =>
            {
                Assert.That(Sut.Registers.PC, Is.EqualTo(callAddress));
                Assert.That(Sut.Registers.SP, Is.EqualTo(oldSP.Sub(2)));
                Assert.That(ReadShortFromMemory(Sut.Registers.SP.ToUShort()), Is.EqualTo(oldPC.ToShort()));
            });
        }

        [Test]
        public void ExecuteRet_pops_PC()
        {
            var pushedAddress = Fixture.Create<ushort>();
            var sp = Fixture.Create<short>();

            Sut.Registers.SP = sp;
            Sut.Memory[sp.ToUShort()] = pushedAddress.GetLowByte();
            Sut.Memory[sp.ToUShort().Inc()] = pushedAddress.GetHighByte();

            Sut.ExecuteRet();

            Assert.Multiple(() =>
            {
                Assert.That(Sut.Registers.PC, Is.EqualTo(pushedAddress));
                Assert.That(Sut.Registers.SP, Is.EqualTo(sp.Add(2)));
            });
        }

        private short ReadShortFromMemory(ushort address)
        {
            return NumberUtils.CreateShort(Sut.Memory[address], Sut.Memory[address.Inc()]);
        }
    }
}
