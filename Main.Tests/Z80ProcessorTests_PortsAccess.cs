using AutoFixture;
using NUnit.Framework;

namespace Konamiman.Z80dotNet.Tests
{
    public class Z80ProcessorTests_PortsAccess
    {
        Z80ProcessorForTests Sut { get; set; }
        Fixture Fixture { get; set; }

        [SetUp]
        public void Setup()
        {
            Fixture = new Fixture();

            Sut = new Z80ProcessorForTests {
                PortsSpace = new PlainMemory(65536)
            };
        }

        [Test]
        public void Test_IN_A_without_extended_port_access()
        {
            Sut.PortsSpace[0x34] = 0xAA;
            Sut.PortsSpace[0x1234] = 0xBB;

            Sut.Registers.A = 0x12;
            Execute(0xDB, 0x34); // IN A,(0x34)

            Assert.That(Sut.Registers.A, Is.EqualTo(0xAA));
        }

        [Test]
        public void Test_IN_A_with_extended_port_access()
        {
            Sut.UseExtendedPortsSpace = true;

            Sut.PortsSpace[0x34] = 0xAA;
            Sut.PortsSpace[0x1234] = 0xBB;

            Sut.Registers.A = 0x12;
            Execute(0xDB, 0x34); // IN A,(0x34)

            Assert.That(Sut.Registers.A, Is.EqualTo(0xBB));
        }

        [Test]
        public void Test_IN_r_without_extended_port_access()
        {
            Sut.PortsSpace[0x34] = 0xAA;
            Sut.PortsSpace[0x1234] = 0xBB;

            Sut.Registers.B = 0x12;
            Sut.Registers.C = 0x34;
            Execute(0xED, 0x50); // IN D,(C)

            Assert.That(Sut.Registers.D, Is.EqualTo(0xAA));
        }

        [Test]
        public void Test_IN_r_with_extended_port_access()
        {
            Sut.UseExtendedPortsSpace = true;

            Sut.PortsSpace[0x34] = 0xAA;
            Sut.PortsSpace[0x1234] = 0xBB;

            Sut.Registers.B = 0x12;
            Sut.Registers.C = 0x34;
            Execute(0xED, 0x50); // IN D,(C)

            Assert.That(Sut.Registers.D, Is.EqualTo(0xBB));
        }

        [Test]
        public void Test_INI_without_extended_port_access()
        {
            Sut.PortsSpace[0x34] = 0xAA;
            Sut.PortsSpace[0x1234] = 0xBB;

            Sut.Registers.B = 0x12;
            Sut.Registers.C = 0x34;
            Sut.Registers.HL = 0x1000;
            Execute(0xED, 0xA2); // INI

            Assert.That(Sut.Memory[0x1000], Is.EqualTo(0xAA));
        }

        [Test]
        public void Test_INI_with_extended_port_access()
        {
            Sut.UseExtendedPortsSpace = true;

            Sut.PortsSpace[0x34] = 0xAA;
            Sut.PortsSpace[0x1234] = 0xBB;

            Sut.Registers.B = 0x12;
            Sut.Registers.C = 0x34;
            Sut.Registers.HL = 0x1000;
            Execute(0xED, 0xA2); // INI

            Assert.That(Sut.Memory[0x1000], Is.EqualTo(0xBB));
        }

        [Test]
        public void Test_IND_without_extended_port_access()
        {
            Sut.PortsSpace[0x34] = 0xAA;
            Sut.PortsSpace[0x1234] = 0xBB;

            Sut.Registers.B = 0x12;
            Sut.Registers.C = 0x34;
            Sut.Registers.HL = 0x1000;
            Execute(0xED, 0xAA); // IND

            Assert.That(Sut.Memory[0x1000], Is.EqualTo(0xAA));
        }

        [Test]
        public void Test_IND_with_extended_port_access()
        {
            Sut.UseExtendedPortsSpace = true;

            Sut.PortsSpace[0x34] = 0xAA;
            Sut.PortsSpace[0x1234] = 0xBB;

            Sut.Registers.B = 0x12;
            Sut.Registers.C = 0x34;
            Sut.Registers.HL = 0x1000;
            Execute(0xED, 0xAA); // INI

            Assert.That(Sut.Memory[0x1000], Is.EqualTo(0xBB));
        }

        [Test]
        public void Test_INIR_without_extended_port_access()
        {
            Sut.PortsSpace[0x34] = 0xAA;

            Sut.Registers.B = 0x03;
            Sut.Registers.C = 0x34;
            Sut.Registers.HL = 0x1000;
            Execute(0xED, 0xB2); // INIR
            Execute(0xED, 0xB2); // INIR
            Execute(0xED, 0xB2); // INIR

            Assert.Multiple(() => {
                Assert.That(Sut.Memory[0x1000], Is.EqualTo(0xAA));
                Assert.That(Sut.Memory[0x1001], Is.EqualTo(0xAA));
                Assert.That(Sut.Memory[0x1002], Is.EqualTo(0xAA));
            });
        }

