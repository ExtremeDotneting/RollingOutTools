using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace RollingOutTools.PureApi.AspNetCore
{
    public abstract class BasePureApiModelBinder : IModelBinder
    {
        bool? useOnIt;

        protected Type ParameterType{ get; private set; }

        /// <summary>
        /// Name of parmeter from controller method.
        /// </summary>
        protected string ParamName { get; private set; }

        /// <summary>
        /// Name of parmeter, that contains all object with values for pureapi.
        /// </summary>
        protected string NameOfPureApiContainerParameter { get; private set; }="r";

        public BasePureApiModelBinder(Type parameterType)
        {
            ParameterType = parameterType;
        }

        protected bool CheckIfParameterIsMarked(ModelBindingContext bindingContext)
        {
            //If firs time
            if (useOnIt == null)
            {
                ControllerActionDescriptor actionDescriptor = bindingContext.ActionContext.ActionDescriptor as ControllerActionDescriptor;
                //if from controller
                if (actionDescriptor != null)
                {

                    ParameterInfo currentParamInfo = actionDescriptor.MethodInfo.GetParameters()
                        .First(par => par.Name == bindingContext.FieldName);
                    var attrParam = currentParamInfo.GetCustomAttribute<FromPureApiAttribute>();


                    if (attrParam == null)
                    {
                        useOnIt = false;
                    }
                    else
                    {
                        //save metadata here
                        ParamName= attrParam.ParameterName ?? bindingContext.FieldName;
                        var attrMethod = actionDescriptor.MethodInfo.GetCustomAttribute<PureApiMethodSettingsAttribute>();
                        if (attrMethod != null)
                        {
                            NameOfPureApiContainerParameter= attrMethod.NameGlobalHttpParameter;
                        }
                        useOnIt = true;
                    }
                }
                else
                {
                    throw new Exception($"Current model binder can work only with controllers, because it require attributes.");
                }

            }

            return useOnIt.Value;
        }

        public abstract Task BindModelAsync(ModelBindingContext bindingContext);
    }
}
