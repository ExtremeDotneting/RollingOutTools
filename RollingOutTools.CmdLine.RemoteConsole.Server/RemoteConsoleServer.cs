using Nancy.Hosting.Self;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RollingOutTools.CmdLine.RemoteConsole.Server
{
    public class RemoteConsoleServer
    {
        public static void Run(short port=5511 )
        {
            HostConfiguration hostConfigs = new HostConfiguration()
            {
                UrlReservations = new UrlReservations() { CreateAutomatically = true },
                AllowChunkedEncoding = true,
                
            };

            using (var host = new NancyHost(hostConfigs, new Uri($"http://localhost:{port}")))
            {
                Console.WriteLine("Prepare.");
                host.Start();
                Console.WriteLine($"Running on http://localhost:{port}.");
                AutoResetEvent are = new AutoResetEvent(false);
                are.WaitOne();

        


            }
            

           
        }
    }
}
