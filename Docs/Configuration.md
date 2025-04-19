## Configuration

The [Z80Processor class](../Main/Z80Processor.cs) can be configured by using a set of methods and properties that are in place for this purpose. The class constructor sets the configuration to  values that are useful in most cases, so the processor object is ready to be used as soon as it is instantiated; however in some cases it may be necessary to apply custom configuration. This section describes the available configuration members.

_NOTE:_ Remember that plugging [custom dependencies](Dependencies.md) is an alternative (or complementary) way to customize the behavior of the processor class.

* **`AutoStopOnDiPlusHalt`**:  When this value is true, the processor execution will stop automatically if a HALT instruction is executed when interrupts are disabled. See the [execution stop conditions](StopConditions.md) documentation for details. The default value is _true_.

* **`AutoStopOnRetWithStackEmpty `**: When this value is true, the processor execution will stop automatically if a RET instruction is executed when the stack is empty. See the [execution stop conditions](StopConditions.md) documentation for details. The default value is _false_.

* **`ClockFrequencyInMhz`** and **`ClockSpeedFactor`**: These two values, when multiplied, hold the clock speed at which the simulated processor runs; this information is only relevant for [the clock synchronizer](../Main/Dependencies%20Interfaces/IClockSynchronizer.cs). There are two properties to help in the development of computer emulators, so it is possible to fix the clock frequency to the value for the real system but allow the user to tune the speed factor as appropriate. The default value is 4 (MHz) for the clock frequency and 1 for the speed factor.

* **`SetMemoryAccessMode`** and **`GetMemoryAccessMode`** methods allow to set and get the memory access mode for a certain range of memory addresses (see the [memory access workflow](MemoryAccessFlow.md) for details). The default value is `MemoryAccessMode.ReadAndWrite` for the entire memory.

* **`SetMemoryWaitStatesForM1`** and **`GetMemoryWaitStatesForM1`** methods allow to configure the amount of extra T states that will be counted for timing purposes when the memory is read during the M1 state (when fetching the instruction opcode bytes). The default value is zero for the entire memory.

* **`SetMemoryWaitStatesForNonM1`** and **`GetMemoryWaitStatesForNonM1`** methods allow to configure the amount of extra T states that will be counted for timing purposes when the memory is read or written during the execution of an instruction but outside the M1 state. The default value is zero for the entire memory.

* **`SetPortsSpaceAccessMode`** and **`GetPortsSpaceAccessMode`** methods allow to set and get the ports space access mode for a certain range of port numbers (see the [memory access workflow](MemoryAccessFlow.md) for details). The default value is `MemoryAccessMode.ReadAndWrite` for the entire ports space.

* **`SetPortWaitStates`** and **`GetPortWaitStates`** methods allow to configure the amount of extra T states that will be counted for timing purposes when a port is read or written during the execution of an instruction (not including the extra T state that is always added by the Z80 itself). The default value is zero for the entire ports space.

* **`UserState`**: This is a convenience property that can be used by the client code to store just anything. The code of the processor class will never access this property for anything. The default value is _null_.

* **`UseExtendedPortsSpace`**: Enables or disables the extended ports space (it's disabled by default), see below for details.


### The extended ports space

In a real Z80 processor the port access instructions (`IN`, `INI`, `INIR`, `IND`, `INDR`, `OUT`, `OUTI`, `OTIR`, `OUTD`, `OTDR`) work with 16 bit port numbers. These port numbers are composed as follows:

* The lower half, whose value is set to pins A7-A0 of the address bus, is included in the instruction itself, either explicitly (as part of the instruction opcode) or implicitly (in register C).
* The higher half, whose value is set to pins A15-A8 of the address bus, is taken from register A or B (when the instruction begins its execution) as follows:
  * For the `IN A,(n)` and `OUT (n),A` instructions: register A.
  * For the other instructions: register B.

Z80.NET 1.0 didn't support 16 bit port numbers: register A/B was ignored and the actual port accessed was always a number in the range 0-255, as specified by the instruction opcode or as taken from register C.

Z80.NET 1.1 implements a new boolean property, `UseExtendedPortsSpace`, that works as follows:

* When it's `false` (default) the behavior is as in Z80.NET 1.0: port numbers are always 8 bit values.
* When it's `true`, 16 bit port numbers are supported as specified above.

For example, take the following snippet:

```
ld a,12h
in a,(34h)
```

* When `UseExtendedPortsSpace` is `false` this will load A with the value of `PortsSpace[0x34]`.
* When `UseExtendedPortsSpace` is `true` this will load A with the value of `PortsSpace[0x1234]`.

Note that before setting `UseExtendedPortsSpace` to `true` you need to set `PortsSpace` to an instance of `IMemory` that reports a size of at least 65536, otherwise you'll get an exception; example:

```C#
var Z80 = new Z80Processor();
Z80.PortsSpace = new PlainMemory(65536);
Z80.UseExtendedPortsSpace = true;
```

You may want to take a look at [the relevant unit tests](../Main.Tests/Z80ProcessorTests_PortsAccess.cs) for more details on how port access works depending on the value of `UseExtendedPortsSpace`.