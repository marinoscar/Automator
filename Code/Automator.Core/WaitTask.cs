using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Automator.Core
{
    public class WaitTask : ITask
    {
        public int DurationInMs { get; set; }

        public void Execute()
        {
            Thread.Sleep(DurationInMs);
        }

        public string TaskCaption { get { return string.Format("Wait {0}ms", DurationInMs); } }

        public string TaskName { get { return GetType().Name; } }
    }
}
