using RollingOutTools.CmdLine;
using RollingOutTools.Storage;
using RollingOutTools.Storage.JsonFileStorage;
using System;

namespace CmdTest
{
    class Program
    {
        static void Main(string[] args)
        {
            StorageHardDrive.InitDependencies(
               new JsonLocalStorage("storage.json")
               );

            //Простейшая консоль с командами из методов классса.
            CmdLineExtension.Init(new DefaultConsoleHandler());
            var cmds = new DefaultConsoleSwitcher();
            cmds.RunDefault(new CmdLineFacade(cmds));
        }
    }
}
