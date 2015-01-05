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
        protected int colorTableSize = 32;

        private int charWidth = 8;
        private int currentScreenMode = 0;
        private int screenWidthInCharacters = 32;
        private bool screenIsActive = false;
        private Dictionary<Point, byte> screenBuffer = new Dictionary<Point, byte>();
        private Tuple<Color, Color>[] colorTable; 
        protected Color[] Colors { get; private set; }
        protected Color BackgroundColor { get; private set; }
        protected Color ForegroundColor { get; private set; }

        protected Dictionary<byte, byte[]> characterPatterns = new Dictionary<byte, byte[]>();
        protected abstract void ClearScreen(Color background);
        protected abstract void PrintChar(Point coordinates, Color foreground, Color background, byte theChar, int charWidth);
        
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
                Colors[i] = Color.FromArgb(255, int.Parse(tokens[0]), int.Parse(tokens[1]), int.Parse(tokens[2]));
            }

            ForegroundColor = Colors[0];
            BackgroundColor = Colors[0];

            colorTable = new Tuple<Color, Color>[colorTableSize];
            for(int i = 0; i < colorTableSize; i++) {
                colorTable[i] = new Tuple<Color, Color>(Color.Transparent, Color.Transparent);
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
                DoPrintChar(position, screenBuffer[position], charWidth);
        }

        public void BlankScreen()
        {
            Debug.WriteLine("*** Blank");
            screenIsActive = false;
            ClearScreen(BackgroundColor);
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
            Debug.Write(value==255 ? "" : Encoding.ASCII.GetString(new[] {value}));
            var coordinates = new Point(position%screenWidthInCharacters, position/screenWidthInCharacters);
            if(coordinates.X >= screenWidthInCharacters || coordinates.Y >= 24)
                return;

            screenBuffer[coordinates] = value;

            if(screenIsActive)
                DoPrintChar(coordinates, value, charWidth);
        }

        public void WriteToPatternGeneratorTable(int position, byte value)
        {
            var charIndex = position/8;
            var offset = position%8;
            characterPatterns[(byte)charIndex][offset] = value;

            if(screenIsActive)
                foreach(var p in screenBuffer.Keys.Where(k => screenBuffer[k] == charIndex))
                    DoPrintChar(p, screenBuffer[p], charWidth);
        }

        public void WriteToColourTable(int position, byte value)
        {
            var foregroundColorIndex = value >> 4;
            var backgroundColorIndex = value & 0x0F;

            colorTable[position] = new Tuple<Color, Color>(
                foregroundColorIndex == 0 ? BackgroundColor : Colors[foregroundColorIndex],
                backgroundColorIndex == 0 ? BackgroundColor : Colors[backgroundColorIndex]);
            ReprintAll();
        }

        public void SetForegroundColor(byte colorIndex)
        {
            ForegroundColor = Colors[colorIndex];
            ReprintAll();
        }

        public void SetBackgroundColor(byte colorIndex)
        {
            BackgroundColor = Colors[colorIndex];
            ClearScreen(BackgroundColor);
            ReprintAll();
        }

        private void ReprintAll()
        {
            ClearScreen(BackgroundColor);
            if(screenIsActive)
                foreach(var position in screenBuffer.Keys)
                    DoPrintChar(position, screenBuffer[position], charWidth);
        }

        private void DoPrintChar(Point coordinates, byte theChar, int charWidth)
        {
            if(currentScreenMode == 1) {
                PrintChar(coordinates, ForegroundColor, BackgroundColor, theChar, charWidth);
            }
            else {
                var colorTableIndex = theChar/8;
                var colors = colorTable[colorTableIndex];
                PrintChar(coordinates, colors.Item1, colors.Item2, theChar, charWidth);
            }
        }
    }
}
