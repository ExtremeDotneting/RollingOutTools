using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RollingOutTools.ReflectionVisit
{
    public class JsonToParamsBindings
    {
        public static JsonToParamsBindings Inst { get; } = new JsonToParamsBindings();

        static readonly JsonSerializer _jsonSerializer;

        static JsonToParamsBindings()
        {
            _jsonSerializer = JsonSerializer.Create();
        }

        /// <summary>
        /// Получает набор параметров из json массива. При этом соответствие устанавливается по порядковому номеру.
        /// </summary>
        /// <param name="ignoreErrors">Если истина, то при ошибках десереализации параметрам будет 
        /// задано значение по-умолчанию в соответствии с их типом.</param>
        public List<object> ResolveFromArray(string jsonArrayStr, IEnumerable<Type> paramTypes,
            bool ignoreErrors = true, JsonSerializer jsonSerializer = null)
        {
            List<object> res = new List<object>();
            var jToken = GetJToken(jsonArrayStr, jsonSerializer);

            int i = 0;
            foreach (var paramType in paramTypes)
            {
                object currentValue = null;
                try
                {
                    currentValue = jToken[i].ToObject(paramType);
                }
                catch
                {
                    if (ignoreErrors)
                    {
                        if (paramType.IsValueType)
                        {
                            currentValue = Activator.CreateInstance(paramType);
                        }

                    }
                    else
                    {
                        throw;
                    }
                }
                res.Add(currentValue);
                i++;
            }
            return res;
        }

        /// <summary>
        /// Получает набор параметров из json массива. При этом соответствие устанавливается по порядковому номеру.
        /// </summary>
        /// <param name="ignoreErrors">Если истина, то при ошибках десереализации параметрам будет 
        /// задано значение по-умолчанию в соответствии с их типом.</param>
        public List<object> ResolveFromArray(string jsonArrayStr, IEnumerable<Parameter> parameters,
            bool ignoreErrors = true, JsonSerializer jsonSerializer = null)
        {
            var paramTypes = new List<Type>();
            foreach(var parameter in parameters)
            {
                paramTypes.Add(parameter.ParamType);
            }
            return ResolveFromArray(jsonArrayStr,paramTypes, ignoreErrors, jsonSerializer);
        }

        /// <summary>
        /// Получает набор параметров из комплексного json объекта.
        /// Т.к. каждый парметр подбирается в соответствии с именем, то этот метод медленнее, но более точный.
        /// </summary>
        /// <param name="ignoreErrors">Если истина, то при ошибках десереализации параметрам будет 
        /// задано значение по-умолчанию в соответствии с их типом.</param>
        public List<object> ResolveFromComplexObj(string jsonComplexObjStr, IEnumerable<Parameter> parameters,
            bool ignoreErrors = true, JsonSerializer jsonSerializer = null)
        {
            List<object> res = new List<object>();
            var jToken = GetJToken(jsonComplexObjStr, jsonSerializer);
            int i = 0;
            foreach (var parameter in parameters)
            {
                var paramType = parameter.ParamType;
                object currentValue = null;
                try
                {
                    currentValue = jToken[parameter.ParamName].ToObject(paramType);
                }
                catch
                {
                    if (ignoreErrors)
                    {
                        if (paramType.IsValueType)
                        {
                            currentValue = Activator.CreateInstance(paramType);
                        }

                    }
                    else
                    {
                        throw;
                    }
                }
                res.Add(currentValue);
                i++;
            }
            return res;
        }

        JToken GetJToken(string jsonArrayStr, JsonSerializer jsonSerializer)
        {
            if (jsonSerializer == null)
            {
                jsonSerializer = _jsonSerializer;
            }
            JToken jToken = jsonSerializer.Deserialize<JToken>(new JsonTextReader(new StringReader(jsonArrayStr)));
            return jToken;
        }
    }
}
