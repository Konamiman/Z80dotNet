namespace Konamiman.Z80dotNet
{
    /// <summary>
    /// Contains utility extension methods for classes implementing the <see cref="IZ80Processor" /> interface.
    /// </summary>
    public static class Z80ProcessorExtensions
    {
        /// <summary>
        /// Simulates the execution of a CALL instruction by pushing the current content of the PC register
        /// into the stack and setting it to the specified value.
        /// </summary>
        /// <param name="processor"></param>
        /// <param name="address">New value for the PC register</param>
        public static void ExecuteCall(this IZ80Processor processor, ushort address)
        {
            var registers = processor.Registers;
            var memory = processor.Memory;

            var oldAddress = (short)registers.PC;
			var sp = (ushort)(registers.SP - 1);
            memory[sp] = oldAddress.GetHighByte();
			sp--;
            memory[sp] = oldAddress.GetLowByte();

			registers.SP = (short)sp;
			registers.PC = address;
        }

        /// <summary>
        /// Simulates the execution of a RET instruction by setting the value of the PC register
        /// from the value popped from the stack.
        /// </summary>
        /// <param name="processor"></param>
        public static void ExecuteRet(this IZ80Processor processor)
        {
            var registers = processor.Registers;
            var memory = processor.Memory;

            var sp = (ushort)registers.SP;
            var newPC = NumberUtils.CreateShort(memory[sp], memory[sp + 1]);
            
            registers.PC = (ushort)newPC;
            registers.SP += 2;
        }
    }
}
