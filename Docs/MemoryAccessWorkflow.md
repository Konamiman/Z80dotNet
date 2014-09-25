## Memory access flow ##

The following chain of operations is followed whenever memory is read, either because opcodes are being fetched or as part of the processing of an instruction:

1. The `MemoryAccess` event is triggered with `MemoryAccessEventArgs.EventType` equal to `BeforeMemoryRead`.
2. **If** the configured memory access mode for the affected address is `ReadAndWrite` or `ReadOnly`, **and** the `CancelMemoryAccess` property of the event has **not** been set to true, the value is read by accessing `Memory[address]`. Otherwise, a value of FFh is assumed.
3. The `MemoryAccess` event is triggered with `MemoryAccessEventArgs.EventType` equal to `AfterMemoryRead`. The code listening the event has the opportunity to change the value.
4. The value in `MemoryAccessEventArgs.Value` is assumed to be the obtained value and is processed appropriately.

As for the operations for a memory write, they are as follows:

1. The `MemoryAccess` event is triggered with `MemoryAccessEventArgs.EventType` equal to `BeforeMemoryWrite`. The code listening the event has the opportunity to change the value to be written.
2. **If** the configured memory access mode for the affected address is `ReadAndWrite` or `WriteOnly`, **and** the `CancelMemoryAccess` property of the event has **not** been set to true, the value is written to `Memory[address]`.
3. The `MemoryAccess` event is triggered with `MemoryAccessEventArgs.EventType` equal to `AfterMemoryWrite`.

The code of the processor class takes care of the extra wait states needed for timing purposes.

The flow for accessing I/O ports is simular, but `PortsSpace` is used instead of `Memory` and the 'Port' members of `MemoryAccessEventArgs.EventType` are used instead of the 'Memory' members.

WIP...