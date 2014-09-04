using System;
using System.Linq;

namespace Konamiman.Z80dotNet
{
    /// <summary>
    /// Represents a trivial memory implementation in which all the addresses are RAM 
    /// and the values written are simply read back.
    /// </summary>
    public class PlainMemory : IMemory
    {
        //TODO: Error checking (addresses and lengths validation)
        //TODO: Add unit tests

        private readonly byte[] memory;

        public PlainMemory(int length)
        {
            memory = new byte[length];
        }

        public byte this[int address]
        {
            get
            {
                return memory[address];
            }
            set
            {
                memory[address] = value;
            }
        }

        public void SetContents(int startAddress, byte[] contents, int startIndex = 0, int? length = null)
        {
            Array.Copy(
                sourceArray: contents,
                sourceIndex: startIndex,
                destinationArray: memory,
                destinationIndex: startAddress,
                length: length ?? contents.Length
                );
            }

        public byte[] GetContents(int startAddress, int length)
        {
            return memory.Skip(startAddress).Take(length).ToArray();
        }
    }
}
