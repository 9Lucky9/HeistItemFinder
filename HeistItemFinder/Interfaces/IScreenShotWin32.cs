using System;
using System.Drawing;

namespace HeistItemFinder.Interfaces
{
    public interface IScreenShotWin32
    {
        public Image CaptureScreen();
        public Image CaptureWindow(IntPtr handle);
    }
}
