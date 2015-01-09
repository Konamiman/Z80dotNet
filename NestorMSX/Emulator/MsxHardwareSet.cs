using Konamiman.NestorMSX.Hardware;
using Konamiman.Z80dotNet;

namespace Konamiman.NestorMSX.Emulator
{
    /// <summary>
    /// Represents the hardware controlled by an MSX computer.
    /// </summary>
    public class MsxHardwareSet
    {
        public IZ80Processor Cpu { get; set; }

        public ISlotsSystem SlotsSystem { get; set; }

        public ITms9918 Vdp { get; set; }

        public IKeyboardController KeyboardController { get; set; }
    }
}
