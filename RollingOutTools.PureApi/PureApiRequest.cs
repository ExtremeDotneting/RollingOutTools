using System;
using System.Collections.Generic;
using System.Text;

namespace RollingOutTools.PureApi
{
    public class PureApiRequest : BasePureApiRequest
    {
        /// <summary>
        /// Parameter name and its value.
        /// </summary>
        public Dictionary<string, object> Parameters { get; set; }
    }
}
