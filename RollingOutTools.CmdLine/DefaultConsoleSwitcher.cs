using RollingOutTools.CmdLine;
using System.Collections.Generic;
using System.Linq;

namespace RollingOutTools.CmdLine
{
    public class DefaultConsoleSwitcher : ICmdSwitcher
    {
        Stack<CommandLineBase> cmdStack = new Stack<CommandLineBase>();
        CommandLineBase currentCmd { get { return cmdStack.First(); } }

        public void ExecuteCmdOnOpened(string cmd)
        {
            currentCmd.ExecuteCmd(cmd);
        }

        public void OpenCmdInStack(CommandLineBase newCmd)
        {
            cmdStack.Push(newCmd);
            currentCmd.OnStart();
        }

        public void RunDefault(CommandLineBase cmdLine)
        {
            OpenCmdInStack(
                cmdLine
            );
            while (cmdStack.Count > 0)
            {
                while (currentCmd.IsInRun)
                {
                    currentCmd.OnEveryLoop();
                }
                cmdStack.Pop();
            }
        }
    }

}
