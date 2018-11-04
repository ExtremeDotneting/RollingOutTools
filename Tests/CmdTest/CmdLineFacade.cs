using RollingOutTools.CmdLine;
using RollingOutTools.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace CmdTest
{
    public class CmdLineFacade : CommandLineBase
    {
        public CmdLineFacade(ICmdSwitcher cmdSwitcher, CmdLineExtension cmdLineExtension = null) : base(cmdSwitcher, cmdLineExtension)
        {
        }

        [CmdInfo]
        public void Test1()
        {
            var res=ReadResource<Dictionary<object, object>>("test res");

        }
    }   
}
