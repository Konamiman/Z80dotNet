using System.Collections.Generic;

namespace Konamiman.Z80dotNet
{
    /// <summary>
    /// Internal class used to keep track of the current instruction execution.
    /// </summary>
    public class InstructionExecutionContext
    {
        public InstructionExecutionContext()
        {
            StopReason = StopReason.NotApplicable;
            OpcodeBytes = new List<byte>();
        }

        public StopReason StopReason
        {
            get;
            set;
        }

        public bool MustStop
        {
            get
            {
                return StopReason != StopReason.NotApplicable;
            }
        }

        public void StartNewInstruction()
        {
            OpcodeBytes.Clear();
            FetchComplete = false;
        }

        public bool FetchComplete
        {
            get;
            set;
        }

        public List<byte> OpcodeBytes
        {
            get;
            set;
        }

        public short StartOfStack
        {
            get;
            set;
        }

        public bool IsRetInstruction
        {
            get;
            set;
        }

        public bool IsLdSpInstruction
        {
            get;
            set;
        }

        public bool IsHaltInstruction
        {
            get;
            set;
        }

        public short SpAfterInstructionFetch
        {
            get;
            set;
        }
    }
}
