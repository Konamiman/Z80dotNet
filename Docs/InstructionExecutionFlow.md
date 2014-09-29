## Instruction execution flow ##

The following chain of operations is followed for instructions execution after the `Start` or the `Continue` method is invoked. The referred members belong to the `IZ80Processor` interface unless otherwise stated. Note that there is a separate subflow for memory access:

**This is all work in progress and highly provisional!** Sugestions for improvement are welcome.

1. The first opcode byte of the next instruction to be executed is retrieved by reading `Memory[Registers.PC]`.
2. The `InstructionExecutor.Execute` method is executed.
3. The code of the `InstructionExecutor.Execute` method executes its `ProcessorAgent.FetchNextOpcode` method, if necessary, in order to retrieve additional opcde bytes for the instruction.
4. The code of the `InstructionExecutor.Execute` method fires the `InstructionExecutor.InstructionFetchFinished` event.
5. The `BeforeInstructionExecution` event is triggered, with the fetched opcode bytes in its `BeforeInstructionExecutionEventArgs`.
6. The code of the `InstructionExecutor.Execute` method processes the instruction, using the members of its `ProcessorAgent` as appropriate to access registers, memory and ports. It can request execution termination by invoking `ProcessorAgent.Stop` (the default implementation of `IZ80InstructionExecutor` does never do this). It returns the count of T states required to execute the instruction.
7. The `AfterInstructionExecution` event is triggered. The code listening the event has an opportunity to request execution termination by invoking `AfterInstructionExecutionEventArgs.ExecutionStopper.Stop`.
8. `ClockSynchronizationHelper.TryWait` is executed, passing the value returned by `InstructionExecutor.Execute` plus any additional extra wait states as the T states count.
9. `TStatesElapsedSinceStart` and `TStatesElapsedSinceReset` are increased by the value returned by `InstructionExecutor.Execute`.
10. If `Stop` was invoked in step 6 or in step 7; or if the instruction was RET and `AutoStopOnRetWithStackEmpty` is true; or if the instruction was HALT, interrupts are disabled and `AutoStopOnDiPlusHalt` is true; the `Start` method returns.
11. Start again in step 1.

WIP...