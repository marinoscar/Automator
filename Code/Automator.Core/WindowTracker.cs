using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Automator.Core
{
    public class WindowTracker
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        private IntPtr CurrentActiveWindow { get; set; }

        public WindowTracker()
        {
            CurrentActiveWindow = GetActiveWindowId();
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
            return GetForegroundWindow();
        }

        public bool HasActiveWindowChanged()
        {
            var result = CurrentActiveWindow != GetActiveWindowId();
            if (result) CurrentActiveWindow = GetActiveWindowId();
            return result;
        }
    }
}
