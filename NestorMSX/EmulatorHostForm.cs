using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Konamiman.NestorMSX.Hardware;
using Konamiman.NestorMSX.Host;
using Konamiman.Z80dotNet;
using KeyEventArgs = Konamiman.NestorMSX.Hardware.KeyEventArgs;

namespace NestorMSX
{
    public partial class EmulatorHostForm : Form, IKeyEventSource, IDrawingSurface
    {
        private const int WM_KEYDOWN = 0x100;
        private const int WM_KEYUP = 0x101;

        private const int KEYBUF = 0xFB0F;
        private const int GETPNT = 0xF3FA;
        private const int PUTPNT = 0xF3F8;

        private const int LF = 10;

        private const int ScreenLines = 24;

        private readonly IZ80Processor processor;
        private readonly List<Keys> PressedKeys = new List<Keys>();
        private readonly List<byte> PastedText = new List<byte>();

        public EmulatorHostForm() : this(null)
        {
        }

        public EmulatorHostForm(IZ80Processor processor)
        {
            this.processor = processor;
            this.Vdp = null;
            InitializeComponent();
            ClientSize = new Size(((32*8) + 16) * 2, ((24*8) + 16) * 2);
            canvas.Paint += CanvasOnPaint;
            
            this.processor = processor;
            if(processor != null)
                processor.BeforeInstructionFetch += ProcessorOnBeforeInstructionFetch;
        }

        public IExternallyControlledTms9918 Vdp { get; set; }

        private void ProcessorOnBeforeInstructionFetch(object sender, BeforeInstructionFetchEventArgs eventArgs)
        {
            var count = Math.Min(PastedText.Count, 8);
            if(count > 0 && (processor.Memory[GETPNT] == processor.Memory[PUTPNT])) {
                processor.Memory.SetContents(KEYBUF, PastedText.Take(count).ToArray(), 0, count);
                WriteShort(GETPNT, KEYBUF);
                WriteShort(PUTPNT, KEYBUF + count);
                PastedText.RemoveRange(0, count);
            }
        }

        void WriteShort(ushort address, int value)
        {
            processor.Memory[address] = ((ushort)value).GetLowByte();
            processor.Memory[address+1] = ((ushort)value).GetHighByte();
        }

        public event EventHandler<PaintEventArgs> RequiresPaint;

        public Graphics GetGraphics()
        {
            return canvas.CreateGraphics();
        }

        private void CanvasOnPaint(object sender, PaintEventArgs paintEventArgs)
        {
            if(RequiresPaint != null)
                RequiresPaint(this, paintEventArgs);
        }

        public void StartGeneratingKeyEvents()
        {
        }

        public void StopGeneratingKeyEvents()
        {
        }

        public event EventHandler<KeyEventArgs> KeyPressed;
        public event EventHandler<KeyEventArgs> KeyReleased;

        protected override bool ProcessKeyMessage(ref Message m)
        {
            var key = (Keys)m.WParam;
            if(m.Msg == WM_KEYDOWN)
            {
                if(!PressedKeys.Contains(key))
                {
                    PressedKeys.Add(key);

                    if(key == Keys.F11 && Vdp != null) {
                        CopyScreenAsText();
                    }
                    
                    if(key == Keys.F12) {
                        PasteTextAsKeyboardData();
                    }

                    if (KeyPressed != null)
                        KeyPressed(this, new KeyEventArgs(key));
                }
            }
            else if(m.Msg == WM_KEYUP)
            {
                if(PressedKeys.Contains(key))
                    PressedKeys.Remove(key);
                if(KeyReleased != null)
                    KeyReleased(this, new KeyEventArgs(key));
            }

            return base.ProcessKeyMessage(ref m);
        }

        private void PasteTextAsKeyboardData()
        {
            var text = Clipboard.GetText();
            PastedText.AddRange(Encoding.ASCII.GetBytes(text).Where(b => b != LF).ToArray());
        }

        private void CopyScreenAsText()
        {
            var sb = new StringBuilder();
            var screenBytes = Vdp.GetVramContents(Vdp.PatternNameTableAddress, Vdp.PatternNameTableSize);
            var lineWidth = screenBytes.Length/ScreenLines;
            for(int i = 0; i < ScreenLines; i++) {
                var lineBytes =
                    screenBytes
                        .Skip(lineWidth*i)
                        .Take(lineWidth)
                        .Where(b => b >= 32 && b < 127)
                        .ToArray();
                sb.AppendLine(Encoding.ASCII.GetString(lineBytes));
            }

            Clipboard.SetText(sb.ToString().TrimEnd());
        }
    }
}
