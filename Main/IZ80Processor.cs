using System;

namespace Konamiman.Z80dotNet
{
    /// <summary>
    /// Represents a Z80 processor class that can be used to develop processor simulators or computer emulators.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The Z80 processor class is intended to be used in synchronous mode and controlled by events.
    /// You simply configure the instance, subscribe to the <see cref="IZ80Processor.MemoryAccess"/> and
    /// <see cref="IZ80Processor.InstructionExecution"/> events and invoke the <see cref="IZ80Processor.Start"/> method.
    /// The method returns when the processor stops execution for whatever reason (see <see cref="IZ80Processor.StopReason"/>).
    /// </para>
    /// The <see cref="IZ80Processor.MemoryAccess"/> and <see cref="IZ80Processor.InstructionExecution"/> events 
    /// provide full control of the memory and ports access and the instructions executions. During these events
    /// you can examine and alter the memory contents and even stop the processor execution
    /// (see the <see cref="IZ80Processor.Stop"/> method).
    /// <para>
    /// An alternative way of using the class is to use the <see cref="IZ80Processor.ExecuteNextInstruction"/> method.
    /// This method will simply execute the next instruction (as pointed by the PC register, see <see cref="IZ80Processor.Registers"/>)
    /// and then returns immediately. This can be useful to allow for step-by-step debugging of Z80 code.
    /// </para>
    /// </remarks>
    public interface IZ80Processor
    {
        #region Processor control

        /// <summary>
        /// Performs a reset (see <see cref="IZ80Processor.Reset"/>) and sets the processor in running state.
        /// This method cannot be invoked from an event handler.
        /// </summary>
        /// <remarks>
        /// The method will finish when the <see cref="IZ80Processor.Stop"/> method is invoked from within an
        /// event handler, or when a HALT instruction is executed with the interrupts disabled (only if
        /// <see cref="IZ80Processor.AutoStopOnDiPlusHalt"/> is true).
        /// </remarks>
        /// <param name="globalState">If this value is not null, it will be copied to the
        /// <see cref="IZ80Processor.UserState"/> property.
        /// </param>
        /// <exception cref="InvalidOperationException">The method is invoked from within an event handler.</exception>
        void Start(object globalState = null);

        /// <summary>
        /// Stops the processor execution, causing the <see cref="IZ80Processor.Start"/> method to return.
        /// This method can only be invoked from an event handler.
        /// </summary>
        /// <remarks>
        /// If the method is executed from a <see cref="IZ80Processor.InstructionExecution"/> event, the processor
        /// execution will stop immediately. Otherwise it will stop after the current instruction finishes executing.
        /// </remarks>
        /// <param name="isPause">If true, the <see cref="IZ80Processor.StopReason"/> property of the
        /// processor classs will return <see cref="StopReason.PauseInvoked"/> after the method returns.
        /// Otherwise, it will return <see cref="StopReason.StopInvoked"/>.</param>
        /// <exception cref="InvalidOperationException">The method is not invoked from within an event handler.</exception>
        void Stop(bool isPause = false);

        /// <summary>
        /// Sets the processor in running state without first doing a reset, thus preserving the state of all the registers.
        /// This method cannot be invoked from an event handler.
        /// </summary>
        /// <exception cref="InvalidOperationException">The method is invoked from within an event handler, or the
        /// <see cref="IZ80Processor.Start"/> method has never been invoked since the processor object was created.</exception>
        void Continue();

        /// <summary>
        /// Resets the registers to its initial state. The running state is not modified.
        /// </summary>
        /// <para>
        /// This method sets the PC, IFF1, IFF2 and IM registers to 0, and all other registers to FFFFh.
        /// </para>
        /// <para>
        /// If the method is executed from a <see cref="IZ80Processor.MemoryAccess"/> event, the reset
        /// will be effective after the current instruction execution finishes.
        /// </para>
        void Reset();

