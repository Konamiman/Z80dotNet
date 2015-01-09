using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Konamiman.NestorMSX.Hardware;
using Konamiman.NestorMSX.Host;
using Konamiman.Z80dotNet;

namespace Konamiman.NestorMSX.Emulator
{
    public class MsxEmulationEnvironment
    {
        private const int BDOS = 0xFB03;    //as defined in dskbasic.mac

        private readonly Z80Processor z80;
        private readonly Tms9918 vdp;
        private readonly SlotsSystem slots;
        private readonly IKeyboardController keyboard;
        private readonly IKeyEventSource keyboardEventSource;
        private readonly DosFunctionCallExecutor dosFunctionsExecutor;
        private readonly EmulatorHostForm form;
        private readonly MsxEmulator emulator;

        public MsxEmulationEnvironment()
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

            z80.BeforeInstructionFetch += Z80OnBeforeInstructionFetch;

            var hardware = new MsxHardwareSet {
                Cpu = z80,
                KeyboardController = keyboard,
                SlotsSystem = slots,
                Vdp = vdp
            };
            emulator = new MsxEmulator(hardware);
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
            keyboardEventSource.StartGeneratingKeyEvents();
            Task.Run(() => emulator.Run());
            Application.Run(form);
        }
    }
}
