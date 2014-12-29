using System.Linq;
using Konamiman.Z80dotNet;

namespace Konamiman.NestorMSX.Hardware
{
    /// <summary>
    /// Represents a memory that is not connected to anything.
    /// Writing has no effect and reading always yields 0xFF;
    /// </summary>
    public class NotConnectedMemory : IMemory
    {
        /// <summary>
        /// Gets the only existing instance of this class.
        /// </summary>
        public static NotConnectedMemory Value { get; private set; }

        static NotConnectedMemory()
        {
            Value = new NotConnectedMemory();
        }

        private NotConnectedMemory()
        {
        }

        public int Size { get { return ushort.MaxValue; }}

        public byte this[int address]
        {
            get { return 0xFF; }
            set { }
        }

        public void SetContents(int startAddress, byte[] contents, int startIndex = 0, int? length = null)
        {
        }

        public byte[] GetContents(int startAddress, int length)
        {
            return Enumerable.Repeat<byte>(0xFF, length).ToArray();
        }
    }
}
