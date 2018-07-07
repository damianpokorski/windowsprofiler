using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WindowsProfiler
{
    public class WindowData
    {
        public string Wndclass;
        public string Title = "";
        public IntPtr hWnd;
    }
    public class WindowsController
    {
        
        public WindowsController()
        {

        }
        List<WindowData> window_list = new List<WindowData>();
        public void ListWindows()
        {
            window_list = new WndSearcher().GetAllWindows();
        }
        public List<WindowData> GetAllWindows()
        {
            ListWindows();
            return window_list;
        }
        public WindowData FindWindow(string title)
        {
            Console.WriteLine(title + "MEH");
            foreach (WindowData window in GetAllWindows())
            {
                if (window.Title != null)
                {
                    if (window.Title.Contains(title))
                    {
                        Console.WriteLine("FOUND : " + title);
                        return window;
                    }
                }
            }
            return new WindowData();
        }
        public RECT GetWindowPos(WindowData _Window)
        {
            RECT rectangle = new RECT();
            GetWindowRect(_Window.hWnd,ref rectangle);
            return rectangle;
        }
        public void SetWindowPosition(string WindowTitle,int PositionX,int PositionY,int Width,int Height)
        {
            if(FindWindow(WindowTitle).Title != null){
                /*
                SetWindowPos(
                    FindWindow(WindowTitle).hWnd,
                    new IntPtr(1),
                    PositionX, PositionY,
                     Width,
                     Height,
                    SetWindowPosFlags.SWP_DRAWFRAME);
                 */
                ShowWindow(FindWindow(WindowTitle).hWnd);
                MoveWindow(FindWindow(WindowTitle).hWnd, PositionX, PositionY, Width, Height, true);
                
            }
        }

        public void SetWindowOnTop()
        {

        }
        public void MinimizeAll()
        {
            foreach (WindowData _w in GetAllWindows())
            {
                Minimize(_w.hWnd);
            }
        }
        public void MinimizeAllExcept(List<string> exceptions)
        {
            foreach (WindowData _Window in GetAllWindows())
            {
                Boolean minimize = true;
                if (exceptions.Contains(_Window.Title))
                {
                    minimize = false;

                }
                else
                {
                    foreach (string exception in exceptions)
                    {
                        if(_Window.Title.Contains(exception)){
                            minimize = false;
                        }
                    }
                }
                if (minimize)
                {
                    if (_Window.Title != "Start")
                    {
                        Minimize(_Window.hWnd);
                    }
                }
            }
        }
        public void UndoMinimizeAll()
        {
            IntPtr lHwnd = FindWindow("Shell_TrayWnd", null);
            SendMessage(lHwnd, WM_COMMAND, (IntPtr)MIN_ALL_UNDO, IntPtr.Zero); 
        }
        public void Minimize(IntPtr hWnd)
        {
            ShowWindow(hWnd, SW_SHOWMINIMIZED);
        }
        public void ShowWindow(IntPtr hWnd)
        {
            ShowWindow( hWnd,SW_SHOWNORMAL);
        }
        public void MaximizeWindow(IntPtr hWnd)
        {
            ShowWindow(hWnd, SW_SHOWMAXIMIZED);
        }
        // Minimize / deminimize
        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;
        private const int SW_SHOWMAXIMIZED = 3;

        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        // MINIMIZE  ALL / UNDO MINIMIZE ALL STUFF
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", EntryPoint = "SendMessage", SetLastError = true)]
        static extern IntPtr SendMessage(IntPtr hWnd, Int32 Msg, IntPtr wParam, IntPtr lParam);

        const int WM_COMMAND = 0x111;
        const int MIN_ALL = 419;
        const int MIN_ALL_UNDO = 416;

        // GET WINDOW POS PINVOKES
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
        // 
        // SETTING WINDOW POS  / SET ON TOP STUFF
        //
        [DllImport("User32.dll")]
        static extern bool MoveWindow(IntPtr handle, int x, int y, int width, int height, bool redraw);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);        
        /// <summary>
        ///     Special window handles
        /// </summary>
        [Flags]
        public enum SpecialWindowHandles : int
        {
            // ReSharper disable InconsistentNaming
            /// <summary>
            ///     Places the window at the bottom of the Z order. If the hWnd parameter identifies a topmost window, the window loses its topmost status and is placed at the bottom of all other windows.
            /// </summary>
            HWND_TOP = 0,
            /// <summary>
            ///     Places the window above all non-topmost windows (that is, behind all topmost windows). This flag has no effect if the window is already a non-topmost window.
            /// </summary>
            HWND_BOTTOM = 1,
            /// <summary>
            ///     Places the window at the top of the Z order.
            /// </summary>
            HWND_TOPMOST = -1,
            /// <summary>
            ///     Places the window above all non-topmost windows. The window maintains its topmost position even when it is deactivated.
            /// </summary>
            HWND_NOTOPMOST = -2
            // ReSharper restore InconsistentNaming
        }

        [Flags]
        public enum SetWindowPosFlags : uint
        {
            // ReSharper disable InconsistentNaming

            /// <summary>
            ///     If the calling thread and the thread that owns the window are attached to different input queues, the system posts the request to the thread that owns the window. This prevents the calling thread from blocking its execution while other threads process the request.
            /// </summary>
            SWP_ASYNCWINDOWPOS = 0x4000,

            /// <summary>
            ///     Prevents generation of the WM_SYNCPAINT message.
            /// </summary>
            SWP_DEFERERASE = 0x2000,

            /// <summary>
            ///     Draws a frame (defined in the window's class description) around the window.
            /// </summary>
            SWP_DRAWFRAME = 0x0020,

            /// <summary>
            ///     Applies new frame styles set using the SetWindowLong function. Sends a WM_NCCALCSIZE message to the window, even if the window's size is not being changed. If this flag is not specified, WM_NCCALCSIZE is sent only when the window's size is being changed.
            /// </summary>
            SWP_FRAMECHANGED = 0x0020,

            /// <summary>
            ///     Hides the window.
            /// </summary>
            SWP_HIDEWINDOW = 0x0080,

            /// <summary>
            ///     Does not activate the window. If this flag is not set, the window is activated and moved to the top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter parameter).
            /// </summary>
            SWP_NOACTIVATE = 0x0010,

            /// <summary>
            ///     Discards the entire contents of the client area. If this flag is not specified, the valid contents of the client area are saved and copied back into the client area after the window is sized or repositioned.
            /// </summary>
            SWP_NOCOPYBITS = 0x0100,

            /// <summary>
            ///     Retains the current position (ignores X and Y parameters).
            /// </summary>
            SWP_NOMOVE = 0x0002,

            /// <summary>
            ///     Does not change the owner window's position in the Z order.
            /// </summary>
            SWP_NOOWNERZORDER = 0x0200,

            /// <summary>
            ///     Does not redraw changes. If this flag is set, no repainting of any kind occurs. This applies to the client area, the nonclient area (including the title bar and scroll bars), and any part of the parent window uncovered as a result of the window being moved. When this flag is set, the application must explicitly invalidate or redraw any parts of the window and parent window that need redrawing.
            /// </summary>
            SWP_NOREDRAW = 0x0008,

            /// <summary>
            ///     Same as the SWP_NOOWNERZORDER flag.
            /// </summary>
            SWP_NOREPOSITION = 0x0200,

            /// <summary>
            ///     Prevents the window from receiving the WM_WINDOWPOSCHANGING message.
            /// </summary>
            SWP_NOSENDCHANGING = 0x0400,

            /// <summary>
            ///     Retains the current size (ignores the cx and cy parameters).
            /// </summary>
            SWP_NOSIZE = 0x0001,

            /// <summary>
            ///     Retains the current Z order (ignores the hWndInsertAfter parameter).
            /// </summary>
            SWP_NOZORDER = 0x0004,

            /// <summary>
            ///     Displays the window.
            /// </summary>
            SWP_SHOWWINDOW = 0x0040,

            // ReSharper restore InconsistentNaming
        }
    }
}
