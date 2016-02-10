using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Automator.Core
{
    public class WindowTracker
    {

        private WindowInfo _current;

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        public IntPtr CurrentActiveWindowHandler { get; set; }
        public WindowInfo CurrentActiveWindow
        {
            get
            {
                if (_current == null || HasActiveWindowChanged())
                    _current = new WindowInfo(CurrentActiveWindowHandler);
                return _current;
            }
        }


        public WindowTracker()
        {
            CurrentActiveWindowHandler = GetActiveWindowId();
        }

        public string GetActiveWindowTitle()
        {
            const int chars = 256;
            var buffer = new StringBuilder(chars);
            var handle = GetForegroundWindow();

            if (GetWindowText(handle, buffer, chars) > 0)
            {
                return buffer.ToString();
            }
            return null;
        }

        public IntPtr GetActiveWindowId()
        {
            var handler = GetForegroundWindow();
            var control = Control.FromHandle(handler);
            if (control != null)
                System.Diagnostics.Debug.WriteLine(control.Name);
            return handler;
        }

        public bool HasActiveWindowChanged()
        {
            var result = CurrentActiveWindowHandler != GetActiveWindowId();
            if (result) CurrentActiveWindowHandler = GetActiveWindowId();
            return result;
        }

    }

    public class WindowInfo
    {
        public const int WM_GETTEXT = 0xD;
        public const int WM_GETTEXTLENGTH = 0x000E;

        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(Point point);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr handle, StringBuilder ClassName, int MaxCount);

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr handle, int msg, int Param1, int Param2);

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr handle, int msg, int Param, System.Text.StringBuilder text);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr handle, out RECT Rect);

        public IntPtr Handle;
        public string ClassName;
        public string Text;
        public Rectangle Rect;

        public WindowInfo(IntPtr Handle)
        {
            this.Handle = Handle;
            this.ClassName = GetWindowClassName(Handle);
            this.Text = GetWindowText(Handle);
            this.Rect = GetWindowRectangle(Handle);
        }

        public string GetWindowClassName(IntPtr handle)
        {
            StringBuilder buffer = new StringBuilder(128);
            GetClassName(handle, buffer, buffer.Capacity);
            return buffer.ToString();
        }

        public string GetWindowText(IntPtr handle)
        {
            StringBuilder buffer = new StringBuilder(SendMessage(handle, WM_GETTEXTLENGTH, 0, 0) + 1);
            SendMessage(handle, WM_GETTEXT, buffer.Capacity, buffer);
            return buffer.ToString();
        }

        public Rectangle GetWindowRectangle(IntPtr handle)
        {
            RECT rect = new RECT();
            GetWindowRect(handle, out rect);
            return new Rectangle(rect.Left, rect.Top, (rect.Right - rect.Left) + 1, (rect.Bottom - rect.Top) + 1);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }
}
