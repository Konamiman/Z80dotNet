using Konamiman.Z80dotNet;

namespace Konamiman.NestorMSX.Emulator
{
    public class MsxEmulator
    {
        private readonly MsxHardwareSet hardware;

        public MsxEmulator(MsxHardwareSet hardware)
        {
            this.hardware = hardware;
            hardware.Cpu.MemoryAccess += Z80OnMemoryAccess;
        }

        public void Run()
        {
            hardware.Cpu.Start();
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
                    hardware.Vdp.WriteToPort(0, args.Value);
                    break;
                case 0x99:
                    hardware.Vdp.WriteToPort(1, args.Value);
                    break;
                case 0xA8:
                    hardware.SlotsSystem.WriteToSlotSelectionRegister(args.Value);
                    break;
                case 0xAA:
                    hardware.KeyboardController.WriteToKeyboardMatrixRowSelectionRegister(args.Value);
                    break;
            }
        }

        private void HandlePortRead(MemoryAccessEventArgs args)
        {
            args.CancelMemoryAccess = true;
            switch(args.Address) {
                case 0x98:
                    args.Value = hardware.Vdp.ReadFromPort(0);
                    break;
                case 0x99:
                    args.Value = hardware.Vdp.ReadFromPort(1);
                    break;
                case 0xA8:
                    args.Value = hardware.SlotsSystem.ReadSlotSelectionRegister();
                    break;
                case 0xA9:
                    args.Value = hardware.KeyboardController.ReadFromKeyboardMatrixRowInputRegister();
                    break;
                default:
                    args.Value = 0xFF;
                    break;
            }
        }
    }
}
