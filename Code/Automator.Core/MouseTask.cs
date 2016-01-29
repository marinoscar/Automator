using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automator.Core
{
    public class MouseTask : ITask, IMouseData
    {
        public static MouseTask FromData (IMouseData data)
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

        public void Execute()
        {
            MousePlayer.Instance.DoMouseClick(this);
        }
    }
}
