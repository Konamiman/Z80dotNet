using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Konamiman.NestorMSX.Hardware;
using Konamiman.Z80dotNet;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Konamiman.NestorMSX.Tests
{
    public class Tms9918Tests
    {
        Fixture Fixture { get; set; }
        Tms9918 Sut { get; set; }
        Mock<ITms9918DisplayRenderer> DisplayRenderer { get; set; }

        [SetUp]
        public void Setup()
        {
            Fixture = new Fixture();
            DisplayRenderer = new Mock<ITms9918DisplayRenderer>();
            Sut = new Tms9918(DisplayRenderer.Object);
            DisplayRenderer.ResetCalls();
        }

        [Test]
        public void Can_create_instances_and_initializes_properly()
        {
            new Tms9918(DisplayRenderer.Object);
            Verify(m => m.BlankScreen());
            Verify(m => m.SetScreenMode(0));
        }

        #region Setting screen mode

        [Test]
        public void Screen_mode_is_set_properly()
        {
            WriteControlRegister(0, 2);
            Verify(m => m.SetScreenMode(2));

            WriteControlRegister(0, 0);
            DisplayRenderer.ResetCalls();
            WriteControlRegister(1, 0x10);
            Verify(m => m.SetScreenMode(1));

            WriteControlRegister(1, 0x08);
            Verify(m => m.SetScreenMode(3));

            WriteControlRegister(1, 0);
            Verify(m => m.SetScreenMode(0));
        }

        #endregion

        #region VRAM access

        [Test]
        public void Can_read_vram_from_port_after_setup()
        {
            var address = RandomVramAddress();
            var value = Fixture.Create<byte>();
            Sut.WriteVram(address, value);

            SetupVramRead(address);
            var actual = Sut.ReadFromPort(0);

            Assert.AreEqual(value, actual);
        }
        
        [Test]
        public void Reads_vram_Sequentially()
        {
            var address = RandomVramAddress();
            var value1 = Fixture.Create<byte>();
            var value2 = Fixture.Create<byte>();
            Sut.WriteVram(address, value1);
            Sut.WriteVram(address + 1, value2);

            SetupVramRead(address);
            var actual = Sut.ReadFromPort(0);
            Assert.AreEqual(value1, actual);
            actual = Sut.ReadFromPort(0);
            Assert.AreEqual(value2, actual);
        }

        [Test]
        public void Reads_vram_overlapping_from_max_address_to_zero()
        {
            var value1 = Fixture.Create<byte>();
            var value2 = Fixture.Create<byte>();
            Sut.WriteVram(0x3FFF, value1);
            Sut.WriteVram(0, value2);

            SetupVramRead(0x3FFF);
            var actual = Sut.ReadFromPort(0);
            Assert.AreEqual(value1, actual);
            actual = Sut.ReadFromPort(0);
            Assert.AreEqual(value2, actual);
        }

        [Test]
        public void Can_write_to_vram_to_port_after_setup()
        {
            var address = RandomVramAddress();
            var oldValue = Fixture.Create<byte>();
            var value = Fixture.Create<byte>();
            Sut.WriteVram(address, oldValue);

            SetupVramWrite(address);
            Sut.WriteToPort(0, value);

            var actual = Sut.ReadVram(address);
            Assert.AreEqual(value, actual);
        }

        [Test]
        public void Writes_vram_Sequentially()
        {
            var address = RandomVramAddress();
            var value1 = Fixture.Create<byte>();
            var value2 = Fixture.Create<byte>();

            SetupVramWrite(address);
            Sut.WriteToPort(0, value1);
            Sut.WriteToPort(0, value2);

            var actual = Sut.ReadVram(address);
            Assert.AreEqual(value1, actual);
            actual = Sut.ReadVram(address+1);
            Assert.AreEqual(value2, actual);
        }

        [Test]
        public void Writes_vram_overlapping_from_max_address_to_zero()
        {
            var value1 = Fixture.Create<byte>();
            var value2 = Fixture.Create<byte>();

            SetupVramWrite(0x3FFF);
            Sut.WriteToPort(0, value1);
            Sut.WriteToPort(0, value2);

            var actual = Sut.ReadVram(0x3FFF);
            Assert.AreEqual(value1, actual);
            actual = Sut.ReadVram(0);
            Assert.AreEqual(value2, actual);
        }

        #endregion

        #region Screen on/off

        [Test]
        public void Notifies_renderer_of_blank_or_active_screen()
        {
            var value = Fixture.Create<byte>().WithBit(6, 0);
            WriteControlRegister(1, value);
            Verify(m => m.BlankScreen(), true);

            value = value.WithBit(6, 1);
            WriteControlRegister(1, value);
            Verify(m => m.ActivateScreen(), true);
        }

        [Test]
        public void Notifies_renderer_of_color_change()
        {
            var value = Fixture.Create<byte>();
            WriteControlRegister(7, value);

            Verify(m => m.SetForegroundColor((byte)(value >> 4)));
            Verify(m => m.SetBackgroundColor((byte)(value & 0x0F)));
        }

        #endregion

        #region Pattern name and generator tables

        [Test]
        public void Notifies_write_to_pattern_name_table()
        {
            var nameTableBaseAddressHighBits = Fixture.Create<byte>() & 0x0F;
            var nameTableBaseAddress = nameTableBaseAddressHighBits << 10;
            var offset = Fixture.Create<byte>();
            var value = Fixture.Create<byte>();

            WriteControlRegister(2, (byte)nameTableBaseAddressHighBits);

            SetupVramWrite(nameTableBaseAddress + offset);
            Sut.WriteToPort(0, value);

            Verify(m => m.WriteToNameTable(offset, value));
        }

        [Test]
        [TestCase(0, 768)]
        [TestCase(1, 960)]
        public void Does_not_notify_write_outside_pattern_name_table(int screenMode, int nameTableSize)
        {
            if(screenMode == 1)
                WriteControlRegister(1, ((byte)0).WithBit(4, 1));

            var nameTableBaseAddressHighBits = Fixture.Create<byte>() & 0x0F;
            var nameTableBaseAddress = nameTableBaseAddressHighBits << 10;
            var offset = nameTableSize - 1;
            var value = Fixture.Create<byte>();

            WriteControlRegister(2, (byte)nameTableBaseAddressHighBits);

            SetupVramWrite(nameTableBaseAddress + offset);
            Sut.WriteToPort(0, value);
            Verify(m => m.WriteToNameTable(offset, value), true);
            Sut.WriteToPort(0, value);
            DisplayRenderer.Verify(m => m.WriteToNameTable(It.IsAny<int>(), It.IsAny<byte>()), Times.Never);
        }

        [Test]
        public void Notifies_write_to_pattern_generator_table()
        {
            var patternTableBaseAddressHighBits = Fixture.Create<byte>() & 0x07;
            var patternTableBaseAddress = patternTableBaseAddressHighBits << 11;
            var offset = Fixture.Create<byte>();
            var value = Fixture.Create<byte>();

            WriteControlRegister(4, (byte)patternTableBaseAddressHighBits);

            SetupVramWrite(patternTableBaseAddress + offset);
            Sut.WriteToPort(0, value);

            Verify(m => m.WriteToPatternGeneratorTable(offset, value));
        }

        [Test]
        [TestCase(0, 2048)]
        [TestCase(1, 2048)]
        public void Does_not_notify_write_outside_pattern_generator_table(int screenMode, int patternTableSize)
        {
            if(screenMode == 1)
                WriteControlRegister(1, ((byte)0).WithBit(4, 1));

            var patternTableBaseAddressHighBits = Fixture.Create<byte>() & 0x07;
            var patternTableBaseAddress = patternTableBaseAddressHighBits << 11;
            var offset = patternTableSize - 1;
            var value = Fixture.Create<byte>();

            WriteControlRegister(4, (byte)patternTableBaseAddressHighBits);

            SetupVramWrite(patternTableBaseAddress + offset);
            Sut.WriteToPort(0, value);
            Verify(m => m.WriteToPatternGeneratorTable(offset, value), true);
            Sut.WriteToPort(0, value);
            DisplayRenderer.Verify(m => m.WriteToPatternGeneratorTable(It.IsAny<int>(), It.IsAny<byte>()), Times.Never);
        }

        [Test]
        public void Notifies_writing_name_table_on_bulk_vram_write()
        {
            var nameTableBaseAddressHighBits = Fixture.Create<byte>() & 0x0F;
            var nameTableBaseAddress = nameTableBaseAddressHighBits << 10;
            var offset = Fixture.Create<byte>();
            var values = Fixture.Create<byte[]>();

            WriteControlRegister(2, (byte)nameTableBaseAddressHighBits);

            Sut.SetVramContents(nameTableBaseAddress + offset, values);

            for(int i=0; i<values.Length; i++)
                Verify(m => m.WriteToNameTable(offset + i, values[i]));
        }

        [Test]
        public void Notifies_writing_pattern_generator_table_on_bulk_vram_write()
        {
            var patternTableBaseAddressHighBits = Fixture.Create<byte>() & 0x07;
            var patternTableBaseAddress = patternTableBaseAddressHighBits << 11;
            var offset = Fixture.Create<byte>();
            var values = Fixture.Create<byte[]>();

            WriteControlRegister(4, (byte)patternTableBaseAddressHighBits);

            Sut.SetVramContents(patternTableBaseAddress + offset, values);

            for(int i=0; i<values.Length; i++)
                Verify(m => m.WriteToPatternGeneratorTable(offset + i, values[i]));
        }

        #endregion

        #region Interrupts

        [Test]
        public void Sets_bit_7_of_status_Register_at_50Hz_and_clears_it_after_read()
        {
            bool bit7Set = false;
            Bit nextBitValue = 0;
            var sw = new Stopwatch();
            sw.Start();
            while (sw.ElapsedMilliseconds < 100) {
                if (Sut.ReadFromPort(1).GetBit(7) == 1) {
                    sw.Stop();
                    nextBitValue = Sut.ReadFromPort(1).GetBit(7);
                    bit7Set = true;
                    break;
                }
                Thread.Sleep(1);
            }

            Assert.True(bit7Set);
            Assert.GreaterOrEqual(sw.ElapsedMilliseconds, 20);
            Assert.LessOrEqual(sw.ElapsedMilliseconds, 30);
            Assert.AreEqual(0, nextBitValue);
        }

        [Test]
        public void Does_not_generate_interrupt_if_GINT_is_zero()
        {
            WriteControlRegister(1, 0);

            bool intLineActive = false;
            var sw = new Stopwatch();
            sw.Start();
            while(sw.ElapsedMilliseconds < 1000) {
                if(Sut.IntLineIsActive) {
                    intLineActive = true;
                    break;
                }
                Thread.Sleep(1);
            }
            sw.Stop();

            Assert.False(intLineActive);
        }

        [Test]
        public void Generates_interrupt_if_GINT_is_one_and_clears_it_after_status_register_read()
        {
            WriteControlRegister(1, ((byte)0).WithBit(5,1));

            bool intLineActive = false;
            bool intLineActiveAfterStatusRegisterRead = false;
            var sw = new Stopwatch();
            sw.Start();
            while(sw.ElapsedMilliseconds < 1000) {
                if(Sut.IntLineIsActive) {
                    intLineActive = true;
                    Sut.ReadFromPort(1);
                    intLineActiveAfterStatusRegisterRead = Sut.IntLineIsActive;
                    break;
                }
                Thread.Sleep(1);
            }
            sw.Stop();

            Assert.True(intLineActive);
            Assert.False(intLineActiveAfterStatusRegisterRead);
        }

        #endregion

        private void SetupVramRead(int address)
        {
            SetupVramAccess(address, 0);
        }

        private void SetupVramWrite(int address)
        {
            SetupVramAccess(address, 1);
        }

        private void SetupVramAccess(int address, Bit rwFlag)
        {
            Sut.WriteToPort(1, (byte)(address & 0xFF));
            Sut.WriteToPort(1, (byte)(((address >> 8) & 0x3F) | (rwFlag << 6)));
        }

        int RandomVramAddress()
        {
            return Fixture.Create<int>() & 0x3FFF;
        }

        void WriteControlRegister(int register, byte value)
        {
            Sut.WriteToPort(1, value);
            Sut.WriteToPort(1, (byte)(register | 0x80));
        }

        void Verify(Expression<Action<ITms9918DisplayRenderer>> expression, bool resetCalls = false)
        {
            DisplayRenderer.Verify(expression, Times.Once);
            if(resetCalls)
                DisplayRenderer.ResetCalls();
        }

    }
}
