using Moq;
using NUnit.Framework;
using AutoFixture;
using System;

namespace Konamiman.Z80dotNet.Tests
{
    public class Z80ProcessorTests_Configuration
    {
        private const int MemorySpaceSize = 65536;
        private const int PortSpaceSize = 256;

        Z80ProcessorForTests Sut { get; set; }
        Fixture Fixture { get; set; }

        [SetUp]
        public void Setup()
        {
            Fixture = new Fixture();

            Sut = new Z80ProcessorForTests();
        }

        [Test]
        public void Can_create_instances()
        {
            Assert.That(Sut, Is.Not.Null);
        }

        [Test]
        public void Has_proper_defaults()
        {
            Assert.Multiple(() =>
            {
                Assert.That(Sut.ClockFrequencyInMHz, Is.EqualTo(4));
                Assert.That(Sut.ClockSpeedFactor, Is.EqualTo(1));
            });

            Assert.That(Sut.AutoStopOnDiPlusHalt);
            Assert.That(Sut.AutoStopOnRetWithStackEmpty, Is.False);
            Assert.That(Sut.StartOfStack, Is.EqualTo(0xFFFF.ToShort()));

            Assert.That(Sut.Memory, Is.InstanceOf<PlainMemory>());
            Assert.That(Sut.Memory.Size, Is.EqualTo(65536));
            Assert.That(Sut.PortsSpace, Is.InstanceOf<PlainMemory>());
            Assert.That(Sut.PortsSpace.Size, Is.EqualTo(256));

            for(int i = 0; i < 65536; i++) {
                Assert.Multiple(() =>
                {
                    Assert.That(Sut.GetMemoryAccessMode((ushort)i), Is.EqualTo(MemoryAccessMode.ReadAndWrite));
                    Assert.That(Sut.GetMemoryWaitStatesForM1((ushort)i), Is.EqualTo(0));
                    Assert.That(Sut.GetMemoryWaitStatesForNonM1((ushort)i), Is.EqualTo(0));
                });
            }       
            for(int i = 0; i < 256; i++) {
                Assert.Multiple(() =>
                {
                    Assert.That(Sut.GetPortAccessMode((byte)i), Is.EqualTo(MemoryAccessMode.ReadAndWrite));
                    Assert.That(Sut.GetPortWaitStates((byte)i), Is.EqualTo(0));
                });
            }

            Assert.That(Sut.Registers, Is.InstanceOf<Z80Registers>());

            Assert.That(Sut.InstructionExecutor, Is.InstanceOf<Z80InstructionExecutor>());
            Assert.That(Sut, Is.SameAs(Sut.InstructionExecutor.ProcessorAgent));
            Assert.That(Sut.ClockSynchronizer, Is.InstanceOf<ClockSynchronizer>());

            Assert.Multiple(() =>
            {
                Assert.That(Sut.StopReason, Is.EqualTo(StopReason.NeverRan));
                Assert.That(Sut.State, Is.EqualTo(ProcessorState.Stopped));
            });
            Assert.That(Sut.IsHalted, Is.False);
            Assert.That(Sut.UserState, Is.Null);
        }

        [Test]
        public void Reset_sets_registers_properly()
        {
            Sut.Registers.IFF1 = 1;
            Sut.Registers.IFF1 = 1;
            Sut.Registers.PC = 1;
            Sut.Registers.AF = 0;
            Sut.Registers.SP = 0;
            Sut.InterruptMode = 1;
            Sut.SetIsHalted();
            
            Sut.Reset();

            Assert.Multiple(() =>
            {
                Assert.That(Sut.Registers.AF, Is.EqualTo(0xFFFF.ToShort()));
                Assert.That(Sut.Registers.SP, Is.EqualTo(0xFFFF.ToShort()));
                Assert.That(Sut.Registers.PC, Is.EqualTo(0));
                Assert.That(Sut.Registers.IFF1.Value, Is.EqualTo(0));
                Assert.That(Sut.Registers.IFF2.Value, Is.EqualTo(0));
                Assert.That(Sut.InterruptMode, Is.EqualTo(0));

                Assert.That(Sut.TStatesElapsedSinceReset, Is.EqualTo(0));
            });

            Assert.That(Sut.IsHalted, Is.False);
        }

