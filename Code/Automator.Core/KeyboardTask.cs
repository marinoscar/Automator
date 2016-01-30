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
            SendKeys.Send(CommandText);
        }

        public string TaskCaption { get { return string.Format("Send Keys {0}", GetSummaryText()); } }

        public string TaskName { get { return GetType().Name; } }

        private string GetSummaryText()
        {
            if (CommandText.Length <= 15) return CommandText;
            return string.Format("{0}...", CommandText.Substring(0, 13));
        }
    }
}
