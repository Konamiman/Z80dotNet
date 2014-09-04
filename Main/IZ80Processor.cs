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
    /// <para>
    /// The default configuration when the class is instantiated is:
    /// <list type="bullet">
    /// <item><description><see cref="IZ80Processor.ClockFrequencyInMHz"/> = 4</description></item>
    /// <item><description><see cref="IZ80Processor.ClockSpeedFactor"/> = 1</description></item>
    /// <item><description><see cref="IZ80Processor.AutoStopOnDiPlusHalt"/> = false</description></item>
    /// <item><description><see cref="IZ80Processor.AutoStopOnRetWithStackEmpty"/> = false</description></item>
    /// <item><description><see cref="IZ80Processor.MemoryWaitStates"/> = all zeros</description></item>
    /// <item><description><see cref="IZ80Processor.PortWaitStates"/> = all zeros</description></item>
    /// <item><description><see cref="IZ80Processor.Memory"/> = an instance of <see cref="PlainMemory"/></description></item>
    /// <item><description><see cref="IZ80Processor.PortsSpace"/> = an instance of <see cref="PlainMemory"/></description></item>
    /// <item><description>Memory and ports access modes = all <see cref="MemoryAccessMode.ReadAndWrite"/></description></item>
    /// </list>
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
        /// processor classs will return <see cref="Konamiman.Z80dotNet.StopReason.PauseInvoked"/> after the method returns.
        /// Otherwise, it will return <see cref="Konamiman.Z80dotNet.StopReason.StopInvoked"/>.</param>
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
        /// Resets all registers to its initial state. The running state is not modified.
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
        /// <exception cref="InvalidOperationException">The method is invoked from within an event handler, or neither the
        /// <see cref="IZ80Processor.Start"/> method or the <see cref="IZ80Processor.Reset"/> method
        /// have never been invoked since the processor object was created.</exception>
        void ExecuteNextInstruction();

        /// <summary>
        /// Simulates the arrival of a non-maskable interrupt request.
        /// </summary>
        void FireNonMaskableInterrupt();

        /// <summary>
        /// Simulates the arrival of a maskable interrupt request.
        /// </summary>
        /// <param name="dataBusValue">The value that the processor will read from the data bus,
        /// relevant for the interrupt modes 0 and 2.</param>
        /// <remarks>This method will have no effect if the IIF1 flag is set to zero.</remarks>
        void FireMaskableInterrupt(byte dataBusValue = 0xFF);

        #endregion

        #region Information and state

        /// <summary>
        /// Obtains the count of T states elapsed since the processor execution started.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This property is set to zero when the processor object is created, and when the
        /// <see cref="IZ80Processor.Start"/> method is invoked. It is not affected by the
        /// <see cref="IZ80Processor.Continue"/> and <see cref="IZ80Processor.Reset"/> methods.
        /// </para>
        /// <para>The value is updated after each relevant operation (memory access or instruction execution),
        /// both in running mode and in single instruction execution mode.</para>
        /// </remarks>
        ulong TStatesElapsedSinceStart { get; }

        /// <summary>
        /// Obtains the count of T states elapsed since the last reset.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This property is set to zero when the processor object is created, and when the
        /// <see cref="IZ80Processor.Start"/> method or the <see cref="IZ80Processor.Reset"/> are invoked.
        /// It is not affected by the <see cref="IZ80Processor.Continue"/> method.
        /// </para>
        /// <para>The value is updated after each relevant operation (memory access or instruction execution),
        /// both in running mode and in single instruction execution mode.</para>
        /// </remarks>
        ulong TStatesElapsedSinceReset { get; }

        /// <summary>
        /// Obtains the reason for the processor not being in the running state,
        /// that is, what triggered the last stop.
        /// </summary>
        StopReason StopReason { get; }

        /// <summary>
        /// Obtains the current processor execution state.
        /// </summary>
        ProcessorState State { get; }

        /// <summary>
        /// Contains an user-defined state object. This property exists for the client code convenience
        /// and can be set to any value, the class code will never access it.
        /// </summary>
        object UserState { get; set; }

        /// <summary>
        /// Returns true when a HALT instruction is executed, returns to false when an interrupt request arrives.
        /// </summary>
        bool IsHalted { get; set; }

        #endregion

        #region Inside and outside world

        /// <summary>
        /// Gets or sets the register set used by the processor.
        /// </summary>
        Z80Registers Registers { get; set; }

        /// <summary>
        /// Gets or sets the visible memory for the processor.
        /// </summary>
        IMemory Memory { get; }

        /// <summary>
        /// Sets the mode of a portion of the visible memory.
        /// </summary>
        /// <param name="startAddress">First memory address that will be set</param>
        /// <param name="length">Length of the memory portion that will be set</param>
        /// <param name="mode">New memory mode</param>
        /// <exception cref="System.InvalidOperationException"><c>startAddress</c> + <c>length</c> goes beyond 65535.</exception>
        void SetMemoryAccessMode(ushort startAddress, ushort length, MemoryAccessMode mode);

        /// <summary>
        /// Gets or sets the visible ports space for the processor.
        /// </summary>
        IMemory PortsSpace { get; }

        /// <summary>
        /// Sets the mode of a portion of the visible ports space.
        /// </summary>
        /// <param name="startAddress">First port that will be set</param>
        /// <param name="length">Length of the mports space that will be set</param>
        /// <param name="mode">New memory mode</param>
        /// <exception cref="System.InvalidOperationException"><c>startAddress</c> + <c>length</c> goes beyond 255.</exception>
        void SetPortsSpaceMode(ushort startPort, ushort length, MemoryAccessMode mode);

        #endregion

        #region Configuration

        /// <summary>
        /// Gets or sets the clock frequency in MegaHertzs. 
        /// This value cannot be changed while the processor is running or in single instruction execution mode.
        /// </summary>
        /// <exception cref="System.ArgumentException">The product of <see cref="IZ80Processor.ClockSpeedFactor"/>
        /// by the new value gives a number that is smaller than 0.001 or greater than 1000.</exception>
        /// <exception cref="System.InvalidOperationException">The procesor is running or in single instruction execution mode.</exception>
        decimal ClockFrequencyInMHz { get; set; }

        /// <summary>
        /// Gets or sets a value that is multiplied by the clock frequency to obtain the effective
        /// clock frequency simulated by the processor.
        /// </summary>
        /// <exception cref="System.ArgumentException">The product of <see cref="IZ80Processor.ClockFrequencyInMHz"/>
        /// by the new value gives a number that is smaller than 0.001 or greater than 1000.</exception>
        decimal ClockSpeedFactor { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the processor should stop automatically or not when a HALT
        /// instruction is executed with interrupts disabled.
        /// </summary>
        bool AutoStopOnDiPlusHalt { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the processor should stop automatically when a RET
        /// instruction is executed and the stack is empty.
        /// </summary>
        /// <remarks>
        /// <para>"The stack is empty" means that the SP register has the value
        /// it had when the <see cref="IZ80Processor.Start"/> method or the <see cref="IZ80Processor.Stop"/>
        /// method was executed, or the value set by the last execution of a <c>LD, SP</c> instruction.
        /// </para>
        /// <para>
        /// This setting is useful for testing simple programs, so that the processor stops automatically
        /// as soon as the program finishes with a RET.
        /// </para>
        /// </remarks>
        bool AutoStopOnRetWithStackEmpty { get; set; }

        /// <summary>
        /// Gets the 64K array that indicates how many wait states will be simulated when accessing each visible memory address.
        /// </summary>
        byte[] MemoryWaitStates { get; set; }

        /// <summary>
        /// Sets the wait states that will be simulated when accessing of a portion of the visible memory.
        /// </summary>
        /// <param name="startAddress">First memory address that will be set</param>
        /// <param name="length">Length of the memory portion that will be set</param>
        /// <param name="waitStates">New wait states</param>
        /// <remarks>
        /// This is just a convenience method. The wait states can be changed too by just accessing the array
        /// returned by the <see cref="IZ80Processor.MemoryWaitStates"/> property.
        /// </remarks>
        /// <exception cref="System.InvalidOperationException"><c>startAddress</c> + <c>length</c> goes beyond 65535.</exception>
        void SetMemoryWaitStates(ushort startAddress, ushort length, ushort waitStates);

        /// <summary>
        /// Gets the 256 bytes array that indicates how many wait states will be simulated when accessing each port.
        /// </summary>
        ushort[] PortWaitStates { get; set; }

        /// <summary>
        /// Sets the wait states that will be simulated when accessing a given range of ports.
        /// </summary>
        /// <param name="startPort">First port that will be set</param>
        /// <param name="length">Length of the port range that will be set</param>
        /// <param name="waitStates">New wait states</param>
        /// <remarks>
        /// This is just a convenience method. The wait states can be changed too by just accessing the array
        /// returned by the <see cref="IZ80Processor.PortWaitStates"/> property.
        /// </remarks>
        /// <exception cref="System.InvalidOperationException"><c>startAddress</c> + <c>length</c> goes beyond 255.</exception>
        void SetPortWaitStates(ushort startPort, ushort length, ushort waitStates);

        #endregion

        #region Events

        /// <summary>
        /// Memory access event. Is is triggered before and after each memory and port read and write.
        /// </summary>
        event EventHandler<MemoryAccessEventArgs> MemoryAccess;

        /// <summary>
        /// Instruction execution event. It is triggered before and after an instruction is executed.
        /// </summary>
        event EventHandler<InstructionExecutionEventArgs> InstructionExecution;

        #endregion
    }
}
