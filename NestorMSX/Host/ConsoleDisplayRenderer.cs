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

            ClearScreen();
           
            Console.WindowHeight = 25;
            Console.BufferHeight = 25;
        }

        protected override void SetScreenWidth(int width)
        {
            base.SetScreenWidth(width);
            Console.WindowWidth = width;
            Console.BufferWidth = width;
        }

        protected override void SetForegroundColor(Color color)
        {
        }

        protected override void SetBackgroundColor(Color color)
        {
        }

        protected override void ClearScreen()
        {
            Console.Clear();
        }

        protected override void PrintChar(Point coordinates, byte theChar, int charWidth)
        {
            Console.SetCursorPosition(coordinates.X, coordinates.Y);
            if(theChar != 255)
                Console.Write(Convert.ToString(theChar));
        }
    }
}