        [Test]
        public void Interrupt_mode_can_be_set_to_0_1_or_2()
        {
            Sut.InterruptMode = 0;
            Assert.That(Sut.InterruptMode, Is.EqualTo(0));

            Sut.InterruptMode = 1;
            Assert.That(Sut.InterruptMode, Is.EqualTo(1));

            Sut.InterruptMode = 2;
            Assert.That(Sut.InterruptMode, Is.EqualTo(2));
        }

        [Test]
        public void Interrupt_mode_cannot_be_set_to_higher_than_2()
        {
            Assert.Throws<ArgumentException>(() => Sut.InterruptMode = 3);
        }

        [Test]
        public void SetMemoryAccessMode_and_GetMemoryAccessMode_are_consistent()
        {
            Sut.SetMemoryAccessMode(0, 0x4000, MemoryAccessMode.NotConnected);
            Sut.SetMemoryAccessMode(0x4000, 0x4000, MemoryAccessMode.ReadAndWrite);
            Sut.SetMemoryAccessMode(0x8000, 0x4000, MemoryAccessMode.ReadOnly);
            Sut.SetMemoryAccessMode(0xC000, 0x4000, MemoryAccessMode.WriteOnly);

            Assert.Multiple(() =>
            {
                Assert.That(Sut.GetMemoryAccessMode(0), Is.EqualTo(MemoryAccessMode.NotConnected));
                Assert.That(Sut.GetMemoryAccessMode(0x3FFF), Is.EqualTo(MemoryAccessMode.NotConnected));
                Assert.That(Sut.GetMemoryAccessMode(0x4000), Is.EqualTo(MemoryAccessMode.ReadAndWrite));
                Assert.That(Sut.GetMemoryAccessMode(0x7FFF), Is.EqualTo(MemoryAccessMode.ReadAndWrite));
                Assert.That(Sut.GetMemoryAccessMode(0x8000), Is.EqualTo(MemoryAccessMode.ReadOnly));
                Assert.That(Sut.GetMemoryAccessMode(0xBFFF), Is.EqualTo(MemoryAccessMode.ReadOnly));
                Assert.That(Sut.GetMemoryAccessMode(0xC000), Is.EqualTo(MemoryAccessMode.WriteOnly));
                Assert.That(Sut.GetMemoryAccessMode(0xFFFF), Is.EqualTo(MemoryAccessMode.WriteOnly));
            });
        }

        [Test]
        public void SetMemoryAccessMode_works_when_address_plus_length_are_on_memory_size_boundary()
        {
           var value = Fixture.Create<MemoryAccessMode>();
           var length = Fixture.Create<byte>();

           Sut.SetMemoryAccessMode((ushort)(MemorySpaceSize - length), length, MemoryAccessMode.NotConnected);
        }

        [Test]
        public void SetMemoryAccessMode_fails_when_address_plus_length_are_beyond_memory_size_boundary()
        {
            var value = Fixture.Create<MemoryAccessMode>();
            var length = Fixture.Create<byte>();

            Assert.Throws<ArgumentException>(() => Sut.SetMemoryAccessMode((ushort)(MemorySpaceSize - length), length + 1, MemoryAccessMode.NotConnected));
        }

        [Test]
        public void SetMemoryAccessMode_fails_when_length_is_negative()
        {
            var value = Fixture.Create<MemoryAccessMode>();

            Assert.Throws<ArgumentException>(() => Sut.SetMemoryAccessMode(0, -1, value));
        }

        [Test]
        public void SetPortsSpaceAccessMode_and_GetPortsSpaceAccessMode_are_consistent()
        {
            Sut.SetPortsSpaceAccessMode(0, 64, MemoryAccessMode.NotConnected);
            Sut.SetPortsSpaceAccessMode(64, 64, MemoryAccessMode.ReadAndWrite);
            Sut.SetPortsSpaceAccessMode(128, 64, MemoryAccessMode.ReadOnly);
            Sut.SetPortsSpaceAccessMode(192, 64, MemoryAccessMode.WriteOnly);

            Assert.Multiple(() =>
            {
                Assert.That(Sut.GetPortAccessMode(0), Is.EqualTo(MemoryAccessMode.NotConnected));
                Assert.That(Sut.GetPortAccessMode(63), Is.EqualTo(MemoryAccessMode.NotConnected));
                Assert.That(Sut.GetPortAccessMode(64), Is.EqualTo(MemoryAccessMode.ReadAndWrite));
                Assert.That(Sut.GetPortAccessMode(127), Is.EqualTo(MemoryAccessMode.ReadAndWrite));
                Assert.That(Sut.GetPortAccessMode(128), Is.EqualTo(MemoryAccessMode.ReadOnly));
                Assert.That(Sut.GetPortAccessMode(191), Is.EqualTo(MemoryAccessMode.ReadOnly));
                Assert.That(Sut.GetPortAccessMode(192), Is.EqualTo(MemoryAccessMode.WriteOnly));
                Assert.That(Sut.GetPortAccessMode(255), Is.EqualTo(MemoryAccessMode.WriteOnly));
            });
        }

        [Test]
        public void SetPortsAccessMode_works_when_address_plus_length_are_on_ports_space_size_boundary()
        {
            var value = Fixture.Create<MemoryAccessMode>();
            var length = Fixture.Create<byte>();

            Sut.SetPortsSpaceAccessMode((byte)(PortSpaceSize - length), length, MemoryAccessMode.NotConnected);
        }

        [Test]
        public void SetPortsAccessMode_fails_when_address_plus_length_are_beyond_ports_space_size_boundary()
        {
            var value = Fixture.Create<MemoryAccessMode>();
            var length = Fixture.Create<byte>();

            Assert.Throws<ArgumentException>(() => Sut.SetPortsSpaceAccessMode((byte)(PortSpaceSize - length), length + 1, MemoryAccessMode.NotConnected));
        }

        [Test]
        public void SetPortsSpaceAccessMode_fails_when_length_is_negative()
        {
            var value = Fixture.Create<MemoryAccessMode>();

            Assert.Throws<ArgumentException>(() => Sut.SetPortsSpaceAccessMode(0, -1, value));
        }

        [Test]
        public void SetMemoryWaitStatesForM1_and_GetMemoryWaitStatesForM1_are_consistent()
        {
            var value1 = Fixture.Create<byte>();
            var value2 = Fixture.Create<byte>();

            Sut.SetMemoryWaitStatesForM1(0, 0x8000, value1);
            Sut.SetMemoryWaitStatesForM1(0x8000, 0x8000, value2);

            Assert.Multiple(() =>
            {
                Assert.That(Sut.GetMemoryWaitStatesForM1(0), Is.EqualTo(value1));
                Assert.That(Sut.GetMemoryWaitStatesForM1(0x7FFF), Is.EqualTo(value1));
                Assert.That(Sut.GetMemoryWaitStatesForM1(0x8000), Is.EqualTo(value2));
                Assert.That(Sut.GetMemoryWaitStatesForM1(0xFFFF), Is.EqualTo(value2));
            });
        }

        [Test]
        public void SetMemoryWaitStatesForM1_works_when_address_plus_length_are_in_memory_size_boundary()
        {
            var value = Fixture.Create<byte>();
            var length = Fixture.Create<byte>();

            Sut.SetMemoryWaitStatesForM1((ushort)(MemorySpaceSize - length), length, value);
        }

        [Test]
        public void SetMemoryWaitStatesForM1_fails_when_address_plus_length_are_beyond_memory_size_boundary()
        {
            var value = Fixture.Create<byte>();
            var length = Fixture.Create<byte>();

            Assert.Throws<ArgumentException>(
                () => Sut.SetMemoryWaitStatesForM1((ushort)(MemorySpaceSize - length), length + 1, value));
        }

        [Test]
        public void SetMemoryWaitStatesForM1_fails_when_length_is_negative()
        {
            var value = Fixture.Create<byte>();

            Assert.Throws<ArgumentException>(
                () => Sut.SetMemoryWaitStatesForM1(0, -1, value));
        }
        
        [Test]
        public void SetMemoryWaitStatesForNonM1_and_GetMemoryWaitStatesForNonM1_are_consistent()
        {
            var value1 = Fixture.Create<byte>();
            var value2 = Fixture.Create<byte>();

            Sut.SetMemoryWaitStatesForNonM1(0, 0x8000, value1);
            Sut.SetMemoryWaitStatesForNonM1(0x8000, 0x8000, value2);

            Assert.Multiple(() =>
            {
                Assert.That(Sut.GetMemoryWaitStatesForNonM1(0), Is.EqualTo(value1));
                Assert.That(Sut.GetMemoryWaitStatesForNonM1(0x7FFF), Is.EqualTo(value1));
                Assert.That(Sut.GetMemoryWaitStatesForNonM1(0x8000), Is.EqualTo(value2));
                Assert.That(Sut.GetMemoryWaitStatesForNonM1(0xFFFF), Is.EqualTo(value2));
            });
        }

        [Test]
        public void SetMemoryWaitStatesForNonM1_works_when_address_plus_length_are_in_memory_size_boundary()
        {
            var value = Fixture.Create<byte>();
            var length = Fixture.Create<byte>();

            Sut.SetMemoryWaitStatesForNonM1((ushort)(MemorySpaceSize - length), length, value);
        }

        [Test]
        public void SetMemoryWaitStatesForNonM1_fails_when_address_plus_length_are_beyond_memory_size_boundary()
        {
            var value = Fixture.Create<byte>();
            var length = Fixture.Create<byte>();

            Assert.Throws<ArgumentException>(
                () => Sut.SetMemoryWaitStatesForNonM1((ushort)(MemorySpaceSize - length), length + 1, value));
        }

        [Test]
        public void SetMemoryWaitStatesForNonM1_fails_when_length_is_negative()
        {
            var value = Fixture.Create<byte>();

            Assert.Throws<ArgumentException>(
                () => Sut.SetMemoryWaitStatesForNonM1(0, -1, value));
        }
        
        [Test]
        public void SetPortWaitStates_and_GetPortWaitStates_are_consistent()
        {
            var value1 = Fixture.Create<byte>();
            var value2 = Fixture.Create<byte>();

            Sut.SetPortWaitStates(0, 128, value1);
            Sut.SetPortWaitStates(128, 128, value2);

            Assert.Multiple(() =>
            {
                Assert.That(Sut.GetPortWaitStates(0), Is.EqualTo(value1));
                Assert.That(Sut.GetPortWaitStates(127), Is.EqualTo(value1));
                Assert.That(Sut.GetPortWaitStates(128), Is.EqualTo(value2));
                Assert.That(Sut.GetPortWaitStates(255), Is.EqualTo(value2));
            });
        }

        [Test]
        public void SetPortWaitStates_works_when_address_plus_length_are_in_memory_size_boundary()
        {
            var value = Fixture.Create<byte>();
            var length = Fixture.Create<byte>();

            Sut.SetPortWaitStates((ushort)(PortSpaceSize - length), length, value);
        }

        [Test]
        public void SetPortWaitStates_fails_when_address_plus_length_are_beyond_memory_size_boundary()
        {
            var value = Fixture.Create<byte>();
            var length = Fixture.Create<byte>();

            Assert.Throws<ArgumentException>(
                () => Sut.SetPortWaitStates((ushort)(PortSpaceSize - length), length + 1, value));
        }

        [Test]
        public void SetPortWaitStates_fails_when_length_is_negative()
        {
            var value = Fixture.Create<byte>();

            Assert.Throws<ArgumentException>(
                () => Sut.SetPortWaitStates(0, -1, value));
        }

        [Test]
        public void Can_set_Memory_to_non_null_value()
        {
            var value = new Mock<IMemory>().Object;
            Sut.Memory = value;
            Assert.That(Sut.Memory, Is.EqualTo(value));
        }

        [Test]
        public void Cannot_set_Memory_to_null()
        {
            Assert.Throws<ArgumentNullException>(() => Sut.Memory = null);
        }

        [Test]
        public void Can_set_Registers_to_non_null_value()
        {
            var value = new Mock<IZ80Registers>().Object;
            Sut.Registers = value;
            Assert.That(Sut.Registers, Is.EqualTo(value));
        }

        [Test]
        public void Cannot_set_Registers_to_null()
        {
            Assert.Throws<ArgumentNullException>(() => Sut.Registers = null);
        }

