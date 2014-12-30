using System;
using System.Collections.Generic;
using System.Linq;
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
            sut.EnableSlot(3, 0x8F);

            var portValue = sut.ReadSlotSelectionRegister();
            Assert.AreEqual(0xE4, portValue);

            sut.EnableSlot(0, 0x8F);
            sut.EnableSlot(1, 2);
            sut.EnableSlot(2, 1);
            sut.EnableSlot(3, 0);

            portValue = sut.ReadSlotSelectionRegister();
            Assert.AreEqual(0x1B, portValue);
        }

        #endregion

        #region EnableSlot, GetCurrentSlot

        [Test]
        public void EnableSlot_and_GetCurrentSlot_are_symmetrical()
        {
            var sut = EmptySlotsSystem();

            sut.EnableSlot(0, 0);
            sut.EnableSlot(1, 1);
            sut.EnableSlot(2, 2);
            sut.EnableSlot(3, 0x8F);

            Assert.AreEqual(0, sut.GetCurrentSlot(0));
            Assert.AreEqual(1, sut.GetCurrentSlot(1));
            Assert.AreEqual(2, sut.GetCurrentSlot(2));
            Assert.AreEqual(0x8F, sut.GetCurrentSlot(3));

            sut.EnableSlot(0, 0x8F);
            sut.EnableSlot(1, 2);
            sut.EnableSlot(2, 1);
            sut.EnableSlot(3, 0);

            Assert.AreEqual(0x8F, sut.GetCurrentSlot(0));
            Assert.AreEqual(2, sut.GetCurrentSlot(1));
            Assert.AreEqual(1, sut.GetCurrentSlot(2));
            Assert.AreEqual(0, sut.GetCurrentSlot(3));
        }

        [Test]
        public void EnableSlot_throws_exception_if_slot_does_not_exist()
        {
            var sut = EmptySlotsSystem();

            Assert.Throws<InvalidOperationException>(() => sut.EnableSlot(0, new SlotNumber(1, 2)));
        }

        #endregion

        #region GetSlotContents, SetSlotContents

        [Test]
        public void GetSlotContents_and_SetSlotContents_are_symmetrical()
        {
            var contentsForAllSlots = new[] {AnyMemory(), AnyMemory(), AnyMemory(), AnyMemory()};

            var sut = EmptySlotsSystem();

            sut.SetSlotContents(0, contentsForAllSlots[0]);
            sut.SetSlotContents(1, contentsForAllSlots[1]);
            sut.SetSlotContents(2, contentsForAllSlots[2]);
            sut.SetSlotContents(0x8F, contentsForAllSlots[3]);

            Assert.AreEqual(contentsForAllSlots[0], sut.GetSlotContents(0));
            Assert.AreEqual(contentsForAllSlots[1], sut.GetSlotContents(1));
            Assert.AreEqual(contentsForAllSlots[2], sut.GetSlotContents(2));
            Assert.AreEqual(contentsForAllSlots[3], sut.GetSlotContents(0x8F));
        }

        [Test]
        public void GetSlotContent_throws_exception_if_slot_does_not_exist()
        {
            var sut = EmptySlotsSystem();

            Assert.Throws<InvalidOperationException>(() => sut.GetSlotContents(new SlotNumber(1, 2)));
        }

        [Test]
        public void SetSlotContent_throws_exception_if_slot_does_not_exist()
        {
            var sut = EmptySlotsSystem();

            Assert.Throws<InvalidOperationException>(() => sut.SetSlotContents(new SlotNumber(1, 2), AnyMemory()));
        }

        [Test]
        public void SetSlotContent_sets_NotConnectedMemory_if_it_is_null()
        {
            var sut = EmptySlotsSystem();

            sut.SetSlotContents(0, AnyMemory());
            Assert.IsNotInstanceOf<NotConnectedMemory>(sut.GetSlotContents(0));

            sut.SetSlotContents(0, null);
            Assert.IsInstanceOf<NotConnectedMemory>(sut.GetSlotContents(0));
        }

        [Test]
        public void SetSlotContent_changes_visible_content_if_necessary()
        {
            var slotContents = Ram();
            var slotData = Fixture.Create<byte>();

            var sut = EmptySlotsSystem();
            sut.SetSlotContents(0x8F, slotContents);
            slotContents[0xC000] = slotData;
            sut.EnableSlot(3, 0x8F);
            Assert.AreEqual(slotData, sut[0xC000]);

            var newSlotContents = Ram();
            var newSlotData = Fixture.Create<byte>();
            newSlotContents[0xC000] = newSlotData;
            sut.SetSlotContents(0x8F, newSlotContents);
            Assert.AreEqual(newSlotData, sut[0xC000]);
        }

        #endregion

        #region Reading and writing data

        [Test]
        public void Reads_data_from_proper_slot()
        {
            var slotContents = new[] {Ram(), Ram(), Ram(), Ram()};
            var testData = ArrayOfRandomItems<byte>(8);

            var sut = EmptySlotsSystem();
            for(int i = 0; i < 4; i++) {
                slotContents[i][i*0x4000] = testData[i*2];
                slotContents[i][(i*0x4000)+0x3FFF] = testData[(i*2)+1];
                sut.SetSlotContents((byte)i, slotContents[i]);
                sut.EnableSlot(i, (byte)i);
            }

            for (int i = 0; i < 4; i++) {
                var address = i*0x4000;
                Assert.AreEqual(testData[i*2], sut[address]);
                Assert.AreEqual(testData[(i*2)+1], sut[address + 0x3FFF]);
            }
        }

        [Test]
        public void Writes_data_to_proper_slot()
        {
            var slotContents = new[] {Ram(), Ram(), Ram(), Ram()};
            var testData = ArrayOfRandomItems<byte>(8);

            var sut = EmptySlotsSystem();
            for(int i = 0; i < 4; i++) {
                sut.SetSlotContents((byte)i, slotContents[i]);
                sut.EnableSlot(i, (byte)i);
                var address = i*0x4000;
                sut[address] = testData[i*2];
                sut[address + 0x3FFF] = testData[(i*2)+1];
            }

            for (int i = 0; i < 4; i++) {
                var address = i*0x4000;
                Assert.AreEqual(testData[i*2], slotContents[i][address]);
                Assert.AreEqual(testData[(i*2)+1], slotContents[i][address+0x3FFF]);
            }
        }

        [Test]
        public void SetContents_writes_data_to_visible_slots()
        {
            var slotContents = new[] {Ram(), Ram(), Ram(), Ram()};
            foreach(var contents in slotContents)
                contents.SetContents(0, Enumerable.Repeat((byte)0xFF, 65536).ToArray());

            var sut = EmptySlotsSystem();
            sut.SetSlotContents(0, slotContents[0]);
            sut.SetSlotContents(1, slotContents[1]);
            sut.SetSlotContents(2, slotContents[2]);
            sut.SetSlotContents(0x8F, slotContents[3]);

            sut.SetContents(0x0000, RepeatByte(0, 16384));
            sut.SetContents(0x4000, RepeatByte(1, 16384));
            sut.SetContents(0x8000, RepeatByte(2, 16384));
            sut.SetContents(0xC000, RepeatByte(3, 16384));

            for(int i=0; i<65536; i++)
                Assert.AreEqual(i >> 14, sut[i]);
        }

        [Test]
        public void GetContents_reads_data_from_visible_slots()
        {
            var slotContents = new[] {Ram(), Ram(), Ram(), Ram()};
            foreach(var contents in slotContents)
                contents.SetContents(0, Enumerable.Repeat((byte)0xFF, 65536).ToArray());

            var sut = EmptySlotsSystem();
            sut.SetSlotContents(0, slotContents[0]);
            sut.SetSlotContents(1, slotContents[1]);
            sut.SetSlotContents(2, slotContents[2]);
            sut.SetSlotContents(0x8F, slotContents[3]);

            for(int i=0; i<65536; i++)
                sut[i] =(byte)(i >> 14);

            var allVisibleMemory = sut.GetContents(0, 65536);

            for(int i=0; i<65536; i++)
                Assert.AreEqual(i >> 14, allVisibleMemory[i]);
        }

        #endregion

        private byte[] RepeatByte(byte value, int count)
        {
            return Enumerable.Repeat(value, count).ToArray();
        }

        private T[] ArrayOfRandomItems<T>(int count)
        {
            var items = new List<T>();
            for(int i=0; i<count; i++)
                items.Add(Fixture.Create<T>());

            return items.ToArray();
        }

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

        private IMemory Ram()
        {
            return new PlainMemory(ushort.MaxValue + 1);
        }
    }
}
