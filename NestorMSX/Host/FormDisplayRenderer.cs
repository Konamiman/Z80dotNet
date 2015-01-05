using System.Drawing;
using NestorMSX;

namespace Konamiman.NestorMSX.Host
{
    public class FormDisplayRenderer : DisplayRendererBase
    {
        private readonly Form1 form;

        public FormDisplayRenderer(Form1 form)
        {
            this.form = form;
            form.RegisterColors(Colors);
        }

        protected override void PrintChar(Point coordinates, Color foreground, Color background, byte theChar, int charWidth)
        {
            form.PrintChar(coordinates, foreground, background, characterPatterns[theChar], charWidth);
        }

        protected override void ClearScreen(Color background)
        {
            form.ClearScreen(background);
        }
    }
}
