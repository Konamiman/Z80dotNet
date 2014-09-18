using System;
using System.Collections.Generic;
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

        protected int nextFetchesLength;

        protected void SetMemoryContents(params byte[] opcodes)
        {
			Array.Copy(opcodes, ProcessorAgent.Memory, opcodes.Length);
		    Registers.PC = 0;
            nextFetchesLength = opcodes.Length;
        }

        protected void ContinueSettingMemoryContents(params byte[] opcodes)
        {
			Array.Copy(opcodes, 0, ProcessorAgent.Memory, nextFetchesLength, opcodes.Length);
            nextFetchesLength += opcodes.Length;
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

        protected void SetReg(string regName, byte value)
        {
            RegProperty(regName).SetValue(Registers, value, null);
        }

        protected void SetReg(string regName, short value)
        {
            RegProperty(regName).SetValue(Registers, value, null);
        }

        protected PropertyInfo RegProperty(string name)
        {
            return
                typeof(Z80Registers).GetProperty(name);
        }

        protected int Execute(byte opcode, byte? prefix = null, params byte[] nextFetches)
        {
            if (prefix == null)
            {
                SetMemoryContents(nextFetches);
                return Sut.Execute(opcode);
            }
            else
            {
                SetMemoryContents(opcode);
                ContinueSettingMemoryContents(nextFetches);

                return Sut.Execute(prefix.Value);
            }

        }

        protected object IfIndexRegister(string regName, object value, object @else)
        {
            return regName.StartsWith("IX") || regName.StartsWith("IY") ? value : @else;
        }

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
            }

            public byte[] Memory { get; set; }

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
                throw new NotImplementedException();
            }

            public void WriteToPort(byte portNumber, byte value)
            {
                throw new NotImplementedException();
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