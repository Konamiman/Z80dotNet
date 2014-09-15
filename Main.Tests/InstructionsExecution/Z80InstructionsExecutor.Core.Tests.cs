using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class Z80InstructionsExecutor
    {
        private Z80InstructionExecutor Sut { get; set; }
        private Mock<IZ80ProcessorAgent> ProcessorAgent { get; set; }
        private IZ80Registers Registers { get; set; }

        [SetUp]
        public void Setup()
        {
            Sut = new Z80InstructionExecutor();
			ProcessorAgent = new Mock<IZ80ProcessorAgent>();
			Registers = new Z80Registers();
            ProcessorAgent.SetupGet(m => m.Registers).Returns(Registers);
            Sut.ProcessorAgent = ProcessorAgent.Object;
            Sut.InstructionFetchFinished += (s, e) => { };
        }

		[Test]
		public void Instructions_execution_fire_FetchFinished_event_and_return_proper_T_states_count()
		{
		    var fetchFinishedEventsCount = 0;

		    SetNextOpcode(0);

		    Sut.InstructionFetchFinished += (sender, e) => fetchFinishedEventsCount++;

			Assert.AreEqual(4, Sut.Execute(0x00));
			Assert.AreEqual(10, Sut.Execute(0x01));

			Assert.AreEqual(8, Sut.Execute(0xCB));

			SetNextOpcode(0x09);
			Assert.AreEqual(15, Sut.Execute(0xDD));
			Assert.AreEqual(15, Sut.Execute(0xFD));

			SetNextOpcode(0x40);
			Assert.AreEqual(12, Sut.Execute(0xED));

			Assert.AreEqual(6, fetchFinishedEventsCount);
        }

        [Test]
        public void Unsupported_instructions_just_return_8_TStates_elapsed()
        {
			var fetchFinishedEventsCount = 0;

			Sut.InstructionFetchFinished += (sender, e) => fetchFinishedEventsCount++;

			SetNextOpcode(0x3F);
			Assert.AreEqual(8, Sut.Execute(0xED));
			SetNextOpcode(0xC0);
			Assert.AreEqual(8, Sut.Execute(0xED));

			Assert.AreEqual(2, fetchFinishedEventsCount);
        }

		[Test]
        public void Unsupported_instructions_invoke_overridable_method_ExecuteUnsopported_ED_Instruction()
		{
		    var sut = NewFakeInstructionExecutor();

			SetNextOpcode(0x3F);
		    sut.Execute(0xED);
			SetNextOpcode(0xC0);
		    sut.Execute(0xED);

			Assert.AreEqual(new Byte[] {0x3F, 0xC0}, sut.UnsupportedExecuted);
        }

		FakeInstructionExecutor NewFakeInstructionExecutor()
        {
			var sut = new FakeInstructionExecutor();
			sut.ProcessorAgent = ProcessorAgent.Object;
			sut.InstructionFetchFinished += (s, e) => { };
            return sut;
        }

		private class FakeInstructionExecutor : Z80InstructionExecutor
		{
		    public List<byte> UnsupportedExecuted = new List<byte>();

		    protected override int ExecuteUnsopported_ED_Instruction(byte secondOpcodeByte)
		    {
				UnsupportedExecuted.Add(secondOpcodeByte);
		        return base.ExecuteUnsopported_ED_Instruction(secondOpcodeByte);
		    }
        }

		[Test]
		public void Execute_increases_R_appropriately()
        {
			SetNextOpcode(0);
		    Registers.R = 0xFE;

			Assert.AreEqual(4, Sut.Execute(0x00));
			Assert.AreEqual(0xFF, Registers.R);
			Assert.AreEqual(10, Sut.Execute(0x01));
			Assert.AreEqual(0x80, Registers.R);

			Assert.AreEqual(8, Sut.Execute(0xCB));
			Assert.AreEqual(0x82, Registers.R);

			SetNextOpcode(0x09);
			Assert.AreEqual(15, Sut.Execute(0xDD));
			Assert.AreEqual(15, Sut.Execute(0xFD));
			Assert.AreEqual(0x86, Registers.R);

			SetNextOpcode(0x40);
			Assert.AreEqual(12, Sut.Execute(0xED));
			Assert.AreEqual(0x88, Registers.R);
        }

		private void SetNextOpcode(byte opcode)
        {
			ProcessorAgent.Setup(m => m.PeekNextOpcode()).Returns(opcode);
			ProcessorAgent.Setup(m => m.FetchNextOpcode()).Returns(opcode);
        }
    }
}
