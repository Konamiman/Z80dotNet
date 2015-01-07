using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using Konamiman.Z80dotNet;

namespace Konamiman.NestorMSX.Host
{
    public class GraphicsBasedDisplay : ICharacterBasedDisplay
    {
        private const int zoomLevel = 2;

        private readonly IDrawingSurface drawingSurface;
        private IDictionary<Point, byte> screenBuffer;
        private Graphics graphics;
        private Color BackdropColor;
        private int characterWidth = 8;
        private bool screenIsActive = false;
        private IDictionary<byte, byte[]> characterPatterns = new Dictionary<byte, byte[]>();
        private IDictionary<byte, Tuple<Color, Color>> characterColors = new Dictionary<byte, Tuple<Color, Color>>();
        private IDictionary<byte, Tuple<Brush, Brush>> characterBrushes = new Dictionary<byte, Tuple<Brush, Brush>>();

        public GraphicsBasedDisplay(IDrawingSurface drawingSurface)
        {
            BackdropColor = Color.Blue;

            for(int i = 0; i < 256; i++) {
                characterColors[(byte)i] = new Tuple<Color, Color>(Color.Black, Color.Black);
                characterBrushes[(byte)i] = new Tuple<Brush, Brush>(new SolidBrush(Color.Black), new SolidBrush(Color.Black));
            }

            graphics = drawingSurface.GetGraphics();
            graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            graphics.ScaleTransform(zoomLevel, zoomLevel);
            graphics.TranslateTransform(8, 8);

            drawingSurface.RequiresPaint += DrawingSurfaceOnRequiresPaint;
        }

        private void DrawingSurfaceOnRequiresPaint(object sender, PaintEventArgs eventArgs)
        {
            eventArgs.Graphics.Clear(BackdropColor);
            RepaintAll();
        }

        private void RepaintAll()
        {
            ClearScreen();
            foreach(var item in screenBuffer)
                DrawCharacter(item.Key, item.Value);
        }

        private void DrawCharacter(Point coordinates, byte charIndex)
        {
            if(!screenIsActive)
                return;

            var baseX = (coordinates.X*characterWidth) + (characterWidth == 6 ? 8 : 0);
            var X = baseX;
            var Y = coordinates.Y*8;
            var brushes = characterBrushes[charIndex];
            var pattern = characterPatterns[charIndex];
            graphics.FillRectangle(brushes.Item2, baseX, Y, characterWidth, 8);
            for(int row = 0; row < 8; row++) {
                for(int column = 7; column >= 8 - characterWidth; column--) {
                    if(pattern[row].GetBit(column)) {
                        graphics.FillRectangle(brushes.Item1, X + (7 - column), Y, 1, 1);
                    }
                }
                X = baseX;
                Y++;
            }
        }

        public void SetScreenBufer(IDictionary<Point, byte> value)
        {
            screenBuffer = value;
        }

        public void SetBackdropColor(Color color)
        {
            BackdropColor = color;
            RepaintAll();
        }

        public void ClearScreen()
        {
            graphics.Clear(BackdropColor);
        }

        public void SetCharacterPattern(byte charIndex, byte[] pattern)
        {
            characterPatterns[charIndex] = pattern;
            ReprintAllInstancesOf(charIndex);
        }

        public void SetCharacterColors(byte charIndex, Color foreground, Color background)
        {
            characterColors[charIndex] = new Tuple<Color, Color>(foreground, background);

            var oldBrushes = characterBrushes[charIndex];
            oldBrushes.Item1.Dispose();
            oldBrushes.Item2.Dispose();

            var newBrushes = new Tuple<Brush, Brush>(
                new SolidBrush(foreground),
                new SolidBrush(background));
            characterBrushes[charIndex] = newBrushes;

            ReprintAllInstancesOf(charIndex);
        }

        private void ReprintAllInstancesOf(byte charIndex)
        {
            foreach(var item in screenBuffer.Where(i => i.Value == charIndex))
                DrawCharacter(item.Key, item.Value);
        }

        public void SetCharacterWidth(int width)
        {
            characterWidth = width;
        }

        public void NotifyScreenBufferContentsAddedOrChanged(Point coordinates)
        {
            DrawCharacter(coordinates, screenBuffer[coordinates]);
        }

        public void NotifyScreenBufferContentsRemoved(Point coordinates)
        {
        }

        public void BlankScreen()
        {
            ClearScreen();
            screenIsActive = false;
        }

        public void ActivateScreen()
        {
            screenIsActive = true;
            RepaintAll();
        }
    }
}
