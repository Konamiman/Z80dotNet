using System;
using System.Collections.Generic;
using System.Reflection;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public partial class Z80InstructionsExecutor
    {
        #region Auxiliary methods

        private void SetNextFetches(params byte[] opcodes)
        {
			ProcessorAgent.Memory = opcodes;
		    Registers.PC = 0;
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
                return Registers.Main;
            else
                return Registers;
        }

        void Execute(byte opcode, byte? prefix = null)
        {
            if (prefix == null)
            {
                Sut.Execute(opcode);
            }
            else
            {
                SetNextFetches(opcode);
                Sut.Execute(prefix.Value);
            }

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