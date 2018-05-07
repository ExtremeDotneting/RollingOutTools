using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace RollingOutTools.Json
{
    public class JsonSerializeOptions
    {
        public bool WithNormalFormating { get; set; }
        public bool IgnoreDefaultValues { get; set; }
    }
}
