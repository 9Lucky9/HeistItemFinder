using System.Runtime.InteropServices;
using System;

namespace HeistItemFinder.Win32
{
    public class User32
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(int hookType, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);
        
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, [In] IntPtr lParam);


    }
}
