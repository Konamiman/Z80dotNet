using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Konamiman.NestorMSX
{
    public partial class FormTest : Form
    {
        public FormTest()
        {
            InitializeComponent();
            pictureBox.Paint += PictureBoxOnPaint;
        }

        private unsafe void PictureBoxOnPaint(object sender, PaintEventArgs paintEventArgs)
        {
            var g = paintEventArgs.Graphics;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.ScaleTransform(8, 8);
            g.TranslateTransform(8, 8);

            var data = new byte[] {1, 0,0,0,2,0,0,0, 3,0,0,0, 4,0,0,0, 5,0,0,0, 6,0,0,0, 7,0,0,0, 8,0,0,0};
            Bitmap bmp;
            fixed(byte* p = data)
                bmp = new Bitmap(8, 8, 4, PixelFormat.Format1bppIndexed, new IntPtr(p));
            var palette = bmp.Palette;
            palette.Entries[0] = Color.Blue;
            palette.Entries[1] = Color.White;
            bmp.Palette = palette;


            var brush = new TextureBrush(bmp);

            g.DrawImage(bmp, 0, 0);
            g.TranslateTransform(0,8);
            g.DrawImage(bmp, new Rectangle(0, 0, 6, 8));
        }

        protected override unsafe void OnPaint(PaintEventArgs e)
        {
            
        }

        private unsafe void FormTest_Load(object sender, EventArgs e)
        {
            pictureBox.CreateGraphics().ScaleTransform(8,8);
        }

        static byte[] PadLines(byte[] bytes, int rows, int columns)
        {
            //The old and new offsets could be passed through parameters,
            //but I hardcoded them here as a sample.
            var currentStride = columns * 3;
            var newStride = columns * 4;
            var newBytes = new byte[newStride * rows];
            for(var i = 0; i < rows; i++)
                Buffer.BlockCopy(bytes, currentStride * i, newBytes, newStride * i, currentStride);
            return newBytes;
        }
    }
}
