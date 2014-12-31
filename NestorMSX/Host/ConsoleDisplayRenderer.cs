using System;
using System.Collections.Generic;
using System.Text;
using Konamiman.NestorMSX.Hardware;

namespace Konamiman.NestorMSX.Host
{
    public class ConsoleDisplayRenderer : ITms9918DisplayRenderer
    {
        private int screenWidth = 32;
        private bool screenIsActive = false;
        private Dictionary<int, string> screenBuffer;

        public ConsoleDisplayRenderer()
        {
            screenBuffer = new Dictionary<int, string>();
            Console.Clear();
           
            Console.WindowHeight = 24;
            Console.BufferHeight = 24;
        }

        void SetScreenWidth(int width)
        {
            screenWidth = width;
            Console.WindowWidth = width;
            Console.BufferWidth = width;
        }

        public void ActivateScreen()
        {
            screenIsActive = true;
            foreach(var position in screenBuffer.Keys)
                PrintChar(position, screenBuffer[position]);

            //Console.WriteLine("*** Activate screen");
        }

        public void BlankScreen()
        {
            screenIsActive = false;
            Console.Clear();
            //Console.WriteLine("*** Blank screen");
        }

        public void SetScreenMode(byte mode)
        {
            SetScreenWidth(mode == 1 ? 40 : 32);
            //Console.WriteLine("*** Set screen mode: " + mode);
        }

        public void WriteToNameTable(int position, byte value)
        {
            var theChar = Encoding.ASCII.GetString(new[] {value});
            screenBuffer[position] = theChar;

            if(screenIsActive)
                PrintChar(position, theChar);
        }

        private void PrintChar(int position, string theChar)
        {
            int y = position/screenWidth;
            int x = position%screenWidth;
            Console.SetCursorPosition(x, y);
            Console.Write(theChar);
        }

        public void WriteToPatternGeneratorTable(int position, byte value)
        {
            //Console.WriteLine("*** Write to pattern table: {0}, {1}", position, value);
        }

        public void SetForegroundColor(byte colorIndex)
        {
            //Console.WriteLine("*** Set foreground color: " + colorIndex);
        }

        public void SetBackgroundColor(byte colorIndex)
        {
            //Console.WriteLine("*** Set background color: " + colorIndex);
        }
    }
}
