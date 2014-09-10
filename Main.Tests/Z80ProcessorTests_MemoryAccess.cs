using System;
using System.Net.Sockets;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Ploeh.AutoFixture;

namespace Konamiman.Z80dotNet.Tests
{
    public class Z80ProcessorTests_MemoryAccess
    {
        Z80Processor Sut { get; set; }
        Fixture Fixture { get; set; }
        Mock<IMemory> Memory { get; set; }
        Mock<IMemory> Ports { get; set; }

        [SetUp]
        public void Setup()
        {
            Fixture = new Fixture();

            Sut = new Z80Processor();
            Memory = new Mock<IMemory>();
            Sut.Memory = Memory.Object;
            Ports = new Mock<IMemory>();
            Sut.PortsSpace = Ports.Object;
        }

        [Test]
        public void Can_create_instances()
        {
            Assert.IsNotNull(Sut);
        }

        [Test]
        [TestCase(MemoryAccessMode.ReadAndWrite, false)]
        [TestCase(MemoryAccessMode.ReadOnly, false)]
        [TestCase(MemoryAccessMode.ReadAndWrite, true)]
        [TestCase(MemoryAccessMode.ReadOnly, true)]
        public void ReadFromMemory_accesses_memory_if_memory_mode_is_ReadAndWrite_or_ReadOnly(MemoryAccessMode accessMode, bool isPort)
        {
            var address = Fixture.Create<byte>();
            var value = Fixture.Create<byte>();
            var memory = isPort ? Ports : Memory;

            memory.Setup(m => m[address]).Returns(value);
            SetAccessMode(address, accessMode, isPort);

            var actual = Read(address, isPort);

            Assert.AreEqual(value, actual);
            memory.Verify(m => m[address]);
        }

        private void SetAccessMode(byte address, MemoryAccessMode accessMode, bool isPort)
        {
            if(isPort)
                Sut.SetPortsSpaceAccessMode(address, 1, accessMode);
            else
                Sut.SetMemoryAccessMode(address, 1, accessMode);
        }

        [Test]
        [TestCase(MemoryAccessMode.NotConnected, false)]
        [TestCase(MemoryAccessMode.WriteOnly, false)]       
        [TestCase(MemoryAccessMode.NotConnected, true)]
        [TestCase(MemoryAccessMode.WriteOnly, true)]
        public void ReadFromMemory_does_not_access_memory_if_memory_mode_is_NotConnected_or_WriteOnly(MemoryAccessMode accessMode, bool isPort)
        {
            var address = Fixture.Create<byte>();
            var value = Fixture.Create<byte>();
            var memory = isPort ? Ports : Memory;

            memory.Setup(m => m[address]).Throws<Exception>();
            SetAccessMode(address, accessMode, isPort);

            Read(address, isPort);
        }

