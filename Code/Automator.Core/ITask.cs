using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Automator.Core
{
    public interface ITask
    {
        [Browsable(false)]
        string TaskName { get; }
        [Browsable(false)]
        string TaskCaption { get; }
        void Execute();
    }
}
