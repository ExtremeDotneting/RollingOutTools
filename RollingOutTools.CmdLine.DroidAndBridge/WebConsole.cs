using System;
using System.Text.RegularExpressions;

using Android.Webkit;

using SiteToApp.Droid.WebViewWorkers;

using RollingOutTools.CmdLine;
using System.IO;

namespace RollingOutTools.CmdLine.DroidAndBridge
{
    public class WebConsole : IConsoleHandler
    {
        WebView wv;
        string bridgePath;
        string bridgeConsolePath;

        public WebConsole(WebView wv, 
                          string bridgePath = "bridge.js", 
                          string bridgeConsolePath = "bridge.console.js")
        {
            this.wv = wv;
            this.bridgePath = bridgePath;
            this.bridgeConsolePath = bridgeConsolePath;
        }

        public void Write(string str, ConsoleColor? consoleColor)
        {
            throw new NotImplementedException();
            
        }

        public void WriteLine(string str, ConsoleColor? consoleColor)
        {
            bool isBridgeLoaded = (bool)WebViewExtensions.ExJsWithResult(wv, "try{if(Bridge){true;}}catch(err){false;}").Result;
            if(!isBridgeLoaded)
            {
                WebViewExtensions.ExJsWithResult(wv, File.ReadAllText(bridgePath));
                WebViewExtensions.ExJsWithResult(wv, File.ReadAllText(bridgeConsolePath));
            }
            str = Regex.Escape(str);
            WebViewExtensions.ExJsWithResult(wv, "Bridge.Console.log('"+str+"')");
        }

        public void WriteLine()
        {
            bool isBridgeLoaded = (bool)WebViewExtensions.ExJsWithResult(wv, "try{if(Bridge){true;}}catch(err){false;}").Result;
            if (!isBridgeLoaded)
            {
                WebViewExtensions.ExJsWithResult(wv, File.ReadAllText(bridgePath));
                WebViewExtensions.ExJsWithResult(wv, File.ReadAllText(bridgeConsolePath));
            }
            WebViewExtensions.ExJsWithResult(wv, "Bridge.Console.log()");
        }

        public string ReadJson(string jsonPrototypeString)
        {
            throw new NotImplementedException();
        }

        public string ReadLine()
        {
            throw new NotImplementedException();
        }
    }
}
