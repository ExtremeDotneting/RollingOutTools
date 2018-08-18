using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace RollingOutTools.CmdLine
{
    static class SharedConsoleMethods
    {
        const string jsonEditorFilePath = "json_editor_buf.json";

        public static string ReadJson(string jsonPrototypeString, IConsoleHandler consoleHandler)
        {
            try
            {
                File.WriteAllText(
                        jsonEditorFilePath,
                        jsonPrototypeString
                        );
                Process editorProcess = Process.Start(jsonEditorFilePath);
                editorProcess.WaitForExit();
                return File.ReadAllText(jsonEditorFilePath);
            }
            catch (Exception ex)
            {
                consoleHandler.WriteLine(
                    $"Was error '{ex.Message}' when try to use json editor. \nBut you can write json string as default.", 
                    ConsoleColor.DarkRed
                    );
                consoleHandler.WriteLine("Or you can press enter to throw error upper.", ConsoleColor.DarkRed);
                if (!string.IsNullOrWhiteSpace(jsonPrototypeString))
                    consoleHandler.WriteLine($"Prototype: {jsonPrototypeString}", ConsoleColor.DarkYellow);
                consoleHandler.Write("Input json line: ", null);
                var res = consoleHandler.ReadLine();
                if (string.IsNullOrEmpty(res))
                {
                    throw;
                }
                else
                {
                    return res;
                }
            }
        }
    }
}
