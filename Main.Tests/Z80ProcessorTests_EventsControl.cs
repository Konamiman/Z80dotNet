using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Konamiman.Z80dotNet.Tests
{
    public class Z80ProcessorTests_EventsControl
    {
        Z80ProcessorForTests Sut { get; set; }

        [SetUp]
        public void Setup()
        {
            Sut = new Z80ProcessorForTests {
                PortsSpace = new PlainMemory(65536)
            };
        }

        #region Default values

        [Test]
        public void MemoryAreasWithReadEvents_defaults_to_all_events_enabled()
        {
            // Default is "all events enabled" represented by empty array (uses optimization flag)
            Assert.That(Sut.MemoryAreasWithReadEvents.Length, Is.EqualTo(0));
        }

        [Test]
        public void MemoryAreasWithWriteEvents_defaults_to_all_events_enabled()
        {
            Assert.That(Sut.MemoryAreasWithWriteEvents.Length, Is.EqualTo(0));
        }

        [Test]
        public void PortAreasWithReadEvents_defaults_to_all_events_enabled()
        {
            Assert.That(Sut.PortAreasWithReadEvents.Length, Is.EqualTo(0));
        }

        [Test]
        public void PortAreasWithWriteEvents_defaults_to_all_events_enabled()
        {
            Assert.That(Sut.PortAreasWithWriteEvents.Length, Is.EqualTo(0));
        }

        #endregion

        #region Input normalization

        [Test]
        public void Memory_areas_are_normalized_to_block_boundaries()
        {
            // Input addresses are masked to 0xXX00 and deduplicated
            Sut.MemoryAreasWithReadEvents = new ushort[] { 0x1234, 0x1299, 0x2000 };

            var result = Sut.MemoryAreasWithReadEvents;
            Assert.That(result.Length, Is.EqualTo(2));
            Assert.That(result, Does.Contain((ushort)0x1200));
            Assert.That(result, Does.Contain((ushort)0x2000));
        }

        [Test]
        public void Memory_areas_are_sorted()
        {
            Sut.MemoryAreasWithReadEvents = new ushort[] { 0x3000, 0x1000, 0x2000 };

            var result = Sut.MemoryAreasWithReadEvents;
            Assert.That(result[0], Is.EqualTo(0x1000));
            Assert.That(result[1], Is.EqualTo(0x2000));
            Assert.That(result[2], Is.EqualTo(0x3000));
        }

        [Test]
        public void Port_areas_in_standard_mode_use_individual_ports()
        {
            // In standard 256-port mode, each port is its own "block"
            Sut.PortAreasWithReadEvents = new ushort[] { 0x10, 0x20, 0x10 }; // duplicate should be removed

            var result = Sut.PortAreasWithReadEvents;
            Assert.That(result.Length, Is.EqualTo(2));
            Assert.That(result, Does.Contain((ushort)0x10));
            Assert.That(result, Does.Contain((ushort)0x20));
        }

        [Test]
        public void Port_areas_in_extended_mode_use_block_boundaries()
        {
            Sut.UseExtendedPortsSpace = true;
            Sut.PortAreasWithReadEvents = new ushort[] { 0x1234, 0x1299, 0x2000 };

            var result = Sut.PortAreasWithReadEvents;
            Assert.That(result.Length, Is.EqualTo(2));
            Assert.That(result, Does.Contain((ushort)0x1200));
            Assert.That(result, Does.Contain((ushort)0x2000));
        }

        #endregion

        #region Memory read events control

        [Test]
        public void Memory_read_events_fire_for_addresses_in_defined_block()
        {
            // Block 0x1000-0x10FF
            Sut.MemoryAreasWithReadEvents = new ushort[] { 0x1000 };

            var eventAddresses = new List<ushort>();
            Sut.MemoryAccess += (sender, e) => {
                if (e.EventType is MemoryAccessEventType.BeforeMemoryRead or MemoryAccessEventType.AfterMemoryRead)
                    eventAddresses.Add(e.Address);
            };

            Sut.Memory.SetContents(0, new byte[] { 0x3A, 0x50, 0x10 }); // LD A,(0x1050)
            Sut.ExecuteNextInstruction();

            // Should fire events for reading from 0x1050 (in block 0x1000)
            Assert.That(eventAddresses, Does.Contain((ushort)0x1050));
        }

        [Test]
        public void Memory_read_events_do_not_fire_for_addresses_outside_defined_block()
        {
            // Block 0x1000-0x10FF only
            Sut.MemoryAreasWithReadEvents = new ushort[] { 0x1000 };

            var eventAddresses = new List<ushort>();
            Sut.MemoryAccess += (sender, e) => {
                if (e.EventType is MemoryAccessEventType.BeforeMemoryRead or MemoryAccessEventType.AfterMemoryRead)
                    eventAddresses.Add(e.Address);
            };

            Sut.Memory.SetContents(0, new byte[] { 0x3A, 0x00, 0x20 }); // LD A,(0x2000)
            Sut.ExecuteNextInstruction();

            // Should NOT fire events for reading from 0x2000 (outside defined blocks)
            Assert.That(eventAddresses, Does.Not.Contain((ushort)0x2000));
        }

        [Test]
        public void Empty_memory_read_areas_disables_all_memory_read_events()
        {
            Sut.MemoryAreasWithReadEvents = new ushort[0];

            var eventCount = 0;
            Sut.MemoryAccess += (sender, e) => {
                if (e.EventType is MemoryAccessEventType.BeforeMemoryRead or MemoryAccessEventType.AfterMemoryRead)
                    eventCount++;
            };

            Sut.Memory.SetContents(0, new byte[] { 0x3A, 0x00, 0x10 }); // LD A,(0x1000)
            Sut.ExecuteNextInstruction();

            Assert.That(eventCount, Is.EqualTo(0));
        }

        #endregion

        #region Memory write events control

        [Test]
        public void Memory_write_events_fire_for_addresses_in_defined_block()
        {
            Sut.MemoryAreasWithWriteEvents = new ushort[] { 0x1000 };

            var eventAddresses = new List<ushort>();
            Sut.MemoryAccess += (sender, e) => {
                if (e.EventType is MemoryAccessEventType.BeforeMemoryWrite or MemoryAccessEventType.AfterMemoryWrite)
                    eventAddresses.Add(e.Address);
            };

            Sut.Registers.A = 0x42;
            Sut.Memory.SetContents(0, new byte[] { 0x32, 0x50, 0x10 }); // LD (0x1050),A
            Sut.ExecuteNextInstruction();

            Assert.That(eventAddresses, Does.Contain((ushort)0x1050));
        }

        [Test]
        public void Memory_write_events_do_not_fire_for_addresses_outside_defined_block()
        {
            Sut.MemoryAreasWithWriteEvents = new ushort[] { 0x1000 };

            var eventAddresses = new List<ushort>();
            Sut.MemoryAccess += (sender, e) => {
                if (e.EventType is MemoryAccessEventType.BeforeMemoryWrite or MemoryAccessEventType.AfterMemoryWrite)
                    eventAddresses.Add(e.Address);
            };

            Sut.Registers.A = 0x42;
            Sut.Memory.SetContents(0, new byte[] { 0x32, 0x00, 0x20 }); // LD (0x2000),A
            Sut.ExecuteNextInstruction();

            Assert.That(eventAddresses, Does.Not.Contain((ushort)0x2000));
        }

        [Test]
        public void Empty_memory_write_areas_disables_all_memory_write_events()
        {
            Sut.MemoryAreasWithWriteEvents = new ushort[0];

            var eventCount = 0;
            Sut.MemoryAccess += (sender, e) => {
                if (e.EventType is MemoryAccessEventType.BeforeMemoryWrite or MemoryAccessEventType.AfterMemoryWrite)
                    eventCount++;
            };

            Sut.Registers.A = 0x42;
            Sut.Memory.SetContents(0, new byte[] { 0x32, 0x00, 0x10 }); // LD (0x1000),A
            Sut.ExecuteNextInstruction();

            Assert.That(eventCount, Is.EqualTo(0));
        }

        #endregion

        #region Port read events control

        [Test]
        public void Port_read_events_fire_for_ports_in_defined_list()
        {
            Sut.PortAreasWithReadEvents = new ushort[] { 0x10, 0x20 };

            var eventPorts = new List<ushort>();
            Sut.MemoryAccess += (sender, e) => {
                if (e.EventType is MemoryAccessEventType.BeforePortRead or MemoryAccessEventType.AfterPortRead)
                    eventPorts.Add(e.Address);
            };

            Sut.Memory.SetContents(0, new byte[] { 0xDB, 0x10 }); // IN A,(0x10)
            Sut.ExecuteNextInstruction();

            Assert.That(eventPorts, Does.Contain((ushort)0x10));
        }

        [Test]
        public void Port_read_events_do_not_fire_for_ports_outside_defined_list()
        {
            Sut.PortAreasWithReadEvents = new ushort[] { 0x10, 0x20 };

            var eventPorts = new List<ushort>();
            Sut.MemoryAccess += (sender, e) => {
                if (e.EventType is MemoryAccessEventType.BeforePortRead or MemoryAccessEventType.AfterPortRead)
                    eventPorts.Add(e.Address);
            };

            Sut.Memory.SetContents(0, new byte[] { 0xDB, 0x30 }); // IN A,(0x30)
            Sut.ExecuteNextInstruction();

            Assert.That(eventPorts, Does.Not.Contain((ushort)0x30));
        }

        [Test]
        public void Empty_port_read_areas_disables_all_port_read_events()
        {
            Sut.PortAreasWithReadEvents = new ushort[0];

            var eventCount = 0;
            Sut.MemoryAccess += (sender, e) => {
                if (e.EventType is MemoryAccessEventType.BeforePortRead or MemoryAccessEventType.AfterPortRead)
                    eventCount++;
            };

            Sut.Memory.SetContents(0, new byte[] { 0xDB, 0x10 }); // IN A,(0x10)
            Sut.ExecuteNextInstruction();

            Assert.That(eventCount, Is.EqualTo(0));
        }

        #endregion

        #region Port write events control

        [Test]
        public void Port_write_events_fire_for_ports_in_defined_list()
        {
            Sut.PortAreasWithWriteEvents = new ushort[] { 0x10, 0x20 };

            var eventPorts = new List<ushort>();
            Sut.MemoryAccess += (sender, e) => {
                if (e.EventType is MemoryAccessEventType.BeforePortWrite or MemoryAccessEventType.AfterPortWrite)
                    eventPorts.Add(e.Address);
            };

            Sut.Registers.A = 0x42;
            Sut.Memory.SetContents(0, new byte[] { 0xD3, 0x10 }); // OUT (0x10),A
            Sut.ExecuteNextInstruction();

            Assert.That(eventPorts, Does.Contain((ushort)0x10));
        }

        [Test]
        public void Port_write_events_do_not_fire_for_ports_outside_defined_list()
        {
            Sut.PortAreasWithWriteEvents = new ushort[] { 0x10, 0x20 };

            var eventPorts = new List<ushort>();
            Sut.MemoryAccess += (sender, e) => {
                if (e.EventType is MemoryAccessEventType.BeforePortWrite or MemoryAccessEventType.AfterPortWrite)
                    eventPorts.Add(e.Address);
            };

            Sut.Registers.A = 0x42;
            Sut.Memory.SetContents(0, new byte[] { 0xD3, 0x30 }); // OUT (0x30),A
            Sut.ExecuteNextInstruction();

            Assert.That(eventPorts, Does.Not.Contain((ushort)0x30));
        }

        [Test]
        public void Empty_port_write_areas_disables_all_port_write_events()
        {
            Sut.PortAreasWithWriteEvents = new ushort[0];

            var eventCount = 0;
            Sut.MemoryAccess += (sender, e) => {
                if (e.EventType is MemoryAccessEventType.BeforePortWrite or MemoryAccessEventType.AfterPortWrite)
                    eventCount++;
            };

            Sut.Registers.A = 0x42;
            Sut.Memory.SetContents(0, new byte[] { 0xD3, 0x10 }); // OUT (0x10),A
            Sut.ExecuteNextInstruction();

            Assert.That(eventCount, Is.EqualTo(0));
        }

        #endregion

        #region Extended port space

        [Test]
        public void Port_events_in_extended_mode_use_block_based_lookup()
        {
            Sut.UseExtendedPortsSpace = true;
            // Block 0x1000-0x10FF
            Sut.PortAreasWithReadEvents = new ushort[] { 0x1000 };

            var eventPorts = new List<ushort>();
            Sut.MemoryAccess += (sender, e) => {
                if (e.EventType is MemoryAccessEventType.BeforePortRead or MemoryAccessEventType.AfterPortRead)
                    eventPorts.Add(e.Address);
            };

            // IN A,(C) with BC = 0x1050
            Sut.Registers.BC = 0x1050;
            Sut.Memory.SetContents(0, new byte[] { 0xED, 0x78 }); // IN A,(C)
            Sut.ExecuteNextInstruction();

            Assert.That(eventPorts, Does.Contain((ushort)0x1050));
        }

        [Test]
        public void Port_events_reset_when_port_mode_changes()
        {
            // Set custom port areas in standard mode
            Sut.PortAreasWithReadEvents = new ushort[] { 0x10 };

            // Change to extended mode - should reset to all enabled
            Sut.UseExtendedPortsSpace = true;

            // Verify reset to empty array (all events enabled)
            Assert.That(Sut.PortAreasWithReadEvents.Length, Is.EqualTo(0));
            Assert.That(Sut.PortAreasWithWriteEvents.Length, Is.EqualTo(0));
        }

        #endregion

        #region Multiple blocks

        [Test]
        public void Events_fire_for_any_of_multiple_defined_blocks()
        {
            Sut.MemoryAreasWithReadEvents = new ushort[] { 0x1000, 0x3000, 0x5000 };

            var eventAddresses = new List<ushort>();
            Sut.MemoryAccess += (sender, e) => {
                if (e.EventType is MemoryAccessEventType.BeforeMemoryRead or MemoryAccessEventType.AfterMemoryRead)
                    eventAddresses.Add(e.Address);
            };

            // Read from block 0x3000
            Sut.Memory.SetContents(0, new byte[] { 0x3A, 0x50, 0x30 }); // LD A,(0x3050)
            Sut.ExecuteNextInstruction();

            Assert.That(eventAddresses, Does.Contain((ushort)0x3050));
        }

        #endregion

        #region All 256 blocks coverage

        [Test]
        public void Setting_all_256_blocks_enables_all_events()
        {
            // Create array with all 256 block addresses
            var allBlocks = Enumerable.Range(0, 256).Select(i => (ushort)(i << 8)).ToArray();
            Sut.MemoryAreasWithReadEvents = allBlocks;

            var eventCount = 0;
            Sut.MemoryAccess += (sender, e) => {
                if (e.EventType is MemoryAccessEventType.BeforeMemoryRead or MemoryAccessEventType.AfterMemoryRead)
                    eventCount++;
            };

            // Read from various addresses
            Sut.Memory.SetContents(0, new byte[] { 0x3A, 0x00, 0x10 }); // LD A,(0x1000)
            Sut.ExecuteNextInstruction();

            Assert.That(eventCount, Is.GreaterThan(0));
        }

        #endregion

        #region Performance comparison

        [Test]
        [Explicit("Manual performance test")]
        public void Performance_comparison_with_and_without_events()
        {
            // This test compares emulation performance when:
            // 1. Events are enabled for the code area (with an event handler attached)
            // 2. Events are disabled for the code area
            //
            // The test runs a nested loop (HL outer, BC inner) once and measures elapsed time.
            // This avoids the overhead of repeatedly starting/stopping execution.

            const ushort outerIterations = 34;
            const ushort innerIterations = (ushort)65535;

            // Program: nested loop at address 0x1000
            // Outer loop controlled by HL, inner loop controlled by BC
            //
            // 0x1000: LD HL, nnnn    (21 lo hi)  ; outer count
            // 0x1003: LD BC, nnnn    (01 lo hi)  ; inner count
            // 0x1006: DEC BC         (0B)
            // 0x1007: LD A, B        (78)
            // 0x1008: OR C           (B1)
            // 0x1009: JR NZ, -5      (20 FB)     ; jump to 0x1006
            // 0x100B: DEC HL         (2B)
            // 0x100C: LD A, H        (7C)
            // 0x100D: OR L           (B5)
            // 0x100E: JR NZ, -13     (20 F3)     ; jump to 0x1003
            // 0x1010: RET            (C9)
            var program = new byte[] {
                0x21, 0x00, 0x00,       // LD HL, nnnn (placeholder)
                0x01, 0x00, 0x00,       // LD BC, nnnn (placeholder)
                0x0B,                   // DEC BC
                0x78,                   // LD A, B
                0xB1,                   // OR C
                0x20, 0xFB,             // JR NZ, -5
                0x2B,                   // DEC HL
                0x7C,                   // LD A, H
                0xB5,                   // OR L
                0x20, 0xF3,             // JR NZ, -13
                0xC9                    // RET
            };
            Sut.Memory.SetContents(0x1000, program);

            // Attach an event handler (simulating real-world usage)
            int eventCount = 0;
            Sut.MemoryAccess += (sender, e) => { eventCount++; };

            // Warm-up run
            //RunNestedLoop(3, 3);

            // Test 1: Events ENABLED for code area (all blocks)
            // Default is all events enabled
            eventCount = 0;

            var swWithEvents = Stopwatch.StartNew();
            RunNestedLoop(outerIterations, innerIterations);
            swWithEvents.Stop();
            var eventsEnabledCount = eventCount;

            // Test 2: Events DISABLED for code area (block 0x2000 only, not 0x1000)
            Sut.MemoryAreasWithReadEvents = [0x2000];
            eventCount = 0;

            var swWithoutEvents = Stopwatch.StartNew();
            RunNestedLoop(outerIterations, innerIterations);
            swWithoutEvents.Stop();
            var eventsDisabledCount = eventCount;

            // Output results
            var speedup = (double)swWithEvents.ElapsedTicks / swWithoutEvents.ElapsedTicks;
            long totalIterations = (long)outerIterations * innerIterations;

            Console.WriteLine($"=== Performance Comparison: Memory Access Events ===");
            Console.WriteLine($"Nested loop: {outerIterations} x {innerIterations} = {totalIterations:N0} total iterations");
            Console.WriteLine();
            Console.WriteLine($"With events enabled:");
            Console.WriteLine($"  Time: {swWithEvents.ElapsedMilliseconds} ms");
            Console.WriteLine($"  Events fired: {eventsEnabledCount:N0}");
            Console.WriteLine();
            Console.WriteLine($"With events disabled:");
            Console.WriteLine($"  Time: {swWithoutEvents.ElapsedMilliseconds} ms");
            Console.WriteLine($"  Events fired: {eventsDisabledCount:N0}");
            Console.WriteLine();
            Console.WriteLine($"Speedup: {speedup:F2}x faster with events disabled");

            // Assert that disabling events is faster
            Assert.That(swWithoutEvents.ElapsedTicks, Is.LessThan(swWithEvents.ElapsedTicks),
                "Emulation with events disabled should be faster");

            // Assert that no events fired when disabled for the code area
            Assert.That(eventsDisabledCount, Is.EqualTo(0),
                "No events should fire when code area is not in event range");
        }

        private void RunNestedLoop(ushort outerCount, ushort innerCount)
        {
            Sut.Reset();
            Sut.Registers.PC = 0x1000;
            Sut.AutoStopOnRetWithStackEmpty = true;

            // Set outer loop count (HL) at 0x1001-0x1002 (little endian)
            Sut.Memory[0x1001] = (byte)(outerCount & 0xFF);
            Sut.Memory[0x1002] = (byte)(outerCount >> 8);

            // Set inner loop count (BC) at 0x1004-0x1005 (little endian)
            Sut.Memory[0x1004] = (byte)(innerCount & 0xFF);
            Sut.Memory[0x1005] = (byte)(innerCount >> 8);

            Sut.Continue();
        }

        #endregion
    }
}
