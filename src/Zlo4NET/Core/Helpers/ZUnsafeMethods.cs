using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Zlo4NET.Core.Helpers
{
    internal static class ZUnsafeMethods
    {
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool WaitNamedPipe(string name, int timeout);

        internal static bool NamedPipeExists(string pipeName)
        {
            try
            {
                var normalizedPath = Path.GetFullPath($@"\\.\pipe\{pipeName}");
                var exists = WaitNamedPipe(normalizedPath, 0);
                if (!exists)
                {
                    var error = Marshal.GetLastWin32Error();
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [DllImport("user32.dll")]
        internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        internal static extern bool SetForegroundWindow(IntPtr hWnd);
    }
}