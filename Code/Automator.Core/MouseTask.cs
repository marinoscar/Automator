using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automator.Core
{
    public class MouseTask : ITask, IMouseData
    {
        public int Button { get; set; }

        public int Count { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
