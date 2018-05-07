using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;

namespace RollingOutTools.PureApi.AspNetCore.Json
{
    public class PureApiJsonModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (!PureApiAspNetCoreJsonBootstrapper.IsInit)
            {
                throw new Exception($"It seems you forgot to call {nameof(PureApiAspNetCoreJsonBootstrapper)} configuring method.");
            }
            if (context.BindingInfo.BinderType?.IsAssignableFrom(typeof(PureApiJsonModelBinder)) ==true)
                return new PureApiJsonModelBinder(context.Metadata.ModelType);
            return null;
        }
    }

}