        /// <summary>
        /// Executes the next instruction as pointed by the PC register, and the returns.
        /// This method cannot be invoked from an event handler.
        /// </summary>
        /// <remarks>
        /// <para>
        /// During the execution of this method, the <see cref="IZ80Processor.MemoryAccess"/> and
        /// <see cref="IZ80Processor.InstructionExecution"/> events will be triggered as usual.
        /// Altough not necessary, it is possible to invoke the <see cref="IZ80Processor.Stop"/> method,
        /// thus modifying the value of <see cref="IZ80Processor.StopReason"/>.
        /// </para>
        /// <para>
        /// This method will never issue a reset. A manual call to <see cref="IZ80Processor.Reset"/> is needed
        /// before the first invocation of this method if <see cref="IZ80Processor.Start"/> has never been invoked.
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">The method is invoked from within an event handler, or the
        /// <see cref="IZ80Processor.Start"/> method has never been invoked since the processor object was created.</exception>
        void ExecuteNextInstruction();

        #endregion

        // Info and state

        ulong TStatesElapsedSinceStart { get; }

        ulong TStatesElapsedSinceReset { get; }

        StopReason StopReason { get; }

        ProcessorState State { get; }

        object UserState { get; set; }

        // Inside and outside world

        Z80Registers Registers { get; set; }

        Z80Lines Lines { get; set; }

        byte[] Memory { get; }

        void SetMemoryContents(ushort startAddress, byte[] contents, ushort startIndex = 0, ushort? length = null);

        void SetMemoryMode(ushort startAddress, ushort length, MemoryMode mode);

        // Config

        decimal ClockFrequencyInMHz { get; set; }

        decimal ClockSpeedFactor { get; set; }

        bool AutoStopOnDiPlusHalt { get; set; }

        int MemoryWaitStates { get; set; }

        // Events

        event EventHandler<MemoryAccessEventArgs> MemoryAccess;

        event EventHandler<InstructionExecutionEventArgs> InstructionExecution;

    }

    public enum ProcessorState
    {
        Stopped,
        Paused,
        Running,
        ExecutingOneInstruction
    }

    public enum StopReason
    {
        NeverRan,
        StopInvoked,
        PauseInvoked,
        ExecuteNextInstructionInvoked,
        DiPlusHalt
    }

    public enum MemoryMode
    {
        RAM,
        ROM,
        NotConnected
    }

    public class InstructionExecutionEventArgs : ProcessorEventArgs
    {
        public InstructionExecutionOperation Operation { get; private set; }

        public byte[] Opcode { get; set; }

        public bool IsContinuationOfBlockInstructionExecution { get; private set; }
    }

    public enum InstructionExecutionOperation
    {
        BeforeInstructionExecution,
        AfterInstructionExecution
    }

    public class MemoryAccessEventArgs : ProcessorEventArgs
    {
        public MemoryAccessOperation Operation { get; private set; }

        public ushort Address { get; private set; }

        public byte Value { get; set; }
    }

    public enum MemoryAccessOperation
    {
        BeforeMemoryRead,
        AfterMemoryRead,
        BeforeMemoryWrite,
        AfterMemoryWrite,
        BeforePortRead,
        AfterPortRead,
        BeforePortWrite,
        AfterPortWrite
    }

    public class ProcessorEventArgs : EventArgs
    {
        object LocalUserState { get; set; }
    }

    public class Z80Lines
    {
        // Values can be 0 or 1 only.

        // Output

        public byte Halt { get; private set; }

        // Input

        public byte Reset { get; set; }

        public byte Interrupt { get; set; }

        public byte NonMaskableInterrupt { get; set; }
    }

    public class Z80Registers
    {
        public MainZ80Registers Main { get; set; }

        public MainZ80Registers Alternate { get; set; }

        public short IX { get; set; }

        public short IY { get; set; }

        public short PC { get; set; }

        public short SP { get; set; }

        public short IR { get; set; }

        public byte IM { get; set; }    // 0, 1 or 2

        public byte IFF1 { get; set; }  // 0 or 1

        public byte IFF2 { get; set; }  // 0 or 1
    }

    public class MainZ80Registers
    {
        public short AF { get; set; }

        public short BC { get; set; }

        public short DE { get; set; }

        public short HL { get; set; }
    }
}
