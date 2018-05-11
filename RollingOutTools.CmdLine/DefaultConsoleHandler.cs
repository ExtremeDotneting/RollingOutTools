using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace RollingOutTools.CmdLine
{
    public class DefaultConsoleHandler : IConsoleHandler
    {
        const string jsonEditorFilePath = "json_editor_buf.json";

        public string ReadJson(string jsonPrototypeString)
        {
            try
            {
                File.WriteAllText(
                        jsonEditorFilePath,
                        jsonPrototypeString
                        );
                Process editorProcess = Process.Start(Environment.CurrentDirectory + "\\" + jsonEditorFilePath);
                editorProcess.WaitForExit();
                return File.ReadAllText(jsonEditorFilePath);
            }
            catch(Exception ex)
            {
                WriteLine($"Was error '{ex.Message}' when try to use json editor. \nBut you can write json string as default.", ConsoleColor.DarkRed);
                WriteLine("Or you can press enter to throw error upper.", ConsoleColor.DarkRed);
                if(!string.IsNullOrWhiteSpace(jsonPrototypeString))
                    WriteLine($"Prototype: {jsonPrototypeString}", ConsoleColor.DarkYellow);
                Write("Input json line: ",null);
                var res= ReadLine();
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

        public string ReadLine()
        {
            return Console.ReadLine();
        }

        public void Write(string str, ConsoleColor? consoleColor)
        {
            if (consoleColor == null)
            {
                Console.Write(str);
                return;
            }
            var current = Console.ForegroundColor;
            Console.ForegroundColor = consoleColor.Value;
            Console.Write(str);
            Console.ForegroundColor = current;
        }

        public void WriteLine(string str, ConsoleColor? consoleColor)
        {
            if (consoleColor==null)
            {
                Console.WriteLine(str);
                return;
            }
            var current = Console.ForegroundColor;
            Console.ForegroundColor = consoleColor.Value;
            Console.WriteLine(str);
            Console.ForegroundColor = current;
        }

        public void WriteLine()
        {
            Console.WriteLine();
        }
    }
}
