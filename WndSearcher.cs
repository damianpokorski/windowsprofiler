using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
namespace WindowsProfiler
{
    public class WndSearcher
    {
        public IntPtr SearchForWindow(string wndclass, string title)
        {
            WindowData sd = new WindowData { Wndclass = wndclass, Title = title };
            EnumWindows(new EnumWindowsProc(EnumProc), ref sd);
            return sd.hWnd;
        }

        public bool EnumProc(IntPtr hWnd, ref WindowData data)
        {
            // Check classname and title 
            // This is different from FindWindow() in that the code below allows partial matches
            if (data.Title != "")
            {
                StringBuilder sb = new StringBuilder(1024);
                GetClassName(hWnd, sb, sb.Capacity);
                if (sb.ToString().StartsWith(data.Wndclass))
                {
                    sb = new StringBuilder(1024);
                    GetWindowText(hWnd, sb, sb.Capacity);
                    if (sb.ToString().StartsWith(data.Title))
                    {
                        data.hWnd = hWnd;
                        return false;    // Found the wnd, halt enumeration
                    }
                }
            }
            else
            {
                if (IsWindowVisible(hWnd))
                {
                    WindowData window = new WindowData();
                    StringBuilder sb = new StringBuilder(1024);
                    GetClassName(hWnd, sb, sb.Capacity);
                    window.Wndclass = sb.ToString();
                    GetWindowText(hWnd, sb, 1024);
                    window.Title = sb.ToString();
                    window.hWnd = hWnd;
                    if (window.Title != "")
                    {
                        all_windows.Add(window);
                    }
                }
            }
            return true;
        }
        public List<WindowData> all_windows = new List<WindowData>();
        public List<WindowData> GetAllWindows()
        {
            all_windows.Clear();
            WindowData sd = new WindowData { Wndclass = "", Title = "" };
            EnumWindows(new EnumWindowsProc(EnumProc), ref sd);
            return all_windows;
        }



        private delegate bool EnumWindowsProc(IntPtr hWnd, ref WindowData data);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, ref WindowData data);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWindowVisible(IntPtr hWnd);
    }
}
