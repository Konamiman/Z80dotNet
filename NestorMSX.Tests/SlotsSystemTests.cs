using System;
using System.Collections.Generic;
using Konamiman.NestorMSX.Hardware;
using Konamiman.NestorMSX.Misc;
using Konamiman.Z80dotNet;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;

namespace Konamiman.NestorMSX.Tests
{
    public class SlotsSystemTests
    {
        private const int MemorySize = ushort.MaxValue + 1;

        IFixture Fixture { get; set; }

        [SetUp]
        public void SetUp()
        {
            Fixture = new Fixture().Customize(new AutoMoqCustomization());
        }

        [Test]
        public void Can_create_instances()
        {
            var sut = new SlotsSystem(new Dictionary<SlotNumber, IMemory> {{NotExpandedSlot(), AnyMemory()}});
            Assert.IsNotNull(sut);
        }

        [Test]
        public void Announces_proper_size()
        {
            var sut = new SlotsSystem(new Dictionary<SlotNumber, IMemory> {{NotExpandedSlot(), AnyMemory()}});
            Assert.AreEqual(MemorySize, sut.Size);
        }

        #region Class instantiation

        [Test]
        public void Cannot_pass_null_to_constructor()
        {
            Assert.Throws<ArgumentNullException>(() => new SlotsSystem(null));
        }

        [Test]
        public void Cannot_pass_memory_with_no_64K_bytes_as_content()
        {
            var contents = new Dictionary<SlotNumber, IMemory>()
            {
                { Fixture.Create<SlotNumber>(), new PlainMemory(MemorySize + Fixture.Create<int>()) }
            };

            Assert.Throws<ArgumentException>(() => new SlotsSystem(contents));
        }

        [Test]
        public void Can_pass_empty_dictionary_to_constructor()
        {
            var contents = new Dictionary<SlotNumber, IMemory>();

            Assert.IsNotNull(new SlotsSystem(contents));
        }

        [Test]
        public void Slot_is_not_expanded_if_content_for_expanded_slot_is_passed_in_constructor()
        {
            var slotNumber = NotExpandedSlot();
            var contents = new Dictionary<SlotNumber, IMemory>()
            {
                { slotNumber, AnyMemory() }
            };

            var sut = new SlotsSystem(contents);
            Assert.False(sut.IsExpanded(slotNumber.PrimarySlotNumber));
        }

        [Test]
        public void Slot_is_expanded_if_content_for_expanded_slot_is_passed_in_constructor()
        {
            var slotNumber = ExpandedSlot();
            var contents = new Dictionary<SlotNumber, IMemory>()
            {
                { slotNumber, AnyMemory() }
            };

            var sut = new SlotsSystem(contents);
            Assert.True(sut.IsExpanded(slotNumber.PrimarySlotNumber));
        }

        [Test]
        public void Specified_slot_contain_its_contents_and_all_others_ends_up_containing_NotConnectedMemory()
        {
            var slot0contents = AnyMemory();
            var slot10contents = AnyMemory();

            var contents = new Dictionary<SlotNumber, IMemory>
            {
                { new SlotNumber(0), slot0contents },
                { new SlotNumber(1, 0), slot10contents },
                { new SlotNumber(1, 1), null },
            };

            var sut = new SlotsSystem(contents);

            Assert.AreEqual(slot0contents, sut.GetSlotContents(0));
            Assert.AreEqual(slot10contents, sut.GetSlotContents(new SlotNumber(1, 0)));
            Assert.IsInstanceOf<NotConnectedMemory>(sut.GetSlotContents(new SlotNumber(1, 1)));
            Assert.IsInstanceOf<NotConnectedMemory>(sut.GetSlotContents(new SlotNumber(1, 2)));
            Assert.IsInstanceOf<NotConnectedMemory>(sut.GetSlotContents(new SlotNumber(1, 3)));
            Assert.IsInstanceOf<NotConnectedMemory>(sut.GetSlotContents(2));
            Assert.IsInstanceOf<NotConnectedMemory>(sut.GetSlotContents(3));
        }

        #endregion

        #region Accessing the slot selection register

        [Test]
        public void Writing_slot_selection_register_changes_selected_primary_slots()
        {
            var sut = EmptySlotsSystem();

            byte portValue = 0xE4;  //11 10 01 00
            sut.WriteToSlotSelectionRegister(portValue);

            Assert.AreEqual(0, sut.GetCurrentSlot(0).PrimarySlotNumber);
            Assert.AreEqual(1, sut.GetCurrentSlot(1).PrimarySlotNumber);
            Assert.AreEqual(2, sut.GetCurrentSlot(2).PrimarySlotNumber);
            Assert.AreEqual(3, sut.GetCurrentSlot(3).PrimarySlotNumber);

            portValue = 0x1B;  //00 01 10 11
            sut.WriteToSlotSelectionRegister(portValue);

            Assert.AreEqual(3, sut.GetCurrentSlot(0).PrimarySlotNumber);
            Assert.AreEqual(2, sut.GetCurrentSlot(1).PrimarySlotNumber);
            Assert.AreEqual(1, sut.GetCurrentSlot(2).PrimarySlotNumber);
            Assert.AreEqual(0, sut.GetCurrentSlot(3).PrimarySlotNumber);
        }

        [Test]
        public void Writing_slot_selection_register_fires_event()
        {
            var sut = EmptySlotsSystem();
            var portValue = Fixture.Create<byte>();
            var eventFired = false;
            byte portValueInEvent = 0;

            sut.SlotSelectionRegisterWritten += (sender, args) =>
            {
                eventFired = true;
                portValueInEvent = args.Value;
            };

            sut.WriteToSlotSelectionRegister(portValue);

            Assert.True(eventFired);
            Assert.AreEqual(portValue, portValueInEvent);
        }

        [Test]
        public void Reading_slot_selection_register_returns_value_that_depends_on_connected_slots()
        {
            var sut = EmptySlotsSystem();

            sut.EnableSlot(0, 0);
            sut.EnableSlot(1, 1);
            sut.EnableSlot(2, 2);
            sut.EnableSlot(3, 3);

            var portValue = sut.ReadSlotSelectionRegister();
            Assert.AreEqual(0xE4, portValue);

            sut.EnableSlot(0, 3);
            sut.EnableSlot(1, 2);
            sut.EnableSlot(2, 1);
            sut.EnableSlot(3, 0);

            portValue = sut.ReadSlotSelectionRegister();
            Assert.AreEqual(0x1B, portValue);
        }

        #endregion

        private SlotsSystem EmptySlotsSystem()
        {
            return new SlotsSystem(
                new Dictionary<SlotNumber, IMemory> 
                {
                    { new SlotNumber(3, 0),  null }
                });
        }

        private SlotNumber NotExpandedSlot()
        {
            return new SlotNumber(SlotNumberPart());
        }

        private SlotNumber ExpandedSlot()
        {
            return new SlotNumber(SlotNumberPart(), SlotNumberPart());
        }

        private byte SlotNumberPart()
        {
            return (byte)(Fixture.Create<int>() & 3);
        }

        private IMemory AnyMemory()
        {
            var mock = new Mock<IMemory>();
            mock.SetupGet(m => m.Size).Returns(MemorySize);
            return mock.Object;
        }
    }
}
