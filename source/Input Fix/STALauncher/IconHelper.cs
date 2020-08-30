using System;
using System.Runtime.InteropServices;

namespace STALauncher
{
    internal class IconHelper
    {
        internal static void SetConsoleIcon(IntPtr icon)
        {
            Win32.SendMessage(Win32.FindWindow(null, Console.Title), 0x0080, (IntPtr)0, icon);
            Win32.SendMessage(Win32.FindWindow(null, Console.Title), 0x0080, (IntPtr)1, icon);
            Win32.SendMessage(Win32.FindWindow(null, Console.Title), 0x0080, (IntPtr)2, icon);
        }

        internal static IntPtr GetIconOf(string file)
        {
            SHFILEINFO shinfo = new SHFILEINFO();
            Win32.SHGetFileInfo(file, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), Win32.SHGFI_ICON | Win32.SHGFI_LARGEICON);
            return shinfo.hIcon;
        }

        #region Icon Native Uitls

        //https://www.cnblogs.com/zhangtao/archive/2011/04/25/2027246.html
        [StructLayout(LayoutKind.Sequential)]
        internal struct SHFILEINFO
        {
            public IntPtr hIcon;
            public IntPtr iIcon;
            public uint dwAttributes;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };

        internal class Win32
        {
            #region WM Msg

            public const uint SHGFI_ICON = 0x100;
            public const uint SHGFI_LARGEICON = 0x0; // 'Large icon
            public const uint SHGFI_SMALLICON = 0x1; // 'Small icon

            #endregion WM Msg

            [DllImport("shell32.dll")]
            internal static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

            [DllImport("User32.dll", EntryPoint = "SendMessage")]
            internal static extern IntPtr SendMessage(int hWnd, int msg, IntPtr wParam, IntPtr lParam);

            [DllImport("User32.dll", EntryPoint = "FindWindow")]
            internal static extern int FindWindow(string lpClassName, string lpWindowName);
        }

        #endregion Icon Native Uitls
    }
}