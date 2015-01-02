using System;
using System.Linq;
using Konamiman.NestorMSX.Misc;
using Konamiman.Z80dotNet;

namespace Konamiman.NestorMSX.Hardware
{
    /// <summary>
    /// Represents a 64K ROM memory.
    /// </summary>
    public class PlainRom : IMemory
    {
        private readonly byte[] memory;

        public PlainRom()
        {
            memory = Enumerable.Repeat<byte>(0xFF, Size).ToArray();
        }

        public PlainRom(byte[] contents, Z80Page page = null) : this()
        {
            Array.Copy(contents, 0, memory, page == null ? 0 : page.AddressMask, contents.Length);
        }

        public int Size
        {
            get
            {
                return ushort.MaxValue + 1;
            }
        }

        public byte this[int address]
        {
            get { return memory[address]; }
            set { }
        }

        public void SetContents(int startAddress, byte[] contents, int startIndex = 0, int? length = null)
        {
        }

        public byte[] GetContents(int startAddress, int length)
        {
            return memory.Skip(startAddress).Take(length).ToArray();
        }
    }
}
