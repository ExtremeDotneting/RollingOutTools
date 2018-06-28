using System;
using System.Text.RegularExpressions;
using Android.Webkit;
using RollingOutTools.CmdLine;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Reflection;

namespace RollingOutTools.CmdLine.DroidAndBridge
{
    public class DroidAndBridgeConsoleHandler : IConsoleHandler
    {
        public bool ThrowExceptions { get; set; }
        public bool Enabled { get; set; } = true;

        WebView _wv;
        AutoResetEvent are = new AutoResetEvent(true);

        public DroidAndBridgeConsoleHandler(WebView wv)
        {
            _wv = wv;
        }

        public void Write(string str, ConsoleColor? consoleColor)
        {
            WriteLine(str, consoleColor);

        }

        public void WriteLine(string str, ConsoleColor? consoleColor)
        {
            //Make it async
            Task.Run(() =>
            {
                //Synchronized inside, no need to await.
                _WriteLine(str, consoleColor);
            });

        }

        public void WriteLine()
        {
            WriteLine("", null);
        }

        public string ReadJson(string jsonPrototypeString)
        {
            //wait are.Set()
            are.WaitOne();
            try
            {
                if (!Enabled)
                    throw new Exception("Can`t read json, console disabled.");
                //implemention here
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                if (ThrowExceptions)
                    throw;
            }
            finally
            {
                //make it free
                are.Set();
            }
            return null;
        }

        public string ReadLine()
        {
            //wait are.Set()
            are.WaitOne();
            try
            {
                if (!Enabled)
                    throw new Exception("Can`t read, console disabled.");
                //implemention here
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                if (ThrowExceptions)
                    throw;
            }
            finally
            {
                //make it free
                are.Set();
            }
            return null;
        }

        async Task _WriteLine(string str, ConsoleColor? consoleColor)
        {
            //wait are.Set()
            are.WaitOne();
            try
            {
                if (!Enabled)
                    return;
                await CheckBridgeInvoked();
                str = Regex.Escape(str);
                await _wv.ExJsWithResult("Bridge.Console.log('" + str + "');");
            }
            catch (Exception ex)
            {
                if (ThrowExceptions)
                    throw;
            }
            finally
            {
                //make it free
                are.Set();
            }
        }

        async Task CheckBridgeInvoked()
        {
            //implemention here
            var isBridgeLoadedStr = (await _wv.ExJsWithResult("try{if(Bridge){'1';}else{'0';};}catch(err){'0';}")).ToString();
            bool isBridgeLoaded = isBridgeLoadedStr == "1";
            if (!isBridgeLoaded)
            {
                string initScript = await ReadBridgeJsFromFile();
                await _wv.ExJsWithResult(initScript);
            }
        }

        async Task<string> ReadBridgeJsFromFile()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "RollingOutTools.CmdLine.DroidAndBridge.console_script.js";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}
