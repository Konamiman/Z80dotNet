using System;
using System.Collections.Generic;
using System.Linq;
using Konamiman.NestorMSX.Misc;
using Konamiman.Z80dotNet;

namespace Konamiman.NestorMSX.Hardware
{
    public class SlotsSystem : IExternallyControlledSlotsSystem
    {
        private const int PrimarySlotsCount = 4;
        private const int MinSlotPartNumber = 0;
        private const int MaxSlotPartNumber = PrimarySlotsCount - 1;
        
        private bool[] isExpanded = new bool[PrimarySlotsCount];

        private IDictionary<SlotNumber, IMemory> slotContents;

        public SlotsSystem(IDictionary<SlotNumber, IMemory> slotContents)
        {
            if(slotContents == null)
                throw new ArgumentNullException("slotContents");

            if(slotContents.Any(c => c.Value != null && c.Value.Size != Size))
                throw new ArgumentException("Size of slot contents must be 65536 bytes");

            for(int i = MinSlotPartNumber; i <= MaxSlotPartNumber; i++) {
                isExpanded[i] = slotContents.Keys.Where(s => s.PrimarySlotNumber == i).Any(s => s.IsExpandedSlot);
            }

            this.slotContents = new Dictionary<SlotNumber, IMemory>(slotContents);
            FillMissingSlotContentsWithNotConnectedMemory();
        }

        private void FillMissingSlotContentsWithNotConnectedMemory()
        {
            for(byte primary = MinSlotPartNumber; primary <= MaxSlotPartNumber; primary++) {
                if(isExpanded[primary]) {
                    for(int secondary = MinSlotPartNumber; secondary <= MaxSlotPartNumber; secondary++) {
                        var slotNumber = new SlotNumber(primary, secondary);
                        if(!ThereIsContentForSlot(slotNumber))
                            slotContents[slotNumber] = NotConnectedMemory.Value;
                    }
                }
                else if(!ThereIsContentForSlot(primary)) {
                    slotContents[primary] = NotConnectedMemory.Value;
                }
            }
        }

        bool ThereIsContentForSlot(SlotNumber slot)
        {
            return slotContents.ContainsKey(slot) && slotContents[slot] != null;
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
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public void SetContents(int startAddress, byte[] contents, int startIndex = 0, int? length = null)
        {
            throw new NotImplementedException();
        }

        public byte[] GetContents(int startAddress, int length)
        {
            throw new NotImplementedException();
        }

        public void WriteToSlotSelectionRegister(byte value)
        {
            throw new NotImplementedException();
        }

        public byte ReadSlotSelectionRegister()
        {
            throw new NotImplementedException();
        }

        public event EventHandler<SlotSelectionRegisterWrittenEventArgs> SlotSelectionRegisterWritten;

        public bool IsExpanded(TwinBit primarySlotNumber)
        {
            return isExpanded[primarySlotNumber];
        }

        public void EnableSlot(Z80Page page, SlotNumber slot)
        {
            throw new NotImplementedException();
        }

        public SlotNumber GetCurrentSlot(Z80Page page)
        {
            throw new NotImplementedException();
        }

        public IMemory GetSlotContents(SlotNumber slot)
        {
            return slotContents[slot];
        }

        public void SetSlotContents(SlotNumber slot, IMemory contents)
        {
            throw new NotImplementedException();
        }
    }
}
