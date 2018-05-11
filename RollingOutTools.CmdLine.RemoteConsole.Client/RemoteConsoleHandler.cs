using Newtonsoft.Json;
using RollingOutTools.Common;
using RollingOutTools.Net;
using RollingOutTools.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RollingOutTools.CmdLine.RemoteConsole.Client
{
    public class RemoteConsoleHandler : IConsoleHandler
    {
        string _consoleServerUrl;
        bool isInit;
        AutoResetEvent initAre = new AutoResetEvent(true);

        public async Task Init(string consoleServerIP = null, int port = 5511)
        {
            string urlPrototype = "http://{0}:{1}";
            if (consoleServerIP != null)
            {
                //If setted by user/
                _consoleServerUrl = string.Format(urlPrototype, consoleServerIP, port);
            }
            if (_consoleServerUrl == null)
            {
                //If have saved url.
                string savedUrl = await Storage_HardDrive.Get<string>("remote_console_url_saved");
                bool isOk = await SendIsConsoleRequest(savedUrl);
                if (await SendIsConsoleRequest(savedUrl))
                {
                    _consoleServerUrl = savedUrl;
                }
            }
            if (_consoleServerUrl == null)
            {
                //If trying find.
                var listOfPossible = NetworkHelpers.GetAllPossibleIPsInLocalNetwork();
                var listOfResponded = await NetworkHelpers.PingAll(listOfPossible, 5);
                foreach (string ipStr in listOfResponded)
                {
                    var ipUrl = string.Format(urlPrototype, ipStr, port);
                    bool isOk = await SendIsConsoleRequest(ipUrl);
                    if (isOk)
                    {
                        _consoleServerUrl = ipUrl;
                        break;
                    }
                }
            }

            if (_consoleServerUrl == null)
            {
                throw new Exception("Can`t find console server url.");
            }
            Storage_HardDrive.Set("remote_console_url_saved", _consoleServerUrl);
            isInit = true;
        }

        public string ReadLine()
        {
            string res = null;
            Task.Run(async () =>
            {
                await Init();
                string generatedCode = TextExtensions.Generate(10);
                var model = new RequestModel()
                {
                    Method = ConsoleMethod.ReadLine,
                    UniqId = generatedCode
                };

                await SendDoWorkRequest(model);

                while (true)
                {
                    res = await SendReadResRequest(generatedCode);
                    if (!string.IsNullOrEmpty(res))
                    {
                        break;
                    }
                    await Task.Delay(300);
                }
            }).Wait();
            return res;
        }

        public void Write(string str, ConsoleColor? consoleColor)
        {
            Task.Run(async () =>
            {
                await InitIfNot();

                var model = new RequestModel()
                {
                    Method = ConsoleMethod.Write,
                    Color = consoleColor,
                    Str = str

                };
                await SendDoWorkRequest(model);
            }).Wait();
        }

        public void WriteLine(string str, ConsoleColor? consoleColor)
        {
            Task.Run(async () =>
            {
                await InitIfNot();
                var model = new RequestModel()
                {
                    Method = ConsoleMethod.WriteLine,
                    Color = consoleColor,
                    Str = str

                };
                await SendDoWorkRequest(model);
            }).Wait();
        }

        public void WriteLine()
        {
            Task.Run(async () =>
            {
                await InitIfNot();
                var model = new RequestModel()
                {
                    Method = ConsoleMethod.WriteLineEmpty
                };
                await SendDoWorkRequest(model);
            }).Wait();
        }

        public string ReadJson(string jsonPrototypeString)
        {
            string res = null;
            Task.Run(async () =>
            {
                await Init();
                string generatedCode = TextExtensions.Generate(10);
                var model = new RequestModel()
                {
                    Method = ConsoleMethod.ReadJson,
                    Str= jsonPrototypeString,
                    UniqId = generatedCode
                };

                await SendDoWorkRequest(model);

                while (true)
                {
                    res = await SendReadResRequest(generatedCode);
                    if (!string.IsNullOrEmpty(res))
                    {
                        break;
                    }
                    await Task.Delay(300);
                }
            }).Wait();
            return res;
        }

        async Task<bool> SendDoWorkRequest(RequestModel reqestModel)
        {
            string json = JsonConvert.SerializeObject(reqestModel);
            var dict = new Dictionary<string, string>() { { "json", json } };
            return "is_ok"==await NetworkHelpers.TrySendPostHttpRequest(_consoleServerUrl+"/dowork", dict);
             

        }

        async Task<string> SendReadResRequest(string uniqId)
        {
            var dict = new Dictionary<string, string>() { { "uniqId", uniqId } };
            var res= await NetworkHelpers.TrySendPostHttpRequest(_consoleServerUrl + "/readres", dict);
            return res;
        }

        async Task<bool> SendIsConsoleRequest(string url)
        {
            var dict = new Dictionary<string, string>();
            return "is_ok" == await NetworkHelpers.TrySendPostHttpRequest(url + "/isconsole", dict);
        }

        async Task InitIfNot()
        {
            initAre.WaitOne();
            if (!isInit)
                await Init();
            initAre.Set();
        }

       
    }
}
