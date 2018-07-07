using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsProfiler
{
    class Screen
    {
        int ScreenNumber;
        public Screen(int _ScreenNumber)
        {
            ScreenNumber = _ScreenNumber;
        }
        public int ColumnWidth(int _column)
        {
            return System.Windows.Forms.Screen.AllScreens[ScreenNumber].WorkingArea.Width / _column;
        }
        public int RowHeight(int _row)
        {
            return System.Windows.Forms.Screen.AllScreens[ScreenNumber].WorkingArea.Height / _row;
        }
        public int PushLeft()
        {
            return System.Windows.Forms.Screen.AllScreens[ScreenNumber].WorkingArea.X;
        }
        public int PushTop()
        {
            return System.Windows.Forms.Screen.AllScreens[ScreenNumber].WorkingArea.Y;
        }
    }
}
