using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RollingOutTools.CmdLine
{
    /// <summary>
    /// Интерфейс позволяет открыть CommandLineBase и его наследников в качестве консоли. 
    /// Сделал интерфейс потому-что он используется в методах этих классов и нужна была абстракция от консоли.
    /// </summary>
    public interface ICmdSwitcher
    {
        void OpenCmdInStack(CommandLineBase newCmd);
        void ExecuteCmdOnOpened(string cmd);
    }
}
