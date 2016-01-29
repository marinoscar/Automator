using Gma.System.MouseKeyHook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Recorder
{
    public class KeyRecorder
    {
        private IKeyboardMouseEvents _globalHook;
        public List<KeyEvent> Keys { get; private set; }
        public event EventHandler<KeyEvent> ActionRecorded;

        public KeyRecorder()
        {
            Keys = new List<KeyEvent>(1000);
        }

        public void Initialize()
        {
            _globalHook = Hook.GlobalEvents();
            _globalHook.KeyPress += KeyPress;

        }

        protected virtual void OnActionRecorded(KeyEvent e)
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

        private void KeyPress(object sender, KeyPressEventArgs e)
        {
            DoRecord(e);
        }

        #endregion

        

        protected virtual void DoRecord(KeyPressEventArgs e)
        {
            var v = KeyEvent.FromEvent(e);
            Keys.Add(v);
            OnActionRecorded(v);
        }
    }

    public class KeyEvent
    {
        public static KeyEvent FromEvent(KeyPressEventArgs e)
        {
            return new KeyEvent()
            {
                Key = e.KeyChar,
                Tick = DateTime.UtcNow.Ticks
            };
        }
        public char Key { get; set; }
        public long Tick { get; set; }
    }
}
