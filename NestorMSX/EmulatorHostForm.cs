using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Konamiman.NestorMSX.Hardware;
using Konamiman.NestorMSX.Host;
using KeyEventArgs = Konamiman.NestorMSX.Hardware.KeyEventArgs;

namespace NestorMSX
{
    public partial class EmulatorHostForm : Form, IKeyEventSource, IDrawingSurface
    {
        private readonly List<Keys> PressedKeys = new List<Keys>();

        public EmulatorHostForm()
        {
            InitializeComponent();
            canvas.Paint += CanvasOnPaint;
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

        public void Start()
        {
        }

        public void Stop()
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            ClientSize = new Size(((32*8) + 16) * 2, ((24*8) + 16) * 2);
            base.OnLoad(e);
        }

        public event EventHandler<KeyEventArgs> KeyPressed;
        public event EventHandler<KeyEventArgs> KeyReleased;

        protected override bool ProcessKeyMessage(ref Message m)
        {
            var key = (Keys)m.WParam;
            if(m.Msg == 0x100)
            {
                if(!PressedKeys.Contains(key))
                {
                    PressedKeys.Add(key);
                    if (KeyPressed != null)
                        KeyPressed(this, new KeyEventArgs(key));
                }
            }
            else if(m.Msg == 0x101)
            {
                if(PressedKeys.Contains(key))
                    PressedKeys.Remove(key);
                if(KeyReleased != null)
                    KeyReleased(this, new KeyEventArgs(key));
            }

            return base.ProcessKeyMessage(ref m);
        }
    }
}
