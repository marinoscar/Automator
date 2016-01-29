using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recorder
{
    public interface IRecorderEvent
    {
        string Type { get; }
        long Tick { get; }
    }

    public abstract class RecorderEvent : IRecorderEvent
    {
        protected RecorderEvent()
        {
            Type = GetType().Name;
            Tick = DateTime.UtcNow.Ticks;
        }

        public string Type { get; protected set; }
        public long Tick { get; protected set; }
    }
}
