using System;
using System.Linq;
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

        public PlainRom(byte[] contents) : this()
        {
            Array.Copy(contents, memory, contents.Length);
        }

        public PlainRom(byte[] page0contents, byte[] page1contents, byte[] page2contents, byte[] page3contents) : this()
        {
            if(page0contents != null)
                Array.Copy(page0contents, memory, page0contents.Length);

            if(page1contents != null)
                Array.Copy(page1contents, 0, memory, 0x4000, page1contents.Length);

            if(page2contents != null)
                Array.Copy(page2contents, 0, memory, 0x8000, page2contents.Length);

            if(page3contents != null)
                Array.Copy(page3contents, 0, memory, 0xC000, page3contents.Length);
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
