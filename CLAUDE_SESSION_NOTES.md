# Z80dotNet - Deactivable Memory Access Events Implementation

## Session Date: 2025-12-21

## Branch: `deactivable-memory-access-events`

## Overview

Implemented selective firing of `MemoryAccess` events based on configurable address ranges. This allows users to improve emulation performance by only receiving events for specific memory/port regions (e.g., video RAM, memory-mapped I/O).

## Design Decisions

### Public API
- **Separate read/write control**: Four properties for independent control:
  - `MemoryAreasWithReadEvents` - controls BeforeMemoryRead/AfterMemoryRead
  - `MemoryAreasWithWriteEvents` - controls BeforeMemoryWrite/AfterMemoryWrite
  - `PortAreasWithReadEvents` - controls BeforePortRead/AfterPortRead
  - `PortAreasWithWriteEvents` - controls BeforePortWrite/AfterPortWrite

- **Named tuples for ranges**: `(ushort StartAddress, ushort EndAddress)[]`

- **New interface**: `IZ80ProcessorEventsControl` (avoids breaking clients extending `IZ80Processor`)

### Implementation
- **Lookup table approach**: `bool[]` arrays for O(1) address lookup (64KB for memory, 256B or 64KB for ports)

- **Optimization flags**: When ranges cover entire space or are empty, no table is allocated:
  - `allFlag = true` → fire all events
  - `allFlag = false` → fire no events
  - `allFlag = null` → use lookup table

- **Backwards compatibility**:
  - Default ranges cover full address space (0x0000-0xFFFF for memory, 0x00-0xFF for ports)
  - When `UseExtendedPortsSpace` changes to true with default "all events" setting, preserves that behavior

## Files Created

### `Main/IZ80ProcessorEventsControl.cs`
New interface with the four event area properties. Full XML documentation included.

### `Main.Tests/Z80ProcessorTests_EventsControl.cs`
19 unit tests covering:
- Default values
- Memory read/write event filtering
- Port read/write event filtering
- Multiple ranges
- Extended ports space behavior
- Read/write independence
- Memory operations still work when events are disabled

## Files Modified

### `Main/Z80Processor.cs`
- Added `IZ80ProcessorEventsControl` to class declaration (line 11)
- Added backing fields, lookup tables, and optimization flags (lines 26-47)
- Added initialization in constructor (lines 74-78)
- Updated `UseExtendedPortsSpace` setter to preserve "all events" flag when extending (lines 773-779)
- Added new region "Events control" with:
  - Property implementations
  - `RebuildMemoryLookupTable()` helper
  - `ShouldFire*Event()` helpers
  - `ShouldFireEventForAccessType()` routing method
- Modified `ReadFromMemoryOrPort()` to check if events should fire (lines 1080-1088)
- Modified `WritetoMemoryOrPort()` to check if events should fire (lines 1152-1159)

## Key Code Locations

| Feature | File | Line(s) |
|---------|------|---------|
| Interface definition | `IZ80ProcessorEventsControl.cs` | 1-90 |
| Backing fields & tables | `Z80Processor.cs` | 26-47 |
| Property implementations | `Z80Processor.cs` | 787-841 |
| Lookup table rebuild | `Z80Processor.cs` | 843-887 |
| Event firing check (read) | `Z80Processor.cs` | 1080-1088 |
| Event firing check (write) | `Z80Processor.cs` | 1152-1159 |
| Extended ports handling | `Z80Processor.cs` | 773-779 |

## Test Results

- All 19 new tests pass
- No regressions in existing tests (52 pre-existing failures unrelated to this feature)
- The `Test_memory_event_with_extended_port_access` test that was affected by the changes now passes

## Potential Future Enhancements

1. **All-false optimization check**: Currently we check if all entries are true to optimize. Could also check if all are false (though empty ranges already handle this case).

2. **Range validation**: Could add validation to reject invalid ranges (e.g., StartAddress > EndAddress).

3. **Convenience methods**: Could add helper methods like `DisableAllMemoryReadEvents()` or `EnableMemoryReadEventsForRange(start, end)`.

## Usage Example

```csharp
var processor = new Z80Processor();

// Only fire read events for video RAM area
processor.MemoryAreasWithReadEvents = new[] { ((ushort)0x4000, (ushort)0x5FFF) };

// Disable all write events (e.g., for ROM areas)
processor.MemoryAreasWithWriteEvents = Array.Empty<(ushort, ushort)>();

// Multiple ranges for port events
processor.PortAreasWithReadEvents = new[] {
    ((ushort)0x10, (ushort)0x1F),  // Keyboard ports
    ((ushort)0xFE, (ushort)0xFE)   // ULA port
};
```

## Commands to Verify

```bash
# Build
dotnet build Main/Main.csproj

# Run new tests
dotnet test Main.Tests/Main.Tests.csproj --filter "Z80ProcessorTests_EventsControl"

# Run all tests (52 pre-existing failures expected)
dotnet test Main.Tests/Main.Tests.csproj
```
