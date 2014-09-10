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

        [SetUp]
        public void Setup()
        {
            Fixture = new Fixture();

            Sut = new Z80Processor();
        }

        [Test]
        public void Can_create_instances()
        {
            Assert.IsNotNull(Sut);
        }

        [Test]
        [TestCase(MemoryAccessMode.ReadAndWrite)]
        [TestCase(MemoryAccessMode.ReadOnly)]
        public void ReadFromMemory_accesses_memory_if_memory_mode_is_ReadAndWrite_or_ReadOnly(MemoryAccessMode accessMode)
        {
            var address = Fixture.Create<ushort>();
            var value = Fixture.Create<byte>();

            var memory = new Mock<IMemory>();
            memory.Setup(m => m[address]).Returns(value);
            Sut.Memory = memory.Object;
            Sut.SetMemoryAccessMode(address, 1, accessMode);

            var actual = Sut.ReadFromMemory(address);

            Assert.AreEqual(value, actual);
            memory.Verify(m => m[address]);
        }

        [Test]
        [TestCase(MemoryAccessMode.NotConnected)]
        [TestCase(MemoryAccessMode.WriteOnly)]
        public void ReadFromMemory_does_not_access_memory_if_memory_mode_is_NotConnected_or_WriteOnly(MemoryAccessMode accessMode)
        {
            var address = Fixture.Create<ushort>();
            var value = Fixture.Create<byte>();

            var memory = new Mock<IMemory>();
            memory.Setup(m => m[address]).Throws<Exception>();
            Sut.Memory = memory.Object;
            Sut.SetMemoryAccessMode(address, 1, accessMode);

            Sut.ReadFromMemory(address);
        }

        [Test]
        public void ReadFromMemory_fires_Before_event_with_appropriate_address_and_value()
        {
            var address = Fixture.Create<ushort>();

            var eventFired = false;

            Sut.MemoryAccess += (sender, args) =>
            {
                if(args.EventType == MemoryAccessEventType.BeforeMemoryRead)
                {
                    eventFired = true;
                    Assert.AreEqual(Sut, sender);
                    Assert.AreEqual(address, args.Address);
                    Assert.AreEqual(0xFF, args.Value);
                }
            };

            Sut.ReadFromMemory(address);

            Assert.IsTrue(eventFired);
        }

        [Test]
        public void ReadFromMemory_fires_After_event_with_appropriate_address_and_value_if_memory_is_accessed()
        {
            var address = Fixture.Create<ushort>();
            var value = Fixture.Create<byte>();

            var eventFired = false;

            var memory = new Mock<IMemory>();
            memory.Setup(m => m[address]).Returns(value);
            Sut.Memory = memory.Object;

            Sut.MemoryAccess += (sender, args) =>
            {
                if(args.EventType == MemoryAccessEventType.AfterMemoryRead)
                {
                    eventFired = true;
                    Assert.AreEqual(Sut, sender);
                    Assert.AreEqual(address, args.Address);
                    Assert.AreEqual(value, args.Value);
                }
            };

            var actual = Sut.ReadFromMemory(address);

            Assert.IsTrue(eventFired);
            Assert.AreEqual(value, actual);
        }

        [Test]
        public void ReadFromMemory_fires_After_event_with_same_value_as_Before_event_if_memory_is_not_accessed()
        {
            var address = Fixture.Create<ushort>();
            var value = Fixture.Create<byte>();

            var beforeEventFired = false;
            var afterEventFired = false;

            var memory = new Mock<IMemory>();
            memory.Setup(m => m[address]).Throws<Exception>();
            Sut.Memory = memory.Object;
            Sut.SetMemoryAccessMode(address, 1, MemoryAccessMode.NotConnected);

            Sut.MemoryAccess += (sender, args) =>
            {
                if(args.EventType == MemoryAccessEventType.BeforeMemoryRead)
                {
                    beforeEventFired = true;
                    Assert.AreEqual(address, args.Address);
                    args.Value = value;
                }
                if(args.EventType == MemoryAccessEventType.AfterMemoryRead)
                {
                    afterEventFired = true;
                    Assert.AreEqual(address, args.Address);
                    Assert.AreEqual(value, args.Value);
                }
            };

            var actual = Sut.ReadFromMemory(address);

            Assert.IsTrue(beforeEventFired);
            Assert.IsTrue(afterEventFired);
        }
        
        [Test]
        public void ReadFromMemory_returns_value_set_in_After_event()
        {
            var address = Fixture.Create<ushort>();
            var valueFromMemory = Fixture.Create<byte>();
            var valueFromEvent = Fixture.Create<byte>();

            var eventFired = false;

            var memory = new Mock<IMemory>();
            memory.Setup(m => m[address]).Returns(valueFromMemory);
            Sut.Memory = memory.Object;

            Sut.MemoryAccess += (sender, args) =>
            {
                if(args.EventType == MemoryAccessEventType.AfterMemoryRead)
                {
                    eventFired = true;
                    args.Value = valueFromEvent;
                }
            };

            var actual = Sut.ReadFromMemory(address);

            Assert.IsTrue(eventFired);
            Assert.AreEqual(valueFromEvent, actual);
        }

        [Test]
        public void ReadFromMemory_does_not_access_memory_if_Cancel_is_set_from_Before_event()
        {
            var address = Fixture.Create<ushort>();
            var value = Fixture.Create<byte>();

            var memory = new Mock<IMemory>();
            memory.Setup(m => m[address]).Throws<Exception>();
            Sut.Memory = memory.Object;

            Sut.MemoryAccess += (sender, args) =>
            {
                if(args.EventType == MemoryAccessEventType.BeforeMemoryRead) 
                {
                    args.CancelMemoryAccess = true;
                    args.Value = value;
                }
            };

            var actual = Sut.ReadFromMemory(address);

            Assert.AreEqual(value, actual);
        }

        [Test]
        public void ReadFromMemory_propagates_Cancel_from_Before_to_after_event()
        {
            var address = Fixture.Create<ushort>();

            var memory = new Mock<IMemory>();
            memory.Setup(m => m[address]).Throws<Exception>();
            Sut.Memory = memory.Object;

            var eventFired = false;

            Sut.MemoryAccess += (sender, args) =>
            {
                if(args.EventType == MemoryAccessEventType.BeforeMemoryRead) 
                {
                    args.CancelMemoryAccess = true;
                }
                else if(args.EventType == MemoryAccessEventType.AfterMemoryRead)
                {
                    eventFired = true;
                    Assert.IsTrue(args.CancelMemoryAccess);
                }
            };

            Sut.ReadFromMemory(address);

            Assert.IsTrue(eventFired);
        }
    }
}
