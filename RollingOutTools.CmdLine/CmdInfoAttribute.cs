using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RollingOutTools.CmdLine
{
    /// <summary>
    /// Атрибут для методов добавленных в наследниках CommandLineBase. Служит для автоматической генерации cli.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public  class CmdInfoAttribute:Attribute
    {
        public string CmdName { get; set; }
        public string Description { get; set; }
        public bool CanAutorun { get; set; } = true;
        //public bool CmdExecutionOnly { get; set; }
    }
}
