using System;
using System.Collections.Generic;
using System.Reflection;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public partial class Z80InstructionsExecutor
    {
        #region Auxiliary methods

        private int nextFetchesLength;

        private void SetMemoryContents(params byte[] opcodes)
        {
			Array.Copy(opcodes, ProcessorAgent.Memory, opcodes.Length);
		    Registers.PC = 0;
            nextFetchesLength = opcodes.Length;
        }

        private void ContinueSettingMemoryContents(params byte[] opcodes)
        {
			Array.Copy(opcodes, 0, ProcessorAgent.Memory, nextFetchesLength, opcodes.Length);
            nextFetchesLength += opcodes.Length;
        }

		FakeInstructionExecutor NewFakeInstructionExecutor()
        {
			var sut = new FakeInstructionExecutor();
            sut.ProcessorAgent = ProcessorAgent = new FakeProcessorAgent();
		    Registers = ProcessorAgent.Registers;
			sut.InstructionFetchFinished += (s, e) => { };
            return sut;
        }

        private T GetReg<T>(string name)
        {
            return (T)RegProperty(name).GetValue(GetTarget(name), null); 
        }

        private void SetReg(string regName, byte value)
        {
            RegProperty(regName).SetValue(GetTarget(regName), value, null);
        }

        private void SetReg(string regName, short value)
        {
            RegProperty(regName).SetValue(GetTarget(regName), value, null);
        }

        PropertyInfo RegProperty(string name)
        {
            return
                typeof(Z80Registers).GetProperty(name) ??
                typeof(MainZ80Registers).GetProperty(name);
        }

        object GetTarget(string regName)
        {
            if (typeof (Z80Registers).GetProperty(regName) == null)
                return Registers;
            else
                return Registers;
        }

        int Execute(byte opcode, byte? prefix = null, params byte[] nextFetches)
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

        object IfIndexRegister(string regName, object value, object @else)
        {
            return regName.StartsWith("IX") || regName.StartsWith("IY") ? value : @else;
        }

        #endregion

        #region Fake classes

        private class FakeInstructionExecutor : Z80InstructionExecutor
		{
		    public List<byte> UnsupportedExecuted = new List<byte>();

		    protected override int ExecuteUnsopported_ED_Instruction(byte secondOpcodeByte)
		    {
				UnsupportedExecuted.Add(secondOpcodeByte);
		        return base.ExecuteUnsopported_ED_Instruction(secondOpcodeByte);
		    }
        }

        private class FakeProcessorAgent : IZ80ProcessorAgent
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