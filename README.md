## What is this? ##

Z80.NET is a Z80 processor simulator that can be used as the core component for developing computer emulators, or to exercise pieces of Z80 code in custom test code. It is written in C# targetting the .NET Framework 4 Client Profile.  

## Hello, world! ##

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

    Debug.Assert(z80.Registers.A == 12);
    Debug.Assert(z80.TStatesElapsedSinceStart == 28);

## How to use

1. Create an instance of [the Z80Processor class](src/develop/Main/Z80Processor.cs).
2. Optionally, plug your own implementations of one or more of the [dependencies](src/develop/Docs/Dependencies.md).
3. [Configure your instance as appropriate](src/develop/Docs/Configuration.md).
4. Optionally, capture [the memory access events](src/develop/Docs/MemoryEvents.md) and/or [the instruction execution events](src/develop/Docs/InstructionEvents.md).
5. Start the simulated processor execution by using one of [the execution control methods](src/develop/Docs/ExecutionControlMethods.md).
6. Execution will stop (and the execution method invoked will then return) when one of [the execution stopping conditions is met](src/develop/Docs/StoppingConditions.md). You can then check [the processor state](src/develop/Docs/State.md) and, if desired, resume execution.   

Execution is completely synchronous: one single thread is used for everything, including firing events. As seen in the Hello World example, you just invoke a starting method and wait until it returns (there are means to force this to happen, see [the execution stopping conditions is met](src/develop/Docs/StoppingConditions.md)). If you want some kind of multithreading, you'll have to implement it by yourself, I just tried to keep things simple. :-)

Interaction of the processor with the hosting code and its outside world (memory and ports) can be achieved by handling the class events, by plugging custom implementations of the dependencies, or both. Take a look at:

* [Instruction execution workflow](src/develop/Docs/InstructionExecutionWorkflow.md)
* [Memory and ports access workflow](src/develop/Docs/MemoryAccessWorkflow.md)

## Done & to do

This is a work in progress project. So far that's what is done:

* [The Z80Processor class](src/develop/Main/Z80Processor.cs) and all of its [dependencies](src/develop/Docs/Dependencies.md).
* [The infrastructure for executing instructions](src/develop/Main/Instructions%20Execution/Core) and part of [the instructions themselves](src/develop/Main/Instructions%20Execution/Instructions).
* [A good bunch of unit tests](src/develop/Main.Tests).

...and that's what's left:

* The rest of the instructions
* The interrupts mechanism
* The code could probably benefit from some optimizations for speed

## Resources

The following resources have been used to develop this project:

* [Z80 official user manual](www.zilog.com/manage_directlink.php?filepath=docs/z80/um0080)
* [The undocumented Z80 documented](http://www.myquest.nl/z80undocumented/) by Sean Young.
* [Z80 instructions table](http://clrhome.org/table/) at [ClrHome.org](http://clrhome.org)
* [Z80 technical reference](http://www.worldofspectrum.org/faq/reference/z80reference.htm) at [WorldOfSpectrum.org](http://www.worldofspectrum.org)
* [Complete Z80 instruction set](http://www.ticalc.org/archives/files/fileinfo/195/19571.html) from [ticalc.org](http://www.ticalc.org). The [instruction tables in the code](src/develop/Main/Instructions%20Execution/Core) were automatically generated from a modified version of this file. 


## But who am I? ##

I'm [Konamiman, the MSX freak](http://www.konamiman.com). No more, no less.
