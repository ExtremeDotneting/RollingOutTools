using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace RollingOutTools.ReflectionVisit
{
    class ReflectionMapMethod : IReflectionMapMethod
    {
        /// <summary>
        /// Cache of MethodInfo to get generic task result.
        /// </summary>
        static ConcurrentDictionary<Type, MethodInfo> TaskReflectionGetResultCache { get; } = new ConcurrentDictionary<Type, MethodInfo>();

        public string DisplayName { get; internal set; }

        public string RealName { get; internal set; }

        public string Description { get; internal set; }

        public IReadOnlyCollection<Parameter> Parameters { get; internal set; }

        public Type ReturnType { get; internal set; }

        public MethodKind Kind { get; internal set; } = MethodKind.DefMethod;

        internal Func<object, object[], object> InvokeAction { get;  set; }

        /// <summary>
        /// Return result from called method without any manipulations on result.
        /// </summary>
        public object Execute(object globalInstance, object[] parameters)
        {
            return InvokeAction(globalInstance, parameters);
        }

        /// <summary>
        /// If method return task, this method will await it.
        /// If it doesn`t - result object will be wrapped in Task for more simple usage.
        /// </summary>
        public async Task<object> ExecuteAndAwait(object globalInstance, object[] parameters)
        {
            object invokeRes = Execute(globalInstance, parameters);
            var task = invokeRes as Task;
            if (task == null)
            {
                return invokeRes;
            }
            else
            {
                await task;
                return GetResultOrNullFromTask(task);
            }            
        }

        object GetResultOrNullFromTask(Task task)
        {
            try
            {
                var type = task.GetType();
                MethodInfo getMethod = null;
                TaskReflectionGetResultCache.TryGetValue(type, out getMethod);
                if (getMethod == null)
                {
                    getMethod = type.GetProperty("Result").GetMethod;
                    TaskReflectionGetResultCache.TryAdd(type, getMethod);
                }                
                var res = getMethod.Invoke(task, new object[0]);
                return res;
            }
            catch
            {
                return null;
            }
        }
    }
}
