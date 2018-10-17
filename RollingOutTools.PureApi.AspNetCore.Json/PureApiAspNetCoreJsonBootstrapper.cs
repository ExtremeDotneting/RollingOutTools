using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace RollingOutTools.PureApi.AspNetCore.Json
{
    public static class PureApiAspNetCoreJsonBootstrapper
    {
        internal static bool IsInit{get;private set;}

        /// <summary>
        /// Only for debug.
        /// </summary>
        public static bool ThrowExceptions { get; set; }=false;

        /// <summary>
        /// Use settings that you pass in parameters.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="jsonSettings"></param>
        /// <returns></returns>
        public static IMvcBuilder AddPureApiJson(this IMvcBuilder builder,JsonSerializerSettings jsonSettings=null)
        {
            if (IsInit)
                throw new System.Exception("Pure api json was initialized before.");
            if (jsonSettings == null)
            {
                builder.AddJsonOptions(jsonOpts =>
                {
                    jsonSettings = jsonOpts.SerializerSettings;
                });
            }
            PureApiJsonModelBinder.JsonSerializerProp = JsonSerializer.Create(jsonSettings);
            builder.AddMvcOptions(options =>
            {
                options.ModelBinderProviders.Insert(0, new PureApiJsonModelBinderProvider());
            });            
            
            IsInit = true;
            return builder;
        }


    }
}
