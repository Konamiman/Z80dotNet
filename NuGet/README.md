## Z80.NET

Z80.NET is a Z80 processor simulator complete with all instructions (documented and undocumented) and support for interrupts. Memory and port access can be controlled via events or by plugging custom memory space implementations, while interrupts are handled by plugging one or more custom interrupt sources. Use Z80.NET to exercise pieces of Z80 code or to create your dream 8 bit computer emulator!

## Hello, world!

```C#
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
```

## Documentation

* [Project README file](https://github.com/Konamiman/Z80dotNet/blob/master/README.md)
* [Configuration](https://github.com/Konamiman/Z80dotNet/blob/master/Docs/Configuration.md)
* [Dependencies](https://github.com/Konamiman/Z80dotNet/blob/master/Docs/Dependencies.md)
* [How execution works](https://github.com/Konamiman/Z80dotNet/blob/master/Docs/HowExecutionWorks.md)
* [Instruction execution flow](https://github.com/Konamiman/Z80dotNet/blob/master/Docs/InstructionExecutionFlow.md)
* [Interrupts](https://github.com/Konamiman/Z80dotNet/blob/master/Docs/Interrupts.md)
* [Memory access flow](https://github.com/Konamiman/Z80dotNet/blob/master/Docs/MemoryAccessFlow.md)
* [Checking state](https://github.com/Konamiman/Z80dotNet/blob/master/Docs/State.md)
* [Execution stop conditions](https://github.com/Konamiman/Z80dotNet/blob/master/Docs/StopConditions.md)
