using Nancy;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RollingOutTools.CmdLine.RemoteConsole.Server
{
    public class RemoteConsoleWebController : NancyModule
    {
        public RemoteConsoleWebController()
        {
            Post["/isconsole"] = Get["/isconsole"] = (data) =>
              {
                  return "is_ok";
              };

            Post["/dowork"] = Get["/dowork"] = (data) =>
            {
                string json = Request.Form["json"];
                var rm = Newtonsoft.Json.JsonConvert.DeserializeObject<RequestModel>(json);
                DoWork(rm);
                return "is_ok";
            };

            Post["/readres"] = Get["/readres"] = (data) =>
            {
                try
                {
                    string uniqId = Request.Form["uniqId"];
                    if (_readResults.ContainsKey(uniqId))
                    {
                        var res = _readResults[uniqId];
                        //_readResults.Remove(uniqId);
                        return res;
                    }
                    else
                    {
                        return HttpStatusCode.NoContent;
                    }
                }
                catch
                {
                    return HttpStatusCode.NoContent;
                }
            };
        }

        static Dictionary<string, string> _readResults = new Dictionary<string, string>();
        static IConsoleHandler _consoleHandler = new DefaultConsoleHandler();
        static object Locker = new object();
        static void DoWork(RequestModel rm)
        {
            if (rm.Method == ConsoleMethod.WriteLine)
            {
                _consoleHandler.WriteLine(rm.Str, rm.Color);
            }
            else if (rm.Method == ConsoleMethod.Write)
            {
                _consoleHandler.Write(rm.Str, rm.Color);
            }
            else if (rm.Method == ConsoleMethod.ReadLine)
            {
                _readResults[rm.UniqId] = _consoleHandler.ReadLine();
            }
            else if (rm.Method == ConsoleMethod.WriteLineEmpty)
            {
                _consoleHandler.WriteLine();
            }
            else if (rm.Method == ConsoleMethod.ReadJson)
            {
                _readResults[rm.UniqId] = _consoleHandler.ReadJson(rm.Str);
            }
        }
    }
}
