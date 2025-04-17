﻿using NUnit.Framework;

namespace Konamiman.Z80dotNet.Tests
{
    public class HelloWorld
    {
        [Test]
        public void HelloWorldTest()
        {
            var Sut = new Z80Processor();
            Sut.AutoStopOnRetWithStackEmpty = true;

            var program = new byte[]
            {
                0x3E, 0x07, //LD A,7
                0xC6, 0x04, //ADD A,4
                0x3C,       //INC A
                0xC9        //RET
            };
            Sut.Memory.SetContents(0, program);
            //hola

            Sut.Start();

            Assert.Multiple(() =>
            {
                Assert.That(Sut.Registers.A, Is.EqualTo(12));
                Assert.That(Sut.TStatesElapsedSinceStart, Is.EqualTo(28));
            });
        }
    }
}
