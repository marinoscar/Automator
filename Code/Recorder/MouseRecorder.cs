using Automator.Core;
using Gma.System.MouseKeyHook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Recorder
{
    public class MouseRecorder : IDisposable
    {

        private IKeyboardMouseEvents _globalHook;
        public List<MouseEvent> Events { get; private set; }
        public event EventHandler<MouseEvent> ActionRecorded;

        public MouseRecorder()
        {
            Events = new List<MouseEvent>(1000);
        }

        public void Initialize()
        {
            _globalHook = Hook.GlobalEvents();
            _globalHook.MouseClick += MouseClick;
            _globalHook.MouseDoubleClick += MouseDoubleClick;
        }

        protected virtual void OnActionRecorded(MouseEvent e)
        {
            var handler = ActionRecorded;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void Dispose()
        {
            if (_globalHook != null) _globalHook.Dispose();
        }

        #region Events

        private void MouseDoubleClick(object sender, MouseEventArgs e)
        {
            OnDoubleClick(e);
        }

        private void MouseClick(object sender, MouseEventArgs e)
        {
            OnClick(e);
        }

        #endregion

        protected virtual void OnClick(MouseEventArgs e)
        {
            DoRecord(e, false);
        }

        protected virtual void OnDoubleClick(MouseEventArgs e)
        {
            DoRecord(e, true);
        }

        protected virtual MouseEvent DoRecord(MouseEventArgs e, bool isDbl)
        {
            var v = MouseEvent.FromEvent(e);
            Events.Add(v);
            OnActionRecorded(v);
            if(Events.Count > 1 && isDbl)
            {
                Events.RemoveAt(Events.IndexOf(v)- 1);
            }
            return v;
        }

    }

    public class MouseEvent : RecorderEvent, IMouseData
    {

        public static MouseEvent FromEvent(MouseEventArgs e)
        {
            return new MouseEvent()
            {
                Count = e.Clicks,
                X = e.X,
                Y = e.Y,
                Button = Convert.ToInt32(e.Button)
            };
        }

        public int X { get; set; }
        public int Y { get; set; }

        public int Count { get; set; }

        public int Button { get; set; }

    }
}
