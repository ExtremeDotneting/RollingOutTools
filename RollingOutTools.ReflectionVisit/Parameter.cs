using System;
using System.Reflection;

namespace RollingOutTools.ReflectionVisit
{
    public class Parameter
    {
        /// <summary>
        /// Может отличаться от имени из ParameterInfo.
        /// </summary>
        public string ParamName { get; set; }

        public ParameterInfo Info { get; set; }
    }
}
