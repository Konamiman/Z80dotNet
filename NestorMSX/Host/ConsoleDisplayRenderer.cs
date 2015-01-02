using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using Konamiman.NestorMSX.Hardware;

namespace Konamiman.NestorMSX.Host
{
    public class ConsoleDisplayRenderer : ITms9918DisplayRenderer
    {
        private int screenWidth = 32;
        private bool screenIsActive = false;
        private Dictionary<Point, string> screenBuffer;
        private int currentScreenMode = 0;

        public ConsoleDisplayRenderer()
        {
            Console.Title = "NestorMSX";

            screenBuffer = new Dictionary<Point, string>();
            Console.Clear();
           
            Console.WindowHeight = 25;
            Console.BufferHeight = 25;
        }

        void SetScreenWidth(int width)
        {
            screenWidth = width;
            Console.WindowWidth = width;
            Console.BufferWidth = width;
            var itemsToRemove = screenBuffer.Keys.Where(x => x.X >= width).ToArray();
            for(int i=0; i<itemsToRemove.Length; i++)
                screenBuffer.Remove(itemsToRemove[i]);
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
            if(mode > 1)
                return;

            Debug.WriteLine("*** Mode " + mode);
            currentScreenMode = mode;
            SetScreenWidth(mode == 1 ? 40 : 32);
        }

        public void WriteToNameTable(int position, byte value)
        {
            var theChar = value==255 ? "" : Encoding.ASCII.GetString(new[] {value});
            Debug.Write(theChar);
            var coordinates = new Point(position%screenWidth, position/screenWidth);
            if(coordinates.X >= screenWidth || coordinates.Y >= 24)
                return;

            screenBuffer[coordinates] = theChar;

            if(screenIsActive)
                PrintChar(coordinates, theChar);
        }

        private void PrintChar(Point coordinates, string theChar)
        {
            Console.SetCursorPosition(coordinates.X, coordinates.Y);
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
