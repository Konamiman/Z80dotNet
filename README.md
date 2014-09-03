## What is this? ##

So, one day I was doing I don't remember what, and then a weird idea came to my mind...

> What if I develop a Z80 simulator in .NET?

_"Nah"_, I replied to myself, _"you're never going to have enough free time for that"_. But somehow, the idea was stuck in my mind... and I agreed with myself to at least _start_ the project and define a minimum set of interfaces and data classes... just in case.

## How would it be supposed to work, in case it is done some day? ##

The `Z80Processor` class (which does not exist yet, just its interface has been defined) is a virtual Z80. The idea is that it should be complete enough for being capable of acting as the core class for processor simulators and computer emulators.

How to use it? You simply:

* Create an instance of the `Z80Processor` class and configure it (at the very least the memory contents).
* Subscribe to the `MemoryAccess` and `InstructionExecution` events.
* Invoke the `Start` method.
* Communicate with the processor by using the above mentioned events. When you want the `Start` method to return, you invoke the `Stop` method.

Or alternatively, instead of invoking the `Start` method, you invoke `ExecuteNextInstruction` instead.

That's it, all synchronous, all event based. Easy peasy.

## That's all?? ##

Well, that's the basic idea, but there's nothing implemented yet. Anyway you can get more details by taking a look at the source code, which is (I hope) pretty well documented.

For sure there are plenty of conceptual mistakes in what I have designed so far... if so, please send me a slap in the face! (use the link below to get my contact details)

## But who are you? ##

I'm [Konamiman, the MSX freak](http://www.konamiman.com). No more, no less.