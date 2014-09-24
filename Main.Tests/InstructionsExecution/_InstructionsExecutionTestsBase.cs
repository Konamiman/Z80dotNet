using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public abstract class InstructionsExecutionTestsBase
    {
        protected Z80InstructionExecutor Sut { get; set; }
        protected FakeProcessorAgent ProcessorAgent { get; set; }
        protected IZ80Registers Registers { get; set; }
        protected Fixture Fixture { get; set; }

        [SetUp]
        public void Setup()
        {
            Sut = new Z80InstructionExecutor();
            Sut.ProcessorAgent = ProcessorAgent = new FakeProcessorAgent();
            Registers = ProcessorAgent.Registers;
            Sut.InstructionFetchFinished += (s, e) => { };

            Fixture = new Fixture();
        }

        #region Auxiliary methods

        protected int nextFetchesAddress;

        protected void SetPortValue(byte portNumber, byte value)
        {
            ProcessorAgent.Ports[portNumber] = value;
        }

        protected byte GetPortValue(byte portNumber)
        {
            return ProcessorAgent.Ports[portNumber];
        }

        protected void SetMemoryContents(params byte[] opcodes)
        {
            SetMemoryContentsAt(0, opcodes);
        }
        
        protected void SetMemoryContentsAt(ushort address, params byte[] opcodes)
        {
            Array.Copy(opcodes, 0, ProcessorAgent.Memory, address, opcodes.Length);
            nextFetchesAddress = address + opcodes.Length;
        }

        protected void ContinueSettingMemoryContents(params byte[] opcodes)
        {
			Array.Copy(opcodes, 0, ProcessorAgent.Memory, nextFetchesAddress, opcodes.Length);
            nextFetchesAddress += opcodes.Length;
        }

		protected FakeInstructionExecutor NewFakeInstructionExecutor()
        {
			var sut = new FakeInstructionExecutor();
            sut.ProcessorAgent = ProcessorAgent = new FakeProcessorAgent();
		    Registers = ProcessorAgent.Registers;
			sut.InstructionFetchFinished += (s, e) => { };
            return sut;
        }

        protected T GetReg<T>(string name)
        {
            return (T)RegProperty(name).GetValue(Registers, null); 
        }

        protected Bit GetFlag(string name)
        {
            return (Bit)RegProperty(name + "F").GetValue(Registers, null); 
        }

        protected void SetReg(string regName, byte value)
        {
            RegProperty(regName).SetValue(Registers, value, null);
        }

        protected void SetReg(string regName, short value)
        {
            RegProperty(regName).SetValue(Registers, value, null);
        }

        protected void SetFlag(string flagName, Bit value)
        {
            RegProperty(flagName + "F").SetValue(Registers, value, null);
        }

        protected PropertyInfo RegProperty(string name)
        {
            return typeof(Z80Registers).GetProperty(name);
        }

        protected int Execute(byte opcode, byte? prefix = null, params byte[] nextFetches)
        {
            return ExecuteAt(0, opcode, prefix, nextFetches);
        }

        protected int ExecuteAt(ushort address, byte opcode, byte? prefix = null, params byte[] nextFetches)
        {
            Registers.PC = address.Inc();   //Inc needed to simulate the first fetch made by the enclosing IZ80Processor
            if (prefix == null)
            {
                SetMemoryContentsAt(address.Inc(), nextFetches);
                return Sut.Execute(opcode);
            }
            else
            {
                SetMemoryContentsAt(address.Inc(), opcode);
                ContinueSettingMemoryContents(nextFetches);

                return Sut.Execute(prefix.Value);
            }

        }

        protected object IfIndexRegister(string regName, object value, object @else)
        {
            return regName.StartsWith("IX") || regName.StartsWith("IY") ? value : @else;
        }

        protected void AssertNoFlagsAreModified(byte opcode, byte? prefix = null)
        {
            var value = Fixture.Create<byte>();
            Registers.F = value;
            Execute(opcode, prefix);

            Assert.AreEqual(value, Registers.F);
        }

        protected void AssertSetsFlags(byte opcode, byte? prefix = null, params string[] flagNames)
        {
            AssertSetsOrResetsFlags(opcode, 1, prefix, flagNames);
        }

        protected void AssertResetsFlags(byte opcode, byte? prefix = null, params string[] flagNames)
        {
            AssertSetsOrResetsFlags(opcode, 0, prefix, flagNames);
        }

        protected void AssertSetsOrResetsFlags(byte opcode, Bit expected, byte? prefix = null, params string[] flagNames)
        {
            var randomValues = Fixture.Create<byte[]>();

            foreach (var value in randomValues)
            {
                foreach (var flag in flagNames)
                    SetFlag(flag, !expected);

                Registers.A = value;

                Execute(opcode, prefix);

                foreach (var flag in flagNames)
                    Assert.AreEqual(expected, GetFlag(flag));
            }
        }

        protected void AssertDoesNotChangeFlags(byte opcode, byte? prefix = null, params string[] flagNames)
        {
            var randomFlags = flagNames.ToDictionary(keySelector: x => x, elementSelector: x => Fixture.Create<Bit>());

            foreach(var flag in flagNames)
                SetFlag(flag, randomFlags[flag]);

            for(var i = 0; i <= Fixture.Create<byte>(); i++)
            {
                Execute(opcode, prefix);

                foreach(var flag in flagNames)
                    Assert.AreEqual(randomFlags[flag], GetFlag(flag));
            }
        }

        protected void WriteShortToMemory(ushort address, short value)
        {
            ProcessorAgent.Memory[address] = value.GetLowByte();
            ProcessorAgent.Memory[address + 1] = value.GetHighByte();
        }

        protected short ReadShortFromMemory(ushort address)
        {
            return NumberUtils.CreateShort(ProcessorAgent.Memory[address], ProcessorAgent.Memory[address + 1]);
        }

        #endregion

        #region ParityTable

        protected byte[] Parity = new byte[] { 
	        1, 0, 0, 1, 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 0, 1,
	        0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 1, 0, 
	        0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 1, 0, 
	        1, 0, 0, 1, 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 0, 1, 
	        0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 1, 0, 
	        1, 0, 0, 1, 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 0, 1, 
	        1, 0, 0, 1, 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 0, 1, 
	        0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 1, 0, 
	        0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 1, 0, 
	        1, 0, 0, 1, 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 0, 1, 
	        1, 0, 0, 1, 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 0, 1, 
	        0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 1, 0, 
	        1, 0, 0, 1, 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 0, 1, 
	        0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 1, 0, 
	        0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 1, 0, 
	        1, 0, 0, 1, 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 0, 1 };
        #endregion

        #region Fake classes

        protected class FakeInstructionExecutor : Z80InstructionExecutor
		{
		    public List<byte> UnsupportedExecuted = new List<byte>();

		    protected override int ExecuteUnsopported_ED_Instruction(byte secondOpcodeByte)
		    {
				UnsupportedExecuted.Add(secondOpcodeByte);
		        return base.ExecuteUnsopported_ED_Instruction(secondOpcodeByte);
		    }
        }

        protected class FakeProcessorAgent : IZ80ProcessorAgent
        {
            public FakeProcessorAgent()
            {
                Registers = new Z80Registers();
                Memory = new byte[65536];
                Ports = new byte[256];
            }

            public byte[] Memory { get; set; }

            public byte[] Ports { get; set; }

            public ushort MemoryPointer { get; set; }

            public byte FetchNextOpcode()
            {
                return Memory[Registers.PC++];
            }

            public byte PeekNextOpcode()
            {
                return Memory[Registers.PC];
            }

            public byte ReadFromMemory(ushort address)
            {
                return Memory[address];
            }

            public void WriteToMemory(ushort address, byte value)
            {
                Memory[address] = value;
            }

            public byte ReadFromPort(byte portNumber)
            {
                return Ports[portNumber];
            }

            public void WriteToPort(byte portNumber, byte value)
            {
                Ports[portNumber] = value;
            }

            public IZ80Registers Registers { get; set; }

            public void SetInterruptMode(byte interruptMode)
            {
                throw new NotImplementedException();
            }

            public void Stop(bool isPause = false)
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}