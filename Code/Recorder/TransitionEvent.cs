using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recorder
{

    public enum TransitionType { None, ActiveWindowChanged };

    public class TransitionEvent : RecorderEvent
    {
        public TransitionType TransitionType { get; set; }
    }
}
