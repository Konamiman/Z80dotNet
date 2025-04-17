using System;

namespace Konamiman.Z80dotNet
{
    /// <summary>
    /// Complements <see cref="IZ80Processor"/> by adding events related to interrupts servicing. 
    /// </summary>
    public interface IZ80ProcessorInterruptEvents
    {
        /// <summary>
        /// Triggered when a maskable interrupt is about to be serviced.
        /// 
        /// For IM 0: The opcode has been already fetched from the data bus and is about to be executed.
        /// 
        /// For IM 1: PC is already set to 0x0038 and the return address has been pushed to the stack.
        /// 
        /// For IM 2: PC is already set to the address of the routine to execute and the return address has been pushed to the stack.
        /// </summary>
        event EventHandler MaskableInterruptServicingStart;

        /// <summary>
        /// Triggered when a non-maskable interrupt is about to be serviced.
        /// PC is already set to 0x0066 and the return address has been pushed to the stack
        /// when this event is invoked.
        /// </summary>
        event EventHandler NonMaskableInterruptServicingStart;

        /// <summary>
        /// Triggered before a RETI instruction is about to be executed,
        /// right after the corresponding BeforeInstructionExecution event
        /// </summary>
        event EventHandler BeforeRetiInstructionExecution;

        /// <summary>
        /// Triggered after a RETI instruction has been executed,
        /// right after the corresponding AfterInstructionExecution event
        /// </summary>
        event EventHandler AfterRetiInstructionExecution;

        /// <summary>
        /// Triggered before a RETN instruction is about to be executed,
        /// right after the corresponding BeforeInstructionExecution event
        /// </summary>
        event EventHandler BeforeRetnInstructionExecution;

        /// <summary>
        /// Triggered after a RETN instruction has been executed,
        /// right after the corresponding AfterInstructionExecution event
        /// </summary>
        event EventHandler AfterRetnInstructionExecution;
    }
}
