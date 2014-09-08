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

## But who are you? ##

I'm [Konamiman, the MSX freak](http://www.konamiman.com). No more, no less.