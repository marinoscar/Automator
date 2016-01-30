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
        private TickInterval _keyTickInterval;
        private long _keyTick;

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
            long lastMouseTick = 0;
            foreach (var e in RawEvents)
            {
                if(e.Type == "TransitionEvent" && ((TransitionEvent)e).TransitionType == TransitionType.ActiveWindowChanged && sb.Length > 0)
                {
                    DoProcessKey(res, sb, e);
                }
                if (e.Type == "KeyEvent")
                {
                    AppendKeyEvents(e, sb);
                }
                if (e.Type == "MouseEvent")
                {
                    if (sb.Length > 0)
                    {
                        DoProcessKey(res, sb, e);
                    }
                    if (lastMouseTick <= 0)
                        lastMouseTick = e.Tick;
                    var m = (MouseEvent)e;
                    res.Add(MouseTask.FromData(m));
                    AddWait(res, (new TickInterval() { Start = lastMouseTick, End = e.Tick}).ToMilliseconds());
                    lastMouseTick = e.Tick;
                }
            }
            res.Remove(res.Last());
            return res;
        }

        private void DoProcessKey(List<ITask> taskList, StringBuilder sb, IRecorderEvent lastEvent)
        {
            taskList.Add(new KeyboardTask() { CommandText = sb.ToString() });
            _keyTickInterval.End = lastEvent.Tick;
            AddWait(taskList, _keyTickInterval.ToMilliseconds());
            sb.Clear();
        }

        private void AppendKeyEvents(IRecorderEvent e, StringBuilder sb)
        {
            var k = (KeyEvent)e;
            if (_keyTickInterval.Start <= 0)
                _keyTickInterval.Start = k.Tick;
            _keyTickInterval.End = k.Tick;
            sb.Append(k.Value);
        }

        private void AddWait(List<ITask> taskList, int ms)
        {
            taskList.Add(new WaitTask() { DurationInMs = ms });
        }

        private void AddTask(List<ITask> taskList, ITask task, TickInterval interval)
        {
            taskList.Add(task);
            AddWait(taskList, interval.ToMilliseconds());
        }


        public string GetCommands()
        {
            return JsonConvert.SerializeObject(TaskList);
        }
    }

    public struct TickInterval
    {
        public long Start { get; set; }
        public long End { get; set; }

        public int ToMilliseconds()
        {
            return (int)((End - Start) / 10000);
        }
    }
}