        byte Read(byte address, bool isPort)
        {
            if(isPort)
                return Sut.ReadFromPort(address);
            else
                return Sut.ReadFromMemory(address);
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void ReadFromMemory_fires_Before_event_with_appropriate_address_and_value(bool isPort)
        {
            var address = Fixture.Create<byte>();

            var eventFired = false;

            Sut.MemoryAccess += (sender, args) =>
            {
                if(args.EventType == BeforeReadEventType(isPort))
                {
                    eventFired = true;
                    Assert.AreEqual(Sut, sender);
                    Assert.AreEqual(address, args.Address);
                    Assert.AreEqual(0xFF, args.Value);
                }
            };

            Read(address, isPort);

            Assert.IsTrue(eventFired);
        }

        MemoryAccessEventType BeforeReadEventType(bool isPort)
        {
            if(isPort)
                return MemoryAccessEventType.BeforePortRead;
            else
                return MemoryAccessEventType.BeforeMemoryRead;
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void ReadFromMemory_fires_After_event_with_appropriate_address_and_value_if_memory_is_accessed(bool isPort)
        {
            var address = Fixture.Create<byte>();
            var value = Fixture.Create<byte>();
            var memory = isPort ? Ports : Memory;

            var eventFired = false;

            memory.Setup(m => m[address]).Returns(value);

            Sut.MemoryAccess += (sender, args) =>
            {
                if(args.EventType == AfterReadEventType(isPort))
                {
                    eventFired = true;
                    Assert.AreEqual(Sut, sender);
                    Assert.AreEqual(address, args.Address);
                    Assert.AreEqual(value, args.Value);
                }
            };

            var actual = Read(address, isPort);

            Assert.IsTrue(eventFired);
            Assert.AreEqual(value, actual);
        }

        MemoryAccessEventType AfterReadEventType(bool isPort)
        {
            if(isPort)
                return MemoryAccessEventType.AfterPortRead;
            else
                return MemoryAccessEventType.AfterMemoryRead;
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void ReadFromMemory_fires_After_event_with_same_value_as_Before_event_if_memory_is_not_accessed(bool isPort)
        {
            var address = Fixture.Create<byte>();
            var value = Fixture.Create<byte>();
            var memory = isPort ? Ports : Memory;

            var beforeEventFired = false;
            var afterEventFired = false;

            memory.Setup(m => m[address]).Throws<Exception>();
            SetAccessMode(address, MemoryAccessMode.NotConnected, isPort);

            Sut.MemoryAccess += (sender, args) =>
            {
                if(args.EventType == BeforeReadEventType(isPort))
                {
                    beforeEventFired = true;
                    Assert.AreEqual(address, args.Address);
                    args.Value = value;
                }
                if(args.EventType == AfterReadEventType(isPort))
                {
                    afterEventFired = true;
                    Assert.AreEqual(address, args.Address);
                    Assert.AreEqual(value, args.Value);
                }
            };

            var actual = Read(address, isPort);

            Assert.IsTrue(beforeEventFired);
            Assert.IsTrue(afterEventFired);
        }
        
        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void ReadFromMemory_returns_value_set_in_After_event(bool isPort)
        {
            var address = Fixture.Create<byte>();
            var valueFromMemory = Fixture.Create<byte>();
            var valueFromEvent = Fixture.Create<byte>();
            var memory = isPort ? Ports : Memory;

            var eventFired = false;

            memory.Setup(m => m[address]).Returns(valueFromMemory);

            Sut.MemoryAccess += (sender, args) =>
            {
                if(args.EventType == AfterReadEventType(isPort))
                {
                    eventFired = true;
                    args.Value = valueFromEvent;
                }
            };

            var actual = Read(address, isPort);

            Assert.IsTrue(eventFired);
            Assert.AreEqual(valueFromEvent, actual);
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void ReadFromMemory_does_not_access_memory_if_Cancel_is_set_from_Before_event(bool isPort)
        {
            var address = Fixture.Create<byte>();
            var value = Fixture.Create<byte>();
            var memory = isPort ? Ports : Memory;

            memory.Setup(m => m[address]).Throws<Exception>();

            Sut.MemoryAccess += (sender, args) =>
            {
                if(args.EventType == BeforeReadEventType(isPort)) 
                {
                    args.CancelMemoryAccess = true;
                    args.Value = value;
                }
            };

            var actual = Read(address, isPort);

            Assert.AreEqual(value, actual);
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void ReadFromMemory_propagates_Cancel_from_Before_to_after_event(bool isPort)
        {
            var address = Fixture.Create<byte>();
            var memory = isPort ? Ports : Memory;

            memory.Setup(m => m[address]).Throws<Exception>();

            var eventFired = false;

            Sut.MemoryAccess += (sender, args) =>
            {
                if(args.EventType == BeforeReadEventType(isPort)) 
                {
                    args.CancelMemoryAccess = true;
                }
                else if(args.EventType == AfterReadEventType(isPort))
                {
                    eventFired = true;
                    Assert.IsTrue(args.CancelMemoryAccess);
                }
            };

            Read(address, isPort);

            Assert.IsTrue(eventFired);
        }

        [Test]
        public void FetchNextOpcode_reads_from_address_pointed_by_PC()
        {
            var address = Fixture.Create<ushort>();
            var value = Fixture.Create<byte>();

            Memory.Setup(m => m[address]).Returns(value);
            Sut.Registers.PC = address.ToShort();

            var actual = Sut.FetchNextOpcode();

            Assert.AreEqual(value, actual);
            Memory.Verify(m => m[address]);
        }

        [Test]
        public void FetchNextOpcode_increases_PC_by_one()
        {
            var address = Fixture.Create<ushort>();
            Sut.Registers.PC = address.ToShort();

            Sut.FetchNextOpcode();

            Assert.AreEqual(address.ToShort().Inc(), Sut.Registers.PC);
        }
    }
}
