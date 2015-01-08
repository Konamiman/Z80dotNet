using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Konamiman.NestorMSX.Hardware;
using Konamiman.Z80dotNet;
using NestorMSX;

namespace Konamiman.NestorMSX.Host
{
    public class MsxEmulator
    {
        private const int BDOS = 0xFB03;    //as defined in dskbasic.mac

        private readonly Z80Processor z80;
        private readonly Tms9918 vdp;
        private readonly SlotsSystem slots;
        private readonly IKeyboardController keyboard;
        private readonly IKeyEventSource keyboardEventSource;
        private readonly DosFunctionCallExecutor dosFunctionsExecutor;
        private readonly EmulatorHostForm form;

        public MsxEmulator()
        {
            z80 = new Z80Processor();
            //z80.ClockFrequencyInMHz = 100;
            z80.ClockSynchronizer = null;

            slots = new SlotsSystem();
            slots.SetSlotContents(0, new PlainRom(File.ReadAllBytes("SpanishMsx1Bios.rom")));
            slots.SetSlotContents(1, new PlainRom(File.ReadAllBytes("dskrom.rom"), 1));
            var ram = new PlainMemory(65536);
            slots.SetSlotContents(3, ram);
            z80.Memory = slots;

            form = new EmulatorHostForm(z80);
            keyboardEventSource = form;
            vdp = new Tms9918(new DisplayRenderer(new GraphicsBasedDisplay(form)));
            form.Vdp = vdp;
            z80.RegisterInterruptSource(vdp);
         
            keyboard = new KeyboardController(form, File.ReadAllText("KeyMappings.txt"));

            dosFunctionsExecutor = new DosFunctionCallExecutor(z80.Registers, slots);

            z80.MemoryAccess += Z80OnMemoryAccess;
            z80.BeforeInstructionFetch += Z80OnBeforeInstructionFetch;
        }

        private void Z80OnBeforeInstructionFetch(object sender, BeforeInstructionFetchEventArgs eventArgs)
        {
            if(z80.Registers.PC == BDOS)
            {
                dosFunctionsExecutor.ExecuteFunctionCall();
            }
        }

        public void Run()
        {
            keyboardEventSource.Start();
            Task.Run(() => z80.Start());
            Application.Run(form);
        }

        private void Z80OnMemoryAccess(object sender, MemoryAccessEventArgs args)
        {
            if(args.EventType == MemoryAccessEventType.BeforePortRead)
                HandlePortRead(args);
            else if(args.EventType == MemoryAccessEventType.BeforePortWrite)
                HandlePortWrite(args);
        }

        private void HandlePortWrite(MemoryAccessEventArgs args)
        {
            switch(args.Address) {
                case 0x98:
                    vdp.WriteToPort(0, args.Value);
                    break;
                case 0x99:
                    vdp.WriteToPort(1, args.Value);
                    break;
                case 0xA8:
                    slots.WriteToSlotSelectionRegister(args.Value);
                    break;
                case 0xAA:
                    keyboard.WriteToKeyboardMatrixRowSelectionRegister(args.Value);
                    break;
            }
        }

        private void HandlePortRead(MemoryAccessEventArgs args)
        {
            args.CancelMemoryAccess = true;
            switch(args.Address) {
                case 0x98:
                    args.Value = vdp.ReadFromPort(0);
                    break;
                case 0x99:
                    args.Value = vdp.ReadFromPort(1);
                    break;
                case 0xA8:
                    args.Value = slots.ReadSlotSelectionRegister();
                    break;
                case 0xA9:
                    args.Value = keyboard.ReadFromKeyboardMatrixRowInputRegister();
                    break;
                default:
                    args.Value = 0xFF;
                    break;
            }
        }
    }
}
