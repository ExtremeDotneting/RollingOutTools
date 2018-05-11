using System;

namespace RollingOutTools.CmdLine.RemoteConsole.Client
{
    class RequestModel
    {
        public string Str { get; set; }
        public ConsoleMethod Method { get; set; }
        public ConsoleColor? Color { get; set; }
        public string UniqId { get; set; }
    }
}
