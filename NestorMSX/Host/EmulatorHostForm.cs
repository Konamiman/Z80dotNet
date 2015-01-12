using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Konamiman.NestorMSX.Exceptions;
using Konamiman.NestorMSX.Hardware;
using Konamiman.NestorMSX.Misc;
using Konamiman.Z80dotNet;
using KeyEventArgs = Konamiman.NestorMSX.Hardware.KeyEventArgs;

namespace Konamiman.NestorMSX.Host
{
    /// <summary>
    /// The form that displays the emulated screen.
    /// </summary>
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
        private readonly Keys CopyKey;
        private readonly Keys PasteKey;
        private readonly Encoding EncodingForCopyPaste;

        public IExternallyControlledTms9918 Vdp { get; set; }

        #region Initialization

        public EmulatorHostForm() : this(null, null)
        {
        }

        public EmulatorHostForm(IZ80Processor processor, Configuration config)
        {
            InitializeComponent();
            if(config == null)
                return;

            ValidateConfiguration(config);
            
            this.processor = processor;
            this.Vdp = null;
            var width = (int)(((32*8) + config.HorizontalMarginInPixels*2)*config.DisplayZoomLevel);
            var height = (int)(((24*8) + config.VerticalMarginInPixels*2)*config.DisplayZoomLevel);
            ClientSize = new Size(width, height);
            canvas.Paint += CanvasOnPaint;
            
            this.processor = processor;
            if(processor != null)
                processor.BeforeInstructionFetch += ProcessorOnBeforeInstructionFetch;

            CopyKey = (Keys)Enum.Parse(typeof(Keys), config.CopyKey);
            PasteKey = (Keys)Enum.Parse(typeof(Keys), config.PasteKey);

            try {
                EncodingForCopyPaste = Encoding.GetEncoding(config.EncodingForCopyAndPaste);
            }
            catch(Exception ex) {
                throw new EmulationEnvironmentCreationException(
                    "Your system does not support the text encoding named '{0}'. Please edit the configuration file and enter a suitable value for the 'EncodingForCopyAndPaste' key. If in doubt, just specify 'ASCII'."
                        .FormatWith(config.EncodingForCopyAndPaste)
                    ,ex);
            }
        }

        private static void ValidateConfiguration(Configuration config)
        {
            if(config.HorizontalMarginInPixels < 0 || config.HorizontalMarginInPixels > 1000) {
                throw new ConfigurationException(
                    "Horizontal margin for display area must be an integer number between 0 and 1000");
            }

            if(config.VerticalMarginInPixels < 0 || config.VerticalMarginInPixels > 1000) {
                throw new ConfigurationException(
                    "Vertical margin for display area must be an integer number between 0 and 1000");
            }

            if(!config.CopyKey.IsValidKeyName())
                throw new ConfigurationException(
                    "The value for the Copy key is invalid. It must be a member of the .NET's System.Windows.Forms enumeration.");

            if(!config.PasteKey.IsValidKeyName())
                throw new ConfigurationException(
                    "The value for the Paste key is invalid. It must be a member of the .NET's System.Windows.Forms enumeration.");
        }

        #endregion

        #region IDrawingSurface

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

        #endregion

        #region Keyboard management

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

                    if(key == CopyKey && Vdp != null) {
                        CopyScreenAsText();
                    }
                    
                    if(key == PasteKey) {
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

        #endregion

        #region Copy & Paste

        private void PasteTextAsKeyboardData()
        {
            var text = Clipboard.GetText();
            PastedText.AddRange(EncodingForCopyPaste.GetBytes(text).Where(b => b != LF).ToArray());
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
                        .ToArray();
                sb.AppendLine(EncodingForCopyPaste.GetString(lineBytes));
            }

            Clipboard.SetText(sb.ToString().TrimEnd());
        }

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

        #endregion
    }
}
