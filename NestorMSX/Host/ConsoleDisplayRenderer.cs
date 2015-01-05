using System;
using System.Collections.Generic;
using System.Drawing;

namespace Konamiman.NestorMSX.Host
{
    public class ConsoleDisplayRenderer : DisplayRendererBase
    {
        private int screenWidth = 32;
        private bool screenIsActive = false;
        private Dictionary<Point, string> screenBuffer;
        private int currentScreenMode = 0;

        public ConsoleDisplayRenderer()
        {
            Console.Title = "NestorMSX";

            ClearScreen(Color.Transparent);
           
            Console.WindowHeight = 25;
            Console.BufferHeight = 25;
        }

        protected override void ClearScreen(Color background)
        {
            Console.Clear();
        }

        protected override void PrintChar(Point coordinates, Color foreground, Color background, byte theChar, int charWidth)
        {
            Console.SetCursorPosition(coordinates.X, coordinates.Y);
            if(theChar != 255)
                Console.Write(Convert.ToString(theChar));
        }

        protected override void SetScreenWidth(int width)
        {
            base.SetScreenWidth(width);
            Console.WindowWidth = width;
            Console.BufferWidth = width;
        }
    }
}
