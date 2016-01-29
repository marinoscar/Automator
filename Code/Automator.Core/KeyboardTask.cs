using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Automator.Core
{
    public class KeyboardTask : ITask
    {
        public string CommandText { get; set; }

        public void Execute()
        {
            SendKeys.SendWait(CommandText);
        }
    }
}