        [Test]
        public void Can_set_PortsSpace_to_non_null_value()
        {
            var value = new Mock<IMemory>().Object;
            Sut.PortsSpace = value;
            Assert.That(Sut.PortsSpace, Is.EqualTo(value));
        }

        [Test]
        public void Cannot_set_PortsSpace_to_null()
        {
            Assert.Throws<ArgumentNullException>(() => Sut.PortsSpace = null);
        }

        [Test]
        public void Can_set_InstructionExecutor_to_non_null_value()
        {
            var value = new Mock<IZ80InstructionExecutor>().Object;
            Sut.InstructionExecutor = value;
            Assert.That(Sut.InstructionExecutor, Is.EqualTo(value));
        }

        [Test]
        public void Cannot_set_InstructionExecutor_to_null()
        {
            Assert.Throws<ArgumentNullException>(() => Sut.InstructionExecutor = null);
        }

        [Test]
        public void Sets_InstructionExecutor_agent_to_self()
        {
            var mock = new Mock<IZ80InstructionExecutor>();
            Sut.InstructionExecutor = mock.Object;

            mock.VerifySet(m => m.ProcessorAgent = Sut);
        }

        [Test]
        public void Can_set_ClockSynchronizationHelper_to_non_null_value()
        {
            var value = new Mock<IClockSynchronizer>().Object;
            Sut.ClockSynchronizer = value;
            Assert.That(Sut.ClockSynchronizer, Is.EqualTo(value));
        }

        [Test]
        public void Can_set_ClockSynchronizationHelper_to_null()
        {
            Sut.ClockSynchronizer = null;
        }

        [Test]
        public void Sets_ClockSynchronizationHelper_clockSpeed_to_processor_speed_by_speed_factor()
        {
            var mock = new Mock<IClockSynchronizer>();
            Sut.ClockFrequencyInMHz = 2;
            Sut.ClockSpeedFactor = 3;
            Sut.ClockSynchronizer = mock.Object;

            mock.VerifySet(m => m.EffectiveClockFrequencyInMHz = 2 * 3);
        }

        [Test]
        public void Can_set_clock_speed_and_clock_factor_combination_up_to_100_MHz()
        {
            Sut.ClockFrequencyInMHz = 20;
            Sut.ClockSpeedFactor = 5;

            Assert.Multiple(() =>
            {
                Assert.That(Sut.ClockFrequencyInMHz, Is.EqualTo(20));
                Assert.That(Sut.ClockSpeedFactor, Is.EqualTo(5));
            });
        }

        [Test]
        public void Cannot_set_clock_speed_and_clock_factor_combination_over_100_MHz()
        {
            Sut.ClockFrequencyInMHz = 1;
            Sut.ClockSpeedFactor = 1;

            Assert.Throws<ArgumentException>(() =>
            {
                Sut.ClockFrequencyInMHz = 1;
                Sut.ClockSpeedFactor = 101;
            });

            Assert.Throws<ArgumentException>(() =>
            {
                Sut.ClockSpeedFactor = 1;
                Sut.ClockFrequencyInMHz = 101;
            });
        }

        [Test]
        public void Can_set_clock_speed_and_clock_factor_combination_down_to_1_KHz()
        {
            Sut.ClockFrequencyInMHz = 0.1M;
            Sut.ClockSpeedFactor = 0.01M;

            Assert.Multiple(() =>
            {
                Assert.That(Sut.ClockFrequencyInMHz, Is.EqualTo(0.1M));
                Assert.That(Sut.ClockSpeedFactor, Is.EqualTo(0.01M));
            });
        }

        [Test]
        public void Cannot_set_clock_speed_and_clock_factor_combination_under_1_KHz()
        {
            Sut.ClockFrequencyInMHz = 1;
            Sut.ClockSpeedFactor = 1;

            Assert.Throws<ArgumentException>(() =>
            {
                Sut.ClockFrequencyInMHz = 1;
                Sut.ClockSpeedFactor = 0.0009M;
            });

            Assert.Throws<ArgumentException>(() =>
            {
                Sut.ClockSpeedFactor = 0.0009M;
                Sut.ClockFrequencyInMHz = 1;
            });
        }
    }
}
