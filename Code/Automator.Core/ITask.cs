using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Automator.Core
{
    public interface ITask
    {
        string TaskName { get; }
        string TaskCaption { get; }
        void Execute();
    }
}
