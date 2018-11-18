namespace Konamiman.Z80dotNet.Enums
{
    /// <summary>
    /// Defines the type of a Z80 interrupt
    /// </summary>
    public enum InterruptType
    {
        /// <summary>
        /// Maskable interrupt
        /// </summary>
        Maskable = 1,

        /// <summary>
        /// Non-maskable interrupt
        /// </summary>
        NonMaskable
    }
}