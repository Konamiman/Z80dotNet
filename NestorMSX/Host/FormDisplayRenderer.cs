using System.Drawing;
using NestorMSX;

namespace Konamiman.NestorMSX.Host
{
    public class FormDisplayRenderer : DisplayRendererBase
    {
        private readonly Form1 form;
        private readonly Color[] Colors;

        public FormDisplayRenderer(Form1 form)
        {
            this.form = form;
        }

        protected override void ClearScreen()
        {
            form.ClearScreen();
        }

        protected override void PrintChar(Point coordinates, byte theChar, int charWidth)
        {
            form.PrintChar(coordinates, characterPatterns[theChar], charWidth);
        }

        protected override void SetForegroundColor(Color color)
        {
            form.SetForegroundColor(color);
        }

        protected override void SetBackgroundColor(Color color)
        {
            form.SetBackgroundColor(color);
        }
    }
}
