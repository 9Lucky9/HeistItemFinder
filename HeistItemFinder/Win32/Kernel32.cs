using System;
using System.Runtime.InteropServices;

namespace HeistItemFinder.Win32
{
    public class Kernel32
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr GetModuleHandle([MarshalAs(UnmanagedType.LPWStr)] string lpModuleName);
    }
}
