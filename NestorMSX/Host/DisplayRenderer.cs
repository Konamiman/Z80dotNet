using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using Konamiman.NestorMSX.Exceptions;
using Konamiman.NestorMSX.Hardware;
using Konamiman.NestorMSX.Misc;

namespace Konamiman.NestorMSX.Host
{
    public class DisplayRenderer : ITms9918DisplayRenderer
    {
        private const int SCREEN_1 = 0;
        private const int SCREEN_0 = 1;

        private readonly ICharacterBasedDisplay display;

        private int charWidth = 8;
        private int currentScreenMode = SCREEN_1;
        private int screenWidthInCharacters = 32;
        private readonly Dictionary<Point, byte> screenBuffer = new Dictionary<Point, byte>();
        private Color[] Colors { get; set; }
        private Color BackdropColor { get; set; }
        private Color TextColor { get; set; }
        private Dictionary<byte, byte[]> characterPatterns = new Dictionary<byte, byte[]>();
        private readonly Dictionary<byte, Tuple<byte, byte>> CharacterColorsForScreen1 = new Dictionary<byte, Tuple<byte, byte>>();
        
        public DisplayRenderer(ICharacterBasedDisplay display, Configuration config)
        {
            this.display = display;
            display.SetScreenBufer(screenBuffer);

            for(int i = 0; i < 256; i++) {
                characterPatterns[(byte)i] = new byte[8];
                CharacterColorsForScreen1[(byte)i] = new Tuple<byte, byte>(0, 0);
            }

            var colorsLines = FileUtils.ReadAllLines(config.ColorsFile);
            try {
                Colors = ParseColorsFile(colorsLines);
            }
            catch(Exception ex) {
                ThrowParseColorsException(ex);
            }

            TextColor = Colors[0];
            BackdropColor = Colors[0];
        }

        private Color[] ParseColorsFile(string[] colorsLines)
        {
            var colors = new Color[16];
            for(int i = 0; i < 16; i++) {
                var line = colorsLines[i];
                var tokens = line.Split(new[] {" ", "\t"}, StringSplitOptions.RemoveEmptyEntries);
                colors[i] = Color.FromArgb(255, int.Parse(tokens[0]), int.Parse(tokens[1]), int.Parse(tokens[2]));
            }
            return colors;
        }

        protected virtual void SetScreenWidth(int width)
        {
            screenWidthInCharacters = width;
            var itemsToRemove = screenBuffer.Keys.Where(x => x.X >= width).ToArray();
            for(int i = 0; i < itemsToRemove.Length; i++) {
                screenBuffer.Remove(itemsToRemove[i]);
                display.NotifyScreenBufferContentsRemoved(itemsToRemove[i]);
            }
        }

        public void ActivateScreen()
        {
            display.ActivateScreen();
        }

        public void BlankScreen()
        {
            display.BlankScreen();
        }

        public void SetScreenMode(byte mode)
        {
            if(mode > 1)
                return;

            currentScreenMode = mode;
            SetScreenWidth(mode == SCREEN_0 ? 40 : 32);
            display.SetCharacterWidth(mode == SCREEN_0 ? 6 : 8);

            SetAllCharsColors();
        }

        public void WriteToNameTable(int position, byte value)
        {
            var coordinates = new Point(position%screenWidthInCharacters, position/screenWidthInCharacters);
            if(coordinates.X >= screenWidthInCharacters || coordinates.Y >= 24)
                return;

            screenBuffer[coordinates] = value;

            display.NotifyScreenBufferContentsAddedOrChanged(coordinates);
        }

        public void WriteToPatternGeneratorTable(int position, byte value)
        {
            var charIndex = (byte)(position/8);
            var pattern = characterPatterns[charIndex];
            var offset = position%8;
            pattern[offset] = value;

            display.SetCharacterPattern(charIndex, pattern);
        }

        public void WriteToColourTable(int position, byte value)
        {
            var foregroundColorIndex = value >> 4;
            var backgroundColorIndex = value & 0x0F;

            var firstCharIndex = (position*8);
            for(byte i = 0; i < 8; i++) {
                var charIndex = (byte)(firstCharIndex + i);
                CharacterColorsForScreen1[charIndex] = new Tuple<byte, byte>((byte)foregroundColorIndex, (byte)backgroundColorIndex);
                if(currentScreenMode == SCREEN_1)
                    display.SetCharacterColors((byte)(firstCharIndex + i), Colors[foregroundColorIndex], Colors[backgroundColorIndex]);
            }
        }

        public void SetTextColor(byte colorIndex)
        {
            TextColor = Colors[colorIndex];
            if(currentScreenMode == SCREEN_0)
                SetAllCharsColorsForScreen0();
        }

        private void SetAllCharsColors()
        {
            if(currentScreenMode == SCREEN_0)
                SetAllCharsColorsForScreen0();
            else
                SetAllCharsColorsForScreen1();
        }

        private void SetAllCharsColorsForScreen0()
        {
            for(int i = 0; i < 256; i++)
                display.SetCharacterColors((byte)i, TextColor, BackdropColor);
        }

        private void SetAllCharsColorsForScreen1()
        {
            for(int i = 0; i < 256; i++)
                display.SetCharacterColors((byte)i,
                    Colors[CharacterColorsForScreen1[(byte)i].Item1],
                    Colors[CharacterColorsForScreen1[(byte)i].Item2]);    
        }

        public void SetBackdropColor(byte colorIndex)
        {
            display.SetBackdropColor(Colors[colorIndex]);
            BackdropColor = Colors[colorIndex];
            if(currentScreenMode == SCREEN_0)
                SetAllCharsColorsForScreen0();
        }

        
        private void ThrowParseColorsException(Exception exception)
        {
            throw new EmulationEnvironmentCreationException(
 @"I couldn't parse the colors palette file. Make sure that:

- It is a text file containing extactly 16 text lines.
- Each line contains 3 color components, separated by spaces.
- Each color component is an integer number in the range 0-255."
            ,exception);
        }
    }
}