        [Test]
        public void Test_INIR_with_extended_port_access()
        {
            Sut.UseExtendedPortsSpace = true;

            Sut.PortsSpace[0x0334] = 0xAA;
            Sut.PortsSpace[0x0234] = 0xBB;
            Sut.PortsSpace[0x0134] = 0xCC;

            Sut.Registers.B = 0x03;
            Sut.Registers.C = 0x34;
            Sut.Registers.HL = 0x1000;
            Execute(0xED, 0xB2); // INIR
            Execute(0xED, 0xB2); // INIR
            Execute(0xED, 0xB2); // INIR

            Assert.Multiple(() => {
                Assert.That(Sut.Memory[0x1000], Is.EqualTo(0xAA));
                Assert.That(Sut.Memory[0x1001], Is.EqualTo(0xBB));
                Assert.That(Sut.Memory[0x1002], Is.EqualTo(0xCC));
            });
        }

        [Test]
        public void Test_INDR_without_extended_port_access()
        {
            Sut.PortsSpace[0x34] = 0xAA;

            Sut.Registers.B = 0x03;
            Sut.Registers.C = 0x34;
            Sut.Registers.HL = 0x1000;
            Execute(0xED, 0xBA); // INDR
            Execute(0xED, 0xBA); // INDR
            Execute(0xED, 0xBA); // INDR

            Assert.Multiple(() => {
                Assert.That(Sut.Memory[0x1000], Is.EqualTo(0xAA));
                Assert.That(Sut.Memory[0x0FFF], Is.EqualTo(0xAA));
                Assert.That(Sut.Memory[0x0FFE], Is.EqualTo(0xAA));
            });
        }

        [Test]
        public void Test_INDR_with_extended_port_access()
        {
            Sut.UseExtendedPortsSpace = true;

            Sut.PortsSpace[0x0334] = 0xAA;
            Sut.PortsSpace[0x0234] = 0xBB;
            Sut.PortsSpace[0x0134] = 0xCC;

            Sut.Registers.B = 0x03;
            Sut.Registers.C = 0x34;
            Sut.Registers.HL = 0x1000;
            Execute(0xED, 0xBA); // INDR
            Execute(0xED, 0xBA); // INDR
            Execute(0xED, 0xBA); // INDR

            Assert.Multiple(() => {
                Assert.That(Sut.Memory[0x1000], Is.EqualTo(0xAA));
                Assert.That(Sut.Memory[0x0FFF], Is.EqualTo(0xBB));
                Assert.That(Sut.Memory[0x0FFE], Is.EqualTo(0xCC));
            });
        }

        [Test]
        public void Test_OUT_A_without_extended_port_access()
        {
            Sut.Registers.A = 0x12;
            Execute(0xD3, 0x34); // OUT (0x34),A

            Assert.That(Sut.PortsSpace[0x34], Is.EqualTo(0x12));
        }

        [Test]
        public void Test_OUT_A_with_extended_port_access()
        {
            Sut.UseExtendedPortsSpace = true;

            Sut.Registers.A = 0x12;
            Execute(0xD3, 0x34); // OUT (0x34),A

            Assert.That(Sut.PortsSpace[0x1234], Is.EqualTo(0x12));
        }

        [Test]
        public void Test_OUT_r_without_extended_port_access()
        {
            Sut.Registers.B = 0x12;
            Sut.Registers.C = 0x34;
            Sut.Registers.D = 0xAA;
            Execute(0xED, 0x51); // OUT (C),D

            Assert.That(Sut.PortsSpace[0x34], Is.EqualTo(0xAA));
        }

        [Test]
        public void Test_OUT_r_with_extended_port_access()
        {
            Sut.UseExtendedPortsSpace = true;

            Sut.Registers.B = 0x12;
            Sut.Registers.C = 0x34;
            Sut.Registers.D = 0xAA;
            Execute(0xED, 0x51); // OUT (C),D

            Assert.That(Sut.PortsSpace[0x1234], Is.EqualTo(0xAA));
        }

        [Test]
        public void Test_OUTI_without_extended_port_access()
        {
            Sut.Registers.B = 0x12;
            Sut.Registers.C = 0x34;
            Sut.Registers.HL = 0x1000;
            Sut.Memory[0x1000] = 0xAA;
            Execute(0xED, 0xA3); // OUTI 

            Assert.That(Sut.PortsSpace[0x34], Is.EqualTo(0xAA));
        }

        [Test]
        public void Test_OUTI_with_extended_port_access()
        {
            Sut.UseExtendedPortsSpace = true;

            Sut.Registers.B = 0x12;
            Sut.Registers.C = 0x34;
            Sut.Registers.HL = 0x1000;
            Sut.Memory[0x1000] = 0xAA;
            Execute(0xED, 0xA3); // OUTI 

            Assert.That(Sut.PortsSpace[0x1234], Is.EqualTo(0xAA));
        }

