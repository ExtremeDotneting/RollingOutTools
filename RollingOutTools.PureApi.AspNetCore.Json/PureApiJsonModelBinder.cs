using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RollingOutTools.PureApi.AspNetCore.Json
{
    public class PureApiJsonModelBinder : BasePureApiModelBinder
    {
        internal static JsonSerializer JsonSerializerProp { get; set; }

        const string ParamNameForItemsBuff="_parsedJsonToJToken";

        IModelBinder _defaultModelBinder;
        
        public PureApiJsonModelBinder(Type modelType, IModelBinder defaultModelBinder) : base(modelType)
        {
            _defaultModelBinder = defaultModelBinder;
        }

        public override async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext.Result.IsModelSet)
                return;            
            try
            {        
                if (CheckIfParameterIsMarked(bindingContext))
                {
                    await ResolveParam(bindingContext);
                }
                else
                {
                    //not our target
                }
            }
            catch (Exception ex)
            {
                var agrEx = new AggregateException(
                    $"Exception in model binder {nameof(PureApiJsonModelBinder)} while processing parameter '{ParamName ?? ""}'.", 
                    ex
                    );

                bindingContext.ModelState.TryAddModelError(
                    bindingContext.ModelName,
                    agrEx,
                    bindingContext.ModelMetadata
                    );
                if (PureApiAspNetCoreJsonBootstrapper.ThrowExceptions)
                    throw agrEx;
            }
        }

        async Task ResolveParam(ModelBindingContext bindingContext)
        {
            var httpCtx = bindingContext.HttpContext;
            var req = bindingContext.HttpContext.Request;
            object res;
            if (req.Method == "POST")
            {
                //on post
                if (req.ContentType.Contains("application/json"))
                {
                    res = ResolveFromPostJsonBody(bindingContext);
                }
                else if (req.HasFormContentType)
                {
                    //Use default model binder for forms data.
                    await _defaultModelBinder.BindModelAsync(bindingContext);                    
                    return;
                }
                else
                {

                    throw new Exception($"Unsapported http post content-type.");       

                }
            }
            else
            {
                throw new Exception($"Unsapported request method. Supported only GET and POST.");
            }

            bindingContext.Result = ModelBindingResult.Success(res);
        }        

        object ResolveFromPostJsonBody(ModelBindingContext bindingContext)
        {
            //values from all body as json            
            var httpCtx = bindingContext.HttpContext;
            var req = bindingContext.HttpContext.Request;
            string bodyStr;

            JToken jToken = null;
            if (httpCtx.Items.ContainsKey(ParamNameForItemsBuff))
            {
                //if json body was parsed for previous parameters.
                jToken = (JToken)httpCtx.Items[ParamNameForItemsBuff];
            }
            else
            {
                using (var reader = new StreamReader(req.Body))
                {
                    bodyStr = reader.ReadToEnd();
                };
                using (JsonReader reader = new JsonTextReader(new StringReader(bodyStr)))
                {
                    jToken = JsonSerializerProp.Deserialize<JToken>(reader);
                    httpCtx.Items[ParamNameForItemsBuff] = jToken;
                };
            }
            object resultValue = jToken[ParamName].ToObject(ParameterType);
            return resultValue;
        }        
    }
}
