using System.Diagnostics;

namespace RollingOutTools.Common
{
    public static class ProcessExtension
    {
        public static void StartNetCore(this Process process, string path)
        {
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.FileName = ".\\" + path;
            process.Start();
        }
    }
}
