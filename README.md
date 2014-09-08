## What is this? ##

So, one day I was doing I don't remember what, and then a weird idea came to my mind...

> What if I develop a Z80 simulator in .NET?

_"Nah"_, I replied to myself, _"you're never going to have enough free time for that"_. But somehow, the idea was stuck in my mind... so I agreed with myself to at least _start_ the project and define a minimum set of interfaces and data classes... just in case.

## How would it be supposed to work, in case it is done some day? ##

The `Z80Processor` class (which does not exist yet, just its interface has been defined) is a virtual Z80 processor. The idea is that it should be complete enough for being capable of acting as the core class for processor simulators and computer emulators.

How to use it? You simply:

* Create an instance of the `Z80Processor` class and configure it (at the very least the memory contents).
* Optionally, plug your own implementation of `IMemory` in the `Memory` property of the class (the default memory implementation is a plain array of bytes).
* Subscribe to the `MemoryAccess` and `InstructionExecution` events.
* Invoke the `Start` method.
* Communicate with the processor by using the above mentioned events. When you want the `Start` method to return, you invoke the `Stop` method.

Or alternatively, instead of invoking the `Start` method, you invoke `ExecuteNextInstruction` instead (useful for step-by-step debuggers).

That's it, all synchronous, all event based. Easy peasy.

## That's all?? ##

Well, that's the basic idea, but there's nothing implemented yet. Anyway you can get more details by taking a look at the source code, which is (I hope) pretty well documented.

For sure there are plenty of conceptual mistakes in what I have designed so far... if so, please send me a slap in the face! (use the link below to get my contact details)

## Hello, world! (not runnable yet) ##

    var z80 = new Z80Processor();
    z80.AutoStopOnRetWithStackEmpty = true;

    var program = new byte[] {
        0x3E, 0x07, //LD A,7
        0xC6, 0x04, //ADD A,4
        0x3C,       //INC A
        0xC9        //RET
    };
    z80.Memory.SetContents(0, program);

    z80.Start();

    Debug.Assert(z80.Registers.Main.A == 12);

## Instruction execution flow ##

The following chain of operations is followed for instructions execution after the `Start` or the `Continue` method is invoked. The referred members belong to the `IZ80Processor` interface unless otherwise stated. Note that there is a separate subflow for memory access:

**This is all work in progress and highly provisional!** Sugestions for improvement are welcome.

1. `PeriodWaiter.StartPeriod` is invoked.
2. The first opcode byte of the next instruction to be executed is retrieved by reading `Memory[Registers.PC]`.
3. The `InstructionExecutor.Execute` method is executed.
4. The code of the `InstructionExecutor.Execute` method executes its `ProcessorAgent.FetchNextOpcode` method, if necessary, in order to retrieve additional opcde bytes for the instruction.
5. The code of the `InstructionExecutor.Execute` method fires the `InstructionExecutor.InstructionFetchFinished` event.
6. The `BeforeInstructionExecution` event is triggered, with the fetched opcode bytes in its `BeforeInstructionExecutionEventArgs`.
7. The code of the `InstructionExecutor.Execute` method processes the instruction, using the members of its `ProcessorAgent` as appropriate to access registers, memory and ports. It can request execution termination by invoking `ProcessorAgent.Stop` (the default implementation of `IZ80InstructionExecutor` does never do this). It returns the count of T states required to execute the instruction.
8. The `AfterInstructionExecution` event is triggered. The code listening the event has an opportunity to request execution termination by invoking `AfterInstructionExecutionEventArgs.ExecutionStopper.Stop`.
9. `PeriodWaiter.WaitToEndOfPeriod` passing the value returned by `InstructionExecutor.Execute` plus any additional extra wait states as the T states count.
10. `TStatesElapsedSinceStart` and `TStatesElapsedSinceReset` are increased by the value returned by `InstructionExecutor.Execute`.
11. If `Stop` was invoked in step 7 or in step 8; or if the instruction was RET and `AutoStopOnRetWithStackEmpty` is true; or if the instruction was HALT, interrupts are disabled and `AutoStopOnDiPlusHalt` is true, the `Stop` method returns.
12. Start again in step 1.

## Memory access flow ##

TO DO...

## But who are you? ##

I'm [Konamiman, the MSX freak](http://www.konamiman.com). No more, no less.