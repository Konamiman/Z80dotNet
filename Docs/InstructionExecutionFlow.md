## Instruction execution flow ##

The following is the chain of operations performed by [IZ80Processor](../Main/IZ80Processor.cs) when executing one instruction, either as part of the instructions execution loop or when executing one single instruction (see [how execution works](HowExecutionWorks.md)).

1. The first opcode byte of the instruction to be executed is retrieved by reading `Memory[Registers.PC]`.
2. The `InstructionExecutor.Execute` method is invoked.

The code for `InstructionExecutor.Execute` does then the following:

3. The `ProcessorAgent.FetchNextOpcode` method is invoked, if necessary, in order to retrieve additional opcde bytes for the instruction.
4. The `InstructionExecutor.InstructionFetchFinished` event is fired. _This causes `IZ80Processor` to fire the `BeforeInstructionExecution` event_.
6. The instruction is processed, using the members of `ProcessorAgent` as appropriate to access registers, memory and ports. _Execution termination can be requested at this point by invoking `ProcessorAgent.Stop` (the default implementation of `IZ80InstructionExecutor` does never do this)_. 
7. The method terminates, returning the count of T states required to execute the instruction.

Control returns then to `IZ80Processor`:

8. The `AfterInstructionExecution` event is triggered. _The code that handles the event has an opportunity to request execution termination by invoking `AfterInstructionExecutionEventArgs.ExecutionStopper.Stop`_.
8. `IClockSynchronizer.TryWait` is executed, having the value returned by `InstructionExecutor.Execute` plus any additional extra wait states passed as the T states count.
9. `TStatesElapsedSinceStart` and `TStatesElapsedSinceReset` are increased by the same value calculated in the previous step.
10. If one of [the execution stop conditions](StopConditions.md) is met, [the method that initiated the execution](HowExecutionWorks.md) returns.
11. If inside an instruction execution loop, the flow starts again for the next instruction.

Note that [BeforeInstructionExecutionEventArgs](../Main/EventArgs/BeforeInstructionExecutionEventArgs.cs) and   [AfterInstructionExecutionEventArgs](../Main/EventArgs/AfterInstructionExecutionEventArgs.cs) inherit from [ProcessorEventArgs](../Main/EventArgs/ProcessorEventArgs.cs), which defines the `LocalUserState` property. This property is propagated from the 'before' event to the 'after' event and can be used by the events handling code at its convenience.