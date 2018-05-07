using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Threading.Tasks;

namespace RollingOutTools.PureApi.AspNetCore.Json
{
    public class PureApiJsonModelBinder : BasePureApiModelBinder
    {
        internal static JsonSerializer JsonSerializerProp { get; set; }
        const string ParamNameForItemsBuff="_parsedJsonToJToken";

        public PureApiJsonModelBinder(Type modelType) : base(modelType)
        {
        }

        public override async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext.Result.IsModelSet)
                return;            
            try
            {        
                if (CheckIfParameterIsMarked(bindingContext))
                {
                    //processing here
                    JToken jToken = _FindJToken(bindingContext);                

                    //searching parameter in jToken
                    if (jToken[ParamName] == null)
                    {
                        throw new Exception($"Can`t find parameter {ParamName} in json body.");
                    }
                    object resultValue = jToken[ParamName].ToObject(ParameterType);
                    bindingContext.Result = ModelBindingResult.Success(resultValue);
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
            //return Task.FromResult<object>(null);
        }

        JToken _FindJToken(ModelBindingContext bindingContext)
        {
            var httpCtx = bindingContext.HttpContext;
            var req = bindingContext.HttpContext.Request;
            JToken jToken = null;

            if (httpCtx.Items.ContainsKey(ParamNameForItemsBuff))
            {
                //if json body was parsed for previous parameters.
                jToken = (JToken)httpCtx.Items[ParamNameForItemsBuff];
            }
            else
            {
                //if it is first parameter with attribute in current controller method.
                if (req.Method == "GET")
                {
                    //on get
                    jToken = _ResolveFromGetRequest(bindingContext);
                }
                else if (req.Method == "POST")
                {
                    //on post
                    if (req.ContentType == "application/json")
                    {
                        //on json body
                        jToken = _ResolveFromPostJsonBody(bindingContext);
                    }
                    else if (req.ContentType == "application/x-www-form-urlencoded")
                    {
                        //on form body   
                        jToken = _ResolveFromPostFormParameter(bindingContext);
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
            }

            return jToken;
        }

        JToken _ResolveFromGetRequest(ModelBindingContext bindingContext)
        {
            JToken jToken = null;
            //on get
            if (bindingContext.ActionContext.RouteData.Values.ContainsKey(NameOfPureApiContainerParameter))
            {
                jToken = _DeserializeAndSaveToBuf(
                    (string)bindingContext.ActionContext.RouteData.Values[NameOfPureApiContainerParameter],
                    bindingContext.HttpContext
                    );
                
            }
            else if (bindingContext.HttpContext.Request.Query.ContainsKey(NameOfPureApiContainerParameter))
            {
                _DeserializeAndSaveToBuf(
                   bindingContext.HttpContext.Request.Query[NameOfPureApiContainerParameter],
                   bindingContext.HttpContext
                   );
            }
            else
            {
                throw new Exception(
                    $"Can`t find PureApi json object parameter {NameOfPureApiContainerParameter} in route or query ."
                    );
            }
            
            return jToken;
        }

        JToken _ResolveFromPostJsonBody(ModelBindingContext bindingContext)
        {
            //values from all body as json            
            var httpCtx = bindingContext.HttpContext;
            var req = bindingContext.HttpContext.Request;
            string bodyStr;
            using (var reader = new StreamReader(req.Body))
            {
                bodyStr = reader.ReadToEnd();
            };
            var jToken = _DeserializeAndSaveToBuf(bodyStr, httpCtx);
            using (JsonReader reader = new JsonTextReader(new StringReader(bodyStr)))
            {
                jToken = JsonSerializerProp.Deserialize<JToken>(reader);
                httpCtx.Items[ParamNameForItemsBuff] = jToken;
            };
            return jToken;
        }

        JToken _ResolveFromPostFormParameter(ModelBindingContext bindingContext)
        {
            var httpCtx = bindingContext.HttpContext;
            var req = bindingContext.HttpContext.Request;

            //values from form field (strings are json)
            if (!httpCtx.Request.Form.ContainsKey(NameOfPureApiContainerParameter))
            {
                throw new Exception(
                    $"Can`t find PureApi json container parameter in form value {NameOfPureApiContainerParameter}."
                    );
            }
            var jToken = _DeserializeAndSaveToBuf(
                httpCtx.Request.Form[NameOfPureApiContainerParameter],
                httpCtx
                );
            return jToken;
        }

        JToken _DeserializeAndSaveToBuf(string jsonStr, HttpContext httpContext)
        {
            using (JsonReader reader = new JsonTextReader(new StringReader(jsonStr)))
            {
                JToken jToken = JsonSerializerProp.Deserialize<JToken>(reader);
                httpContext.Items[ParamNameForItemsBuff] = jToken;
                return jToken;
            };
        }
    }
}
