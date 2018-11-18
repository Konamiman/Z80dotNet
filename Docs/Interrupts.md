## Interrupts

[IZ80Processor](../Main/IZ80Processor.cs) supports maskable and non-maskable interrupts. In order to be able to trigger interrupts, you need to create and plug _interrupt sources_, which are classes that implement [IZ80InterruptSource](../Main/Dependencies%20Interfaces/IZ80InterruptSource.cs). Interrupt sources must be registered by using the `IZ80Processor.RegisterInterruptSource` method so that the triggered interrupts are recognized by the processor instance. More than one interrupt source can be registered at the same time.

Non-maskable interrupts are triggered by the interrupt source by firing its `NmiInterruptPulse` event. This will cause the processor to handle the interrupt the standard way (maskable interrupts are disabled and a call to address 0x0066 is performed) the next time an instruction finishes its execution. Note that the `Start` and `Reset` methods cause a pending non-maskable interrupt to be lost.

Maskable interrupts are handled by the processor in the following way: after an instruction execution is finished, and if maskable interrupts are enabled, the list of registered interrupts sources is scanned for instances that have the `IntLineIsActive` property set to _true_. If at least one match is found, the interrupt is serviced the standard way (depends on the current interrupt mode). For interrupt mode 2 the `ValueOnDataBus` of the interrupt source is used as well, so it should have a meaningful value.

The `HALT` instruction behaves the expected way: after this instruction is executed the processor will enter a loop in which `NOP` instructions are executed (PC is not incremented) until an interrupt is received; note however that if `AutoSopOnDiPlusHalt` is _true_ a `HALT` instruction may cause the execution to stop (depends on the processor [configuration](Configuration.md)).

### Interrupt events

[IZ80Processor](../Main/IZ80Processor.cs) provides the following interrupt related events:

* _`NonMaskableInterruptServicingStart`_ is fired right before a maskable interrupt is going to be serviced. The execution state is as follows when the event is fired:

  * For interrupt mode 0: the opcode has been already fetched from the data bus and is about to be executed.

  * For interrupt mode 1: PC is already set to 0x0038 and the return address has been pushed to the stack.

  * For interrupt mode 2: PC is already set to the address of the routine to execute and the return address has been pushed to the stack.

* _`MaskableInterruptServicingStart`_ is fired right before a non-maskable interrupt is going to be serviced. PC is already set to 0x0066 and the return address has been pushed to the stack when this event is fired.

* _`BeforeRetiInstructionExecution`_ and _`BeforeRetnInstructionExecution`_ are fired before a RETI/RETN instruction is about to be executed, right after the corresponding _`BeforeInstructionExecution`_ event.

* _`AfterRetiInstructionExecution`_ and _`AfterRetnInstructionExecution`_ are fired after a RETI/RETN instruction is about to be executed, right after the corresponding _`AfterInstructionExecution`_ event.
