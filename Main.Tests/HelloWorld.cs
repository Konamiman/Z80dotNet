using System;
using NUnit.Framework;

namespace Konamiman.Z80dotNet.Tests
{
    public class HelloWorld
    {
        [Test]
        public void HelloWorldTest()
        {
            var Sut = new Z80Processor();
            Sut.AutoStopOnRetWithStackEmpty = true;
            Sut.InstructionExecutor = new HelloWorldInstructionExecutor();

            var program = new byte[]
            {
                0x3E, 0x07, //LD A,7
                0xC6, 0x04, //ADD A,4
                0x3C,       //INC A
                0xC9        //RET
            };
            Sut.Memory.SetContents(0, program);

            Sut.Start();

            Assert.AreEqual(12, Sut.Registers.Main.A);
            Assert.AreEqual(25, Sut.TStatesElapsedSinceStart);
        }

        //TODO: Remove this class and use the normal Z80InstructionExecutor when it has all the appropriate instructions implemented.
        private class HelloWorldInstructionExecutor : IZ80InstructionExecutor
        {
            public IZ80ProcessorAgent ProcessorAgent { get; set; }

            public int Execute(byte firstOpcodeByte)
            {
                byte value;

                switch (firstOpcodeByte)
                {
                    case 0x3E: //LD A,n
                        value = ProcessorAgent.FetchNextOpcode();
                        FetchFinished();
                        ProcessorAgent.Registers.Main.A = value;
                        return 7;
                    case 0xC6: //ADD A,n
                        value = ProcessorAgent.FetchNextOpcode();
                        FetchFinished();
                        ProcessorAgent.Registers.Main.A += value; //TODO: Check for overflow, set flags
                        return 4;
                    case 0x3C: //INC A
                        FetchFinished();
                        ProcessorAgent.Registers.Main.A++; //TODO: Check for overflow, set flags
                        return 4;
                    case 0xC9: //RET
                        FetchFinished(true);
                        //TODO: Update PC and SP
                        return 10;
                    default: //treat as NOP
                        return 4;
                }
            }


            private void FetchFinished(bool isRet = false, bool isHalt = false, bool isLdSp = false)
            {
                InstructionFetchFinished(this, new InstructionFetchFinishedEventArgs()
                {
                    IsRetInstruction = isRet,
                    IsHaltInstruction = isHalt,
                    IsLdSpInstruction = isLdSp
                });
            }

            public event EventHandler<InstructionFetchFinishedEventArgs> InstructionFetchFinished;
        }
    }
}
