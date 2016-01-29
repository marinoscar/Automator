using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Automator.Core
{
    public class MousePlayer
    {
        private static MousePlayer _instance;

        public static MousePlayer Instance
        {
            get { return _instance ?? (_instance = new MousePlayer()); }
        }

        private MousePlayer()
        {

        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);


        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, int dx, int dy, uint cButtons, uint dwExtraInfo);

        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;


        public void DoMouseClick(IMouseData data)
        {
            if (data.Button == 1048576)
                DoMouseLeftClick(data);
            else
                DoMouseRightClick(data);
        }

        private void DoMouseLeftClick(IMouseData data)
        {
            DoClick(
                () => mouse_event(MOUSEEVENTF_LEFTDOWN, data.X, data.Y, 0, 0),
                () => mouse_event(MOUSEEVENTF_LEFTUP, data.X, data.Y, 0, 0),
                data);
        }

        private void DoMouseRightClick(IMouseData data)
        {
            DoClick(
                () => mouse_event(MOUSEEVENTF_RIGHTDOWN, data.X, data.Y, 0, 0),
                () => mouse_event(MOUSEEVENTF_RIGHTUP, data.X, data.Y, 0, 0),
                data);
        }

        private void DoClick(Action down, Action up, IMouseData data)
        {
            SetCursorPos(data.X, data.Y);
            if(data.Count <= 1)
            {
                down();
                up();
                return;
            }
            down();
            up();
            Thread.Sleep(100);
            down();
            up();
        }
    }
}