        [Test]
        public void Test_OUTD_without_extended_port_access()
        {
            Sut.Registers.B = 0x12;
            Sut.Registers.C = 0x34;
            Sut.Registers.HL = 0x1000;
            Sut.Memory[0x1000] = 0xAA;
            Execute(0xED, 0xAB); // OUTD

            Assert.That(Sut.PortsSpace[0x34], Is.EqualTo(0xAA));
        }

        [Test]
        public void Test_OUTD_with_extended_port_access()
        {
            Sut.UseExtendedPortsSpace = true;

            Sut.Registers.B = 0x12;
            Sut.Registers.C = 0x34;
            Sut.Registers.HL = 0x1000;
            Sut.Memory[0x1000] = 0xAA;
            Execute(0xED, 0xAB); // OUTD

            Assert.That(Sut.PortsSpace[0x1234], Is.EqualTo(0xAA));
        }

        [Test]
        public void Test_OTIR_without_extended_port_access()
        {
            Sut.Registers.B = 0x03;
            Sut.Registers.C = 0x34;
            Sut.Registers.HL = 0x1000;
            Sut.Memory[0x1000] = 0xAA;
            Sut.Memory[0x1001] = 0xBB;
            Sut.Memory[0x1002] = 0xCC;
            Execute(0xED, 0xB3); // OTIR
            Execute(0xED, 0xB3); // OTIR
            Execute(0xED, 0xB3); // OTIR

            Assert.That(Sut.PortsSpace[0x34], Is.EqualTo(0xCC));
        }

        [Test]
        public void Test_OTIR_with_extended_port_access()
        {
            Sut.UseExtendedPortsSpace = true;

            Sut.Registers.B = 0x03;
            Sut.Registers.C = 0x34;
            Sut.Registers.HL = 0x1000;
            Sut.Memory[0x1000] = 0xAA;
            Sut.Memory[0x1001] = 0xBB;
            Sut.Memory[0x1002] = 0xCC;
            Execute(0xED, 0xB3); // OTIR
            Execute(0xED, 0xB3); // OTIR
            Execute(0xED, 0xB3); // OTIR

            Assert.Multiple(() => {
                Assert.That(Sut.PortsSpace[0x0334], Is.EqualTo(0xAA));
                Assert.That(Sut.PortsSpace[0x0234], Is.EqualTo(0xBB));
                Assert.That(Sut.PortsSpace[0x0134], Is.EqualTo(0xCC));
            });
        }

        [Test]
        public void Test_OTDR_without_extended_port_access()
        {
            Sut.Registers.B = 0x03;
            Sut.Registers.C = 0x34;
            Sut.Registers.HL = 0x1000;
            Sut.Memory[0x1000] = 0xAA;
            Sut.Memory[0x0FFF] = 0xBB;
            Sut.Memory[0x0FFE] = 0xCC;
            Execute(0xED, 0xBB); // OTDR
            Execute(0xED, 0xBB); // OTDR
            Execute(0xED, 0xBB); // OTDR

            Assert.That(Sut.PortsSpace[0x34], Is.EqualTo(0xCC));
        }

        [Test]
        public void Test_OTDR_with_extended_port_access()
        {
            Sut.UseExtendedPortsSpace = true;

            Sut.Registers.B = 0x03;
            Sut.Registers.C = 0x34;
            Sut.Registers.HL = 0x1000;
            Sut.Memory[0x1000] = 0xAA;
            Sut.Memory[0x0FFF] = 0xBB;
            Sut.Memory[0x0FFE] = 0xCC;
            Execute(0xED, 0xBB); // OTDR
            Execute(0xED, 0xBB); // OTDR
            Execute(0xED, 0xBB); // OTDR

            Assert.Multiple(() => {
                Assert.That(Sut.PortsSpace[0x0334], Is.EqualTo(0xAA));
                Assert.That(Sut.PortsSpace[0x0234], Is.EqualTo(0xBB));
                Assert.That(Sut.PortsSpace[0x0134], Is.EqualTo(0xCC));
            });
        }

        [Test]
        public void Test_memory_event_without_extended_port_access()
        {
            ushort address = 0;

            Sut.MemoryAccess += (object sender, MemoryAccessEventArgs e) => {
                address = e.Address;
            };

            Sut.Registers.A = 0x12;
            Execute(0xDB, 0x34); // IN A,(0x34)

            Assert.That(address, Is.EqualTo(0x34));
        }

        [Test]
        public void Test_memory_event_with_extended_port_access()
        {
            Sut.UseExtendedPortsSpace = true;

            ushort address = 0;

            Sut.MemoryAccess += (object sender, MemoryAccessEventArgs e) => {
                address = e.Address;
            };

            Sut.Registers.A = 0x12;
            Execute(0xDB, 0x34); // IN A,(0x34)

            Assert.That(address, Is.EqualTo(0x1234));
        }

        private void Execute(params byte[] opcodes)
        {
            Sut.Memory.SetContents(0, opcodes);
            Sut.Registers.PC = 0;
            Sut.ExecuteNextInstruction();
        }
    }
}
