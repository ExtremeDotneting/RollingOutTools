using Microsoft.AspNetCore.Mvc;
using System;

namespace RollingOutTools.PureApi.AspNetCore
{
    /// <summary>
    /// Search first modelbinder that implementing type BasePureApiModelBinder.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class FromPureApiAttribute : ModelBinderAttribute
    {
        //You can set parameter name here or will be used default name (name of parameter in c#).
        public string ParameterName { get; set; }

        public FromPureApiAttribute():base(typeof(BasePureApiModelBinder))
        {

        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class PureApiMethodSettingsAttribute : Attribute
    {
        /// <summary>
        /// You can set here name of the http parameter, which value will be used for model binding.
        /// Note, that it will be ignored, when you sending all object as body of method (not as just one parameter).
        /// By default it is 'r'.
        /// </summary>
        public string NameGlobalHttpParameter { get; set; } 
    }
}
