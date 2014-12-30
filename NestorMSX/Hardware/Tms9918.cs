using System;
using System.Timers;
using Konamiman.Z80dotNet;

namespace Konamiman.NestorMSX.Hardware
{
    public class Tms9918 : IExternallyControlledTms9918, IDisposable
    {
        private const int PatternNameTableLength = 960;
        private const int PatternGeneratorTableLength = 2048;

        private readonly ITms9918DisplayRenderer displayRenderer;
        private PlainMemory Vram;
        private bool generateInterrupts;
        private int patternNameTableAddress;
        private int patternGeneratorTableAddress;
        private byte? valueWrittenToPort1;
        private byte readAheadBuffer;
        private Timer interruptTimer;
        private byte statusRegisterValue;
        private int vramPointer;
        private Bit[] modeBits;

        public Tms9918(ITms9918DisplayRenderer displayRenderer)
        {
            Vram = new PlainMemory(16384);
            modeBits = new Bit[] {0, 0, 0};

            this.displayRenderer = displayRenderer;
            displayRenderer.BlankScreen();
            displayRenderer.SetScreenMode(0);

            interruptTimer = new Timer(50000);
            interruptTimer.Elapsed += InterruptTimerOnElapsed;
            interruptTimer.Start();
        }

        private void InterruptTimerOnElapsed(object sender, ElapsedEventArgs args)
        {
            statusRegisterValue |= 0x80;
            if(generateInterrupts)
                IntLineIsActive = true;
        }

        public event EventHandler NmiInterruptPulse;
        public bool IntLineIsActive { get; private set; }
        public byte? ValueOnDataBus { get; private set; }

        public void WriteToPort(Bit portNumber, byte value)
        {
            if(portNumber == 0) {
                WriteVram(vramPointer, value);
                vramPointer = (vramPointer++) & 0x3FFF;
                readAheadBuffer = value;
                valueWrittenToPort1 = null;
                return;
            }

            if(valueWrittenToPort1 == null) {
                valueWrittenToPort1 = value;
                return;
            }

            if((value & 0x80) == 0) {
                SetVramAccess(valueWrittenToPort1.Value, value);
            } else {
                WriteControlRegister(valueWrittenToPort1.Value, value);
            }

            valueWrittenToPort1 = null;
        }

        private void SetVramAccess(byte firstByte, byte secondByte)
        {
            vramPointer = firstByte | ((secondByte & 0x3F) << 8);

            if((secondByte & 0x40) == 0) {
                readAheadBuffer = Vram[vramPointer];
                vramPointer = (vramPointer++) & 0x3FFF;
            }
        }

        private void WriteControlRegister(byte value, byte register)
        {
            register &= 7;

            switch(register) {
                case 0:
                    SetModeBit(2, value.GetBit(1));
                    break;

                case 1:
                    SetModeBit(1, value.GetBit(4));
                    SetModeBit(3, value.GetBit(3));

                    generateInterrupts = value.GetBit(5);
                    if(generateInterrupts && statusRegisterValue.GetBit(7))
                        IntLineIsActive = true;
                    else
                        IntLineIsActive = false;

                    if(value.GetBit(6))
                        displayRenderer.ActivateScreen();
                    else
                        displayRenderer.BlankScreen();

                    break;

                case 2:
                    patternNameTableAddress = value << 10;
                    break;

                case 4:
                    patternGeneratorTableAddress = value << 11;
                    break;

                case 7:
                    displayRenderer.SetBackgroundColor((byte)(value & 0x0F));
                    displayRenderer.SetForegroundColor((byte)(value >> 4));
                    break;
            }
        }

        private void SetModeBit(int mode, Bit value)
        {
            modeBits[mode - 1] = value;

            for(byte i = 0; i <= 3; i++) {
                if(modeBits[i]) {
                    displayRenderer.SetScreenMode((byte)(i + 1));
                    return;
                }
            }

            displayRenderer.SetScreenMode(0);
        }

        public byte ReadFromPort(Bit portNumber)
        {
            valueWrittenToPort1 = null;

            if(portNumber == 0) {
                var value = readAheadBuffer;
                vramPointer = (vramPointer++) & 0x3FFF;
                readAheadBuffer = Vram[vramPointer];
                return value;
            }
            else {
                var value = statusRegisterValue;
                statusRegisterValue &= 0x7F;
                IntLineIsActive = false;
                return value;
            }
        }

        public void WriteVram(int address, byte value)
        {
            Vram[address] = value;
            if(address >= patternNameTableAddress && address < patternNameTableAddress + PatternNameTableLength) {
                displayRenderer.WriteToNameTable(address - patternNameTableAddress, value);
            }
            else if(address >= patternGeneratorTableAddress && address < patternGeneratorTableAddress + PatternGeneratorTableLength) {
                displayRenderer.WriteToPatternGeneratorTable(address - patternGeneratorTableAddress, value);
            }
        }

        public byte ReadVram(int address)
        {
            return Vram[address];
        }

        public byte[] GetVramContents(int startAddress, int length)
        {
            return Vram.GetContents(startAddress, length);
        }

        public void SetVramContents(int startAddress, byte[] contents, int startIndex = 0, int? length = null)
        {
            Vram.SetContents(startAddress, contents, startAddress, length);
        }

        public void Dispose()
        {
            interruptTimer.Dispose();
        }
    }
}
