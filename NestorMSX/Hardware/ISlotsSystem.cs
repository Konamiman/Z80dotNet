using System;
using Konamiman.Z80dotNet;

namespace Konamiman.NestorMSX.Hardware
{
    /// <summary>
    /// Represents the slots system of a MSX computer.
    /// </summary>
    public interface ISlotsSystem : IMemory
    {
        /// <summary>
        /// Writes the specified value in the slot selection register.
        /// </summary>
        /// <param name="value">Value to write</param>
        void WriteToSlotSelectionRegister(byte value);

        /// <summary>
        /// Reads the current value of the slot selection register.
        /// </summary>
        /// <returns>Current value of the slot selection register</returns>
        byte ReadSlotSelectionRegister();

        /// <summary>
        /// Event triggered whenever the slot selection register is written to.
        /// </summary>
        event EventHandler<SlotSelectionRegisterWrittenEventArgs> SlotSelectionRegisterWritten;
    }
}
