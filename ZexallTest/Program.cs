using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Konamiman.Z80dotNet.ZexallTest
{
    class Program
    {
        private static byte DollarCode;

        static void Main(string[] args)
        {
            (new Program()).Run(args);
        }

        private void Run(string[] args)
        {
            var fileName = args.Length == 0 ? "zexall.com" : args[0];

            DollarCode = Encoding.ASCII.GetBytes(new[] {'$'})[0];

            var z80 = new Z80Processor();
            z80.ClockSynchronizer = new DummyClockSynchronizer();
            z80.AutoStopOnRetWithStackEmpty = true;
            
            var program = File.ReadAllBytes(fileName);
            z80.Memory.SetContents(0x100, program);

            z80.Memory[6] = 0xFF;
            z80.Memory[7] = 0xFF;

            z80.BeforeInstructionFetch += Z80OnBeforeInstructionFetch;

            z80.Reset();
            z80.Registers.PC = 0x100;
            z80.Continue();
        }

        private void Z80OnBeforeInstructionFetch(object sender, BeforeInstructionFetchEventArgs args)
        {
            //Absolutely minimum implementation of CP/M for ZEXALL and ZEXDOC to work

            var z80 = (IZ80Processor)sender;

            if(z80.Registers.PC == 0)
                args.ExecutionStopper.Stop();
            else if(z80.Registers.PC != 5)
                return;

            var function = z80.Registers.C;

            if(function == 9) {
                var messageAddress = z80.Registers.DE;
                var bytesToPrint = new List<byte>();
                byte byteToPrint;
                while((byteToPrint = z80.Memory[messageAddress]) != DollarCode) {
                    bytesToPrint.Add(byteToPrint);
                    messageAddress++;
                }

                var stringToPrint = Encoding.ASCII.GetString(bytesToPrint.ToArray());
                Console.Write(stringToPrint);
            }
            else if(function == 2) {
                var byteToPrint = z80.Registers.E;
                var charToPrint = Encoding.ASCII.GetString(new[] {byteToPrint})[0];
                Console.Write(charToPrint);
            }

            z80.ExecuteRet();
        }
    }
}
