using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automator.Core
{
    public class MouseTask : ITask, IMouseData
    {
        public static MouseTask FromData(IMouseData data)
        {
            return new MouseTask()
            {
                Button = data.Button,
                Count = data.Count,
                X = data.X,
                Y = data.Y
            };
        }

        public int Button { get; set; }

        public int Count { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public string TaskCaption { get { return GetName(); } }

        public string TaskName { get { return GetType().Name; } }

        private string GetName()
        {
            if (Count > 1) return "Double Click";
            return Button != 1048576 ? "Right Click" : "Left Click";
        }


        public void Execute()
        {
            MousePlayer.Instance.DoMouseClick(this);
        }
    }
}
