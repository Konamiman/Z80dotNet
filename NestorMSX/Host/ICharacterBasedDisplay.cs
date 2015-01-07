using System.Collections.Generic;
using System.Drawing;

namespace Konamiman.NestorMSX.Host
{
    public interface ICharacterBasedDisplay
    {
        void SetScreenBufer(IDictionary<Point, byte> value);
        void SetBackdropColor(Color color);
        void ClearScreen();
        void SetCharacterPattern(byte charIndex, byte[] pattern);
        void SetCharacterColors(byte charIndex, Color foreground, Color background);
        void SetCharacterWidth(int width);
        void NotifyScreenBufferContentsAddedOrChanged(Point coordinates);
        void NotifyScreenBufferContentsRemoved(Point coordinates);
        void BlankScreen();
        void ActivateScreen();
    }
}
