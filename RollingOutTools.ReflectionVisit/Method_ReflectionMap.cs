using System;
using System.Reflection;
using System.Threading.Tasks;

namespace RollingOutTools.ReflectionVisit
{
    class Method_ReflectionMap : IMethod_ReflectionMap
    {
        public Parameter[] Parameters { get; internal set; }
        public Type ReturnType { get; internal set; }

        public MethodKind Kind { get; internal set; } = MethodKind.DefMethod;
        public bool IsAsync { get; internal set; }

        internal Func<object, object[], object> InvokeAction { get;  set; }

        /// <summary>
        /// Delegate to get Task.Result from method returned task (if it is task).
        /// </summary>
        internal MethodInfo ReflectionToGetResultFromGenericTask { get; set; }

        /// <summary>
        /// Return result from called method without any manipulations on result.
        /// </summary>
        public object Execute(object globalInstance, object[] parameters)
        {
            return InvokeAction(globalInstance, parameters);
        }

        /// <summary>
        /// If method return task, this method will await it.
        /// </summary>
        public async Task<object> ExecuteAndAwait(object globalInstance, object[] parameters)
        {
            Task taskFromMethodResult= (Task)InvokeAction(globalInstance, parameters);
            await taskFromMethodResult;
            return ReflectionToGetResultFromGenericTask.Invoke(taskFromMethodResult,new object[0]);
        }
    }
}
