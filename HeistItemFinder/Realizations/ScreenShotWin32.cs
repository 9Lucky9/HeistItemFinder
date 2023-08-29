using HeistItemFinder.Interfaces;
using HeistItemFinder.Win32;
using System;
using System.Drawing;
using static HeistItemFinder.Win32.GDI32;
using static HeistItemFinder.Win32.User32;

namespace HeistItemFinder.Realizations
{
    public class ScreenShotWin32 : IScreenShotWin32
    {
        /// <inheritdoc/>
        public Image CaptureScreen()
        {
            return CaptureWindow(GetDesktopWindow());
        }

        /// <inheritdoc/>
        public Image CaptureWindow(IntPtr handle)
        {
            // get te hDC of the target window
            IntPtr hdcSrc = GetWindowDC(handle);
            // get the size
            var windowRect = new RECT();
            GetWindowRect(handle, out windowRect);
            int width = windowRect.Right - windowRect.Left;
            int height = windowRect.Bottom - windowRect.Top;
            // create a device context we can copy to
            IntPtr hdcDest = CreateCompatibleDC(hdcSrc);
            // create a bitmap we can copy it to,
            // using GetDeviceCaps to get the width/height
            IntPtr hBitmap = CreateCompatibleBitmap(hdcSrc, width, height);
            // select the bitmap object
            IntPtr hOld = SelectObject(hdcDest, hBitmap);
            // bitblt over
            BitBlt(hdcDest, 0, 0, width, height, hdcSrc, 0, 0, SRCCOPY);
            // restore selection
            SelectObject(hdcDest, hOld);
            // clean up
            DeleteDC(hdcDest);
            ReleaseDC(handle, hdcSrc);

            // get a .NET image object for it
            Image img = Image.FromHbitmap(hBitmap);
            // free up the Bitmap object
            DeleteObject(hBitmap);

            return img;
        }
    }
}
