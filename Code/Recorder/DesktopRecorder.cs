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
        protected List<IRecorderEvent> RawEvents { get; set; }

        public event EventHandler<string> LogAction;

        public bool DisableDebug { get; set; }


        public DesktopRecorder()
        {
            RawEvents = new List<IRecorderEvent>(10000);
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
            _mouseRecorder.Dispose();
            _keyRecorder.Dispose();
            _mouseRecorder = null;
            _keyRecorder = null;
        }

        private void KeyRecorder_ActionRecorded(object sender, KeyEvent e)
        {
            RawEvents.Add(e);
            OnLogAction(string.Format("Key: {0} Data: {1} Code: {2} Modifiers: {3} Shift: {4} Control: {5} Alt: {6}", e.Key, e.Data, e.Code, e.Modifiers, e.Shift, e.Control, e.Alt));
        }

        private void MouseRecorder_ActionRecorded(object sender, MouseEvent e)
        {
            RawEvents.Add(e);
            OnLogAction(string.Format("Mouse Click: X: {0} Y: {1} Count: {2} Button: {3}", e.X, e.Y, e.Count, e.Button));
        }

        private List<ITask> GetTasks()
        {
            var res = new List<ITask>(1000);
            var sb = new StringBuilder();
            foreach(var e in RawEvents)
            {
                if(e.Type == "KeyEvent")
                {
                    var k = (KeyEvent)e;
                    if (k.Modifiers == Keys.None)
                        sb.Append(GetString(k.Data));
                }
                if(e.Type == "MouseEvent")
                {
                    if(sb.Length > 0)
                    {
                        res.Add(new KeyboardTask() { CommandText = sb.ToString() });
                        res.Add(new WaitTask() { DurationInMs = 500 });
                    }
                    sb.Clear();
                    var m = (MouseEvent)e;
                    res.Add(MouseTask.FromData(m));
                }
            }
            return res;
        }

        private string GetString(Keys keyData)
        {
            var nonVirtualKey = MapVirtualKey((uint)keyData, 2);
            return  Convert.ToChar(nonVirtualKey).ToString();
        }

        public string GetCommands()
        {
            return JsonConvert.SerializeObject(GetTasks());
        }
    }
}
