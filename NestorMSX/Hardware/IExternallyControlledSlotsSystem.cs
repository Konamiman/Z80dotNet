using Konamiman.NestorMSX.Misc;
using Konamiman.Z80dotNet;

namespace Konamiman.NestorMSX.Hardware
{
    /// <summary>
    /// Represents a MSX computer slots system that can be controlled externally,
    /// that is, by explicit slot selection and content methods.
    /// </summary>
    public interface IExternallyControlledSlotsSystem : ISlotsSystem
    {
        /// <summary>
        /// Enables a given slot on a given page.
        /// </summary>
        /// <param name="page">Page number to enable the slot in</param>
        /// <param name="slot">Slot number to enable</param>
        void EnableSlot(Z80Page page, SlotNumber slot);

        /// <summary>
        /// Gets the slot number currently enabled in a given page.
        /// </summary>
        /// <param name="page">Page to get the slot currently enabled in</param>
        /// <returns></returns>
        SlotNumber GetCurrentSlot(Z80Page page);

        /// <summary>
        /// Gets the contents of a given slot.
        /// </summary>
        /// <param name="slot">Slot number to get the contents</param>
        /// <returns>Contents of the slot</returns>
        IMemory GetSlotContents(SlotNumber slot);

        /// <summary>
        /// Sets the contents of a given slot.
        /// </summary>
        /// <param name="slot">Slot whose contents are to be set</param>
        /// <param name="contents">Contents to set</param>
        void SetSlotContents(SlotNumber slot, IMemory contents);
    }
}
