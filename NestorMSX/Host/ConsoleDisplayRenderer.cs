using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            Console.Title = "NestorMSX";

            screenBuffer = new Dictionary<int, string>();
            Console.Clear();
           
            Console.WindowHeight = 25;
            Console.BufferHeight = 25;
        }

        void SetScreenWidth(int width)
        {
            screenWidth = width;
            Console.WindowWidth = width;
            Console.BufferWidth = width;
        }

        public void ActivateScreen()
        {
            Debug.WriteLine("*** Activate");
            screenIsActive = true;
            foreach(var position in screenBuffer.Keys)
                PrintChar(position, screenBuffer[position]);
        }

        public void BlankScreen()
        {
            Debug.WriteLine("*** Blank");
            screenIsActive = false;
            Console.Clear();
        }

        public void SetScreenMode(byte mode)
        {
            Debug.WriteLine("*** Mode " + mode);
            screenBuffer.Clear();
            SetScreenWidth(mode == 1 ? 40 : 32);
        }

        public void WriteToNameTable(int position, byte value)
        {
            var theChar = value==255 ? "" : Encoding.ASCII.GetString(new[] {value});
            Debug.Write(theChar);
            screenBuffer[position] = theChar;

            if(screenIsActive)
                PrintChar(position, theChar);
        }

        private void PrintChar(int position, string theChar)
        {
            int y = position/screenWidth;
            int x = position%screenWidth;
            Console.SetCursorPosition(x, y);
            if(theChar != "")
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
