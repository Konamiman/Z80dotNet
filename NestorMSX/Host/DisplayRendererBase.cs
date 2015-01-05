using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Konamiman.NestorMSX.Hardware;

namespace Konamiman.NestorMSX.Host
{
    public abstract class DisplayRendererBase : ITms9918DisplayRenderer
    {
        protected int characterWidthInPixels = 8;

        private int charWidth = 8;
        private int currentScreenMode = 0;
        private int screenWidthInCharacters = 32;
        private bool screenIsActive = false;
        private Dictionary<Point, byte> screenBuffer = new Dictionary<Point, byte>();
        private Color[] Colors;

        protected Dictionary<byte, byte[]> characterPatterns = new Dictionary<byte, byte[]>();
        protected abstract void ClearScreen();
        protected abstract void PrintChar(Point coordinates, byte theChar, int charWidth);
        protected abstract void SetForegroundColor(Color color);
        protected abstract void SetBackgroundColor(Color color);
        
        public DisplayRendererBase()
        {
            for(int i=0; i<256; i++)
                characterPatterns[(byte)i] = new byte[8];

            var colorsLines = File.ReadAllLines("Colors.txt");
            Colors = new Color[16];
            for(int i=0; i<16; i++)
            {
                var line = colorsLines[i];
                var tokens = line.Split(new[] {" ", "\t"}, StringSplitOptions.RemoveEmptyEntries);
                Colors[i] = Color.FromArgb(int.Parse(tokens[0]), int.Parse(tokens[1]), int.Parse(tokens[2]));
            }
        }

        protected virtual void SetScreenWidth(int width)
        {
            screenWidthInCharacters = width;
            var itemsToRemove = screenBuffer.Keys.Where(x => x.X >= width).ToArray();
            for(int i=0; i<itemsToRemove.Length; i++)
                screenBuffer.Remove(itemsToRemove[i]);
        }

        public void ActivateScreen()
        {
            Debug.WriteLine("*** Activate");
            screenIsActive = true;
            foreach(var position in screenBuffer.Keys)
                PrintChar(position, screenBuffer[position], charWidth);
        }

        public void BlankScreen()
        {
            Debug.WriteLine("*** Blank");
            screenIsActive = false;
            ClearScreen();
        }

        public void SetScreenMode(byte mode)
        {
            if(mode > 1)
                return;

            Debug.WriteLine("*** Mode " + mode);
            currentScreenMode = mode;
            SetScreenWidth(mode == 1 ? 40 : 32);
            charWidth = (mode == 1 ? 6 : 8);
        }

        public void WriteToNameTable(int position, byte value)
        {
            var theChar = value==255 ? "" : Encoding.ASCII.GetString(new[] {value});
            Debug.Write(theChar);
            var coordinates = new Point(position%screenWidthInCharacters, position/screenWidthInCharacters);
            if(coordinates.X >= screenWidthInCharacters || coordinates.Y >= 24)
                return;

            screenBuffer[coordinates] = value;

            if(screenIsActive)
                PrintChar(coordinates, value, charWidth);
        }

        public void WriteToPatternGeneratorTable(int position, byte value)
        {
            var charIndex = position/8;
            var offset = position%8;
            characterPatterns[(byte)charIndex][offset] = value;

            if(screenIsActive)
                foreach(var p in screenBuffer.Keys.Where(k => screenBuffer[k] == charIndex))
                    PrintChar(p, screenBuffer[p], charWidth);
        }

        public void SetForegroundColor(byte colorIndex)
        {
            SetForegroundColor(Colors[colorIndex]);
            ReprintAll();
        }

        public void SetBackgroundColor(byte colorIndex)
        {
            SetBackgroundColor(Colors[colorIndex]);
            ReprintAll();
        }

        private void ReprintAll()
        {
            if(screenIsActive)
                foreach(var position in screenBuffer.Keys)
                    PrintChar(position, screenBuffer[position], charWidth);
        }
    }
}
