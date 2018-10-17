using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

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
                return new PureApiJsonModelBinder(
                    context.Metadata.ModelType,
                    CreateDefaultBinder(context)
                    );
            return null;
        }

        IModelBinder CreateDefaultBinder(ModelBinderProviderContext context)
        {
            var defaultBindersProvider=context.Services.GetRequiredService<IOptions<MvcOptions>>().Value.ModelBinderProviders;
            foreach(var binderPr in defaultBindersProvider)
            {
                if (binderPr is PureApiJsonModelBinderProvider)
                    continue;
                try
                {
                    var binder = binderPr.GetBinder(context);
                    if (binder != null)
                        return binder;
                }
                catch { }
            }
            return null;
        }
    }

}
