using System;
using System.Drawing;

namespace HeistItemFinder.Interfaces
{
    public interface IScreenShotWin32
    {
        /// <summary>
        /// Creates an Image object containing a screen shot of a screen.
        /// </summary>
        /// <returns></returns>
        public Image CaptureScreen();

        /// <summary>
        /// Creates an Image object containing a screen shot of a specific window
        /// </summary>
        /// <param name="handle">The handle to the window.</param>
        public Image CaptureWindow(IntPtr handle);
    }
}
