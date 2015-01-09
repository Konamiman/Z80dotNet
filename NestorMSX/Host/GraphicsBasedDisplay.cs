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

        private readonly object syncObject = new object();
        private readonly IDrawingSurface drawingSurface;
        private IDictionary<Point, byte> screenBuffer;
        private Graphics defaultGraphics;
        private Color BackdropColor;
        private int characterWidth = 8;
        private bool screenIsActive = false;
        private IDictionary<byte, byte[]> characterPatterns = new Dictionary<byte, byte[]>();
        private IDictionary<byte, Tuple<Color, Color>> characterColors = new Dictionary<byte, Tuple<Color, Color>>();
        private IDictionary<byte, Tuple<Brush, Brush>> characterBrushes = new Dictionary<byte, Tuple<Brush, Brush>>();
        private readonly Configuration config;

        public GraphicsBasedDisplay(IDrawingSurface drawingSurface, Configuration config)
        {
            this.config = config;

            BackdropColor = Color.Blue;

            for(int i = 0; i < 256; i++) {
                characterColors[(byte)i] = new Tuple<Color, Color>(Color.Black, Color.Black);
                characterBrushes[(byte)i] = new Tuple<Brush, Brush>(new SolidBrush(Color.Black), new SolidBrush(Color.Black));
            }

            defaultGraphics = drawingSurface.GetGraphics();
            defaultGraphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            Transform(defaultGraphics);

            drawingSurface.RequiresPaint += DrawingSurfaceOnRequiresPaint;
        }

        private void DrawingSurfaceOnRequiresPaint(object sender, PaintEventArgs eventArgs)
        {
            lock(syncObject) {
                Transform(eventArgs.Graphics);
                RepaintAll(eventArgs.Graphics);
            }
        }

        private void Transform(Graphics graphics)
        {
            graphics.ScaleTransform((float)config.DisplayZoomLevel, (float)config.DisplayZoomLevel);
            graphics.TranslateTransform(config.HorizontalMarginInPixels, config.VerticalMarginInPixels);
        }

        private void RepaintAll(Graphics graphics)
        {
            ClearScreen(graphics);
            foreach(var item in screenBuffer)
                DrawCharacter(item.Key, item.Value, graphics);
        }

        private void DrawCharacter(Point coordinates, byte charIndex, Graphics graphics)
        {
            if(!screenIsActive)
                return;

            lock(syncObject) {
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
        }

        public void SetScreenBufer(IDictionary<Point, byte> value)
        {
            screenBuffer = value;
        }

        public void SetBackdropColor(Color color)
        {
            BackdropColor = color;
            RepaintAll(defaultGraphics);
        }

        public void ClearScreen(Graphics graphics)
        {
            lock(syncObject)
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
                DrawCharacter(item.Key, item.Value, defaultGraphics);
        }

        public void SetCharacterWidth(int width)
        {
            characterWidth = width;
        }

        public void NotifyScreenBufferContentsAddedOrChanged(Point coordinates)
        {
            DrawCharacter(coordinates, screenBuffer[coordinates], defaultGraphics);
        }

        public void NotifyScreenBufferContentsRemoved(Point coordinates)
        {
        }

        public void BlankScreen()
        {
            ClearScreen(defaultGraphics);
            screenIsActive = false;
        }

        public void ActivateScreen()
        {
            screenIsActive = true;
            RepaintAll(defaultGraphics);
        }
    }
}
