using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recorder
{
    public class DesktopRecorder
    {
        private MouseRecorder _mouseRecorder { get; set; }
        private KeyRecorder _keyRecorder { get; set; }

        public event EventHandler<string> LogAction;

        protected virtual void OnLogAction(string e)
        {
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
            OnLogAction(string.Format("Key: {0}", e.Key));
        }

        private void MouseRecorder_ActionRecorded(object sender, MouseEvent e)
        {
            OnLogAction(string.Format("Mouse Click: X: {0} Y: {1}", e.X, e.Y));
        }
    }
}
