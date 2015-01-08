using System;
using System.Drawing;
using System.Windows.Forms;

namespace Konamiman.NestorMSX.Host
{
    /// <summary>
    /// Represents a Windows Forms drawing surface, that is, 
    /// an object that contains an instance of the Graphics class.
    /// </summary>
    public interface IDrawingSurface
    {
        /// <summary>
        /// Event triggered when the object requires repaint.
        /// </summary>
        event EventHandler<PaintEventArgs> RequiresPaint;

        /// <summary>
        /// Gets the instance of Graphics that allows to draw on this object.
        /// </summary>
        /// <returns></returns>
        Graphics GetGraphics();
    }
}
