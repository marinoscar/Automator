using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automator.Core
{
    public interface IMouseData
    {
        int X { get; }
        int Y { get; }
        int Button { get; }
        int Count { get; }
    }
}
