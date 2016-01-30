using Automator.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Recorder
{
    public class DesktopRecorder
    {
        [DllImport("user32.dll")]
        static extern int MapVirtualKey(uint uCode, uint uMapType);

        private MouseRecorder _mouseRecorder { get; set; }
        private KeyRecorder _keyRecorder { get; set; }
        private WindowTracker _windowTracker;
        private List<ITask> _taskList;

        protected List<IRecorderEvent> RawEvents { get; set; }

        public List<ITask> TaskList { get { return _taskList ?? (_taskList = GetTasks()); } }

        public event EventHandler<string> LogAction;

        public bool DisableDebug { get; set; }


        public DesktopRecorder()
        {
            RawEvents = new List<IRecorderEvent>(10000);
            _windowTracker = new WindowTracker();
        }

        protected virtual void OnLogAction(string e)
        {
            if (DisableDebug) return;
            var handler = LogAction;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void Start()
        {
            _mouseRecorder = new MouseRecorder();
            _keyRecorder = new KeyRecorder();
            _mouseRecorder.ActionRecorded += MouseRecorder_ActionRecorded;
            _keyRecorder.ActionRecorded += KeyRecorder_ActionRecorded;
            _mouseRecorder.Initialize();
            _keyRecorder.Initialize();
        }

        public void Stop()
        {
            if (_mouseRecorder != null)
                _mouseRecorder.Dispose();
            if (_keyRecorder != null)
                _keyRecorder.Dispose();
            _mouseRecorder = null;
            _keyRecorder = null;
        }

        private void KeyRecorder_ActionRecorded(object sender, KeyEvent e)
        {
            AddEvent(e);
            OnLogAction(string.Format("Value: {7} Key: {0} Data: {1} Code: {2} Modifiers: {3} Shift: {4} Control: {5} Alt: {6}", e.Key, e.Data, e.Code, e.Modifiers, e.Shift, e.Control, e.Alt, e.Value.ToString()));
        }

        private void MouseRecorder_ActionRecorded(object sender, MouseEvent e)
        {
            AddEvent(e);
            OnLogAction(string.Format("Mouse Click: X: {0} Y: {1} Count: {2} Button: {3}", e.X, e.Y, e.Count, e.Button));
        }

        private void AddEvent(IRecorderEvent e)
        {
            if(_windowTracker.HasActiveWindowChanged())
            {
                OnLogAction(string.Format("Active Window Changed to: {0} id: {0}", _windowTracker.GetActiveWindowTitle(), _windowTracker.GetActiveWindowId().ToInt64()));
                RawEvents.Add(new TransitionEvent() { TransitionType = TransitionType.ActiveWindowChanged });
            }
            RawEvents.Add(e);
        }

        public void RunTasks()
        {
            TaskList.ForEach(i => i.Execute());
        }

        private List<ITask> GetTasks()
        {
            var res = new List<ITask>(1000);
            var sb = new StringBuilder();
            foreach (var e in RawEvents)
            {
                if(e.Type == "TransitionEvent" && ((TransitionEvent)e).TransitionType == TransitionType.ActiveWindowChanged && sb.Length > 0)
                {
                    DoProcessKey(res, sb);
                }
                if (e.Type == "KeyEvent")
                {
                    var k = (KeyEvent)e;
                    sb.Append(k.Value);
                }
                if (e.Type == "MouseEvent")
                {
                    if (sb.Length > 0)
                    {
                        DoProcessKey(res, sb);
                    }
                    var m = (MouseEvent)e;
                    res.Add(MouseTask.FromData(m));
                    AddWait(res, 500);
                }
            }
            res.Remove(res.Last());
            return res;
        }

        private void DoProcessKey(List<ITask> taskList, StringBuilder sb)
        {
            taskList.Add(new KeyboardTask() { CommandText = sb.ToString() });
            AddWait(taskList, 500);
            sb.Clear();
        }

        private void AddWait(List<ITask> taskList, int ms)
        {
            taskList.Add(new WaitTask() { DurationInMs = ms });
        }


        public string GetCommands()
        {
            return JsonConvert.SerializeObject(TaskList);
        }
    }
}
