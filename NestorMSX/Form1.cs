using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Konamiman.NestorMSX.Hardware;
using Konamiman.Z80dotNet;
using KeyEventArgs = Konamiman.NestorMSX.Hardware.KeyEventArgs;

namespace NestorMSX
{
    public partial class Form1 : Form, IKeyEventSource
    {
        private Graphics g = null;
        private Graphics baseGraphics = null;
        private readonly List<Keys> PressedKeys = new List<Keys>();
        private Brush blue = new SolidBrush(Color.FromArgb(84, 85, 237));
        private Brush foregroundColor = new SolidBrush(Color.White);
        private Brush backgroundColor = new SolidBrush(Color.Blue);

        public Form1()
        {
            InitializeComponent();
        }

        public void Start()
        {
        }

        public void Stop()
        {
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

        private void Form1_Load(object sender, EventArgs e)
        {
            this.ClientSize = new Size(((32*8) + 16) * 2, ((24*8) + 16) * 2);
            g = canvas.CreateGraphics();
            baseGraphics = canvas.CreateGraphics();
            g.ScaleTransform(2, 2);
            g.TranslateTransform(8, 8);
        }

        public void ClearScreen()
        {
            if(g != null)
            {
                var state = g.Save();
                g.ResetTransform();
                g.FillRectangle(backgroundColor, 0, 0, canvas.Width, canvas.Height);
                g.Restore(state);
            }
        }

        public void PrintChar(Point coordinates, byte[] pattern, int charWidth)
        {
            var baseX = (coordinates.X * charWidth) + (charWidth == 6 ? 8 : 0);
            var X = baseX;
            var Y = coordinates.Y * 8;
            g.FillRectangle(backgroundColor, baseX, Y, charWidth, 8);
            for(int i=0; i<8; i++) {
                for(int bit=7; bit>=8-charWidth; bit--) {
                    if(pattern[i].GetBit(bit)) {
                        g.FillRectangle(foregroundColor, X + (7-bit), Y, 1, 1);
                    }
                }
                X = baseX;
                Y++;
            }
        }

        public void SetForegroundColor(Color color)
        {
            if(foregroundColor != null)
                foregroundColor.Dispose();

            this.foregroundColor = new SolidBrush(color);
        }

        public void SetBackgroundColor(Color color)
        {
            if(backgroundColor != null)
                backgroundColor.Dispose();

            this.backgroundColor = new SolidBrush(color);
        }
    }
}
