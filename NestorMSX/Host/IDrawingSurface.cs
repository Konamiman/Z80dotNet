using System;
using System.Drawing;
using System.Windows.Forms;

namespace Konamiman.NestorMSX.Host
{
    public interface IDrawingSurface
    {
        event EventHandler<PaintEventArgs> RequiresPaint;

        Graphics GetGraphics();
    }
}
