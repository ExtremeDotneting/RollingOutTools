using RollingOutTools.Common;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RollingOutTools.ReflectionVisit
{

    public class ReflectionMap
    {
        ReflectionMap(string splitter)
        {
            _splitter = splitter;
        }

        string _splitter;

        /// <summary>
        /// Тип по которому был построен ReflectionMap.
        /// </summary>
        public Type InspectedType { get; private set; }

        Dictionary<string, IMethod_ReflectionMap> _longNameAndMethod = new Dictionary<string, IMethod_ReflectionMap>();
        Dictionary<string, IInfo_ReflectionMap> _longNameAndInfo = new Dictionary<string, IInfo_ReflectionMap>();

        public Dictionary<string, IMethod_ReflectionMap> LongNameAndMethod => _longNameAndMethod;
        public Dictionary<string, IInfo_ReflectionMap> LongNameAndInfo => _longNameAndInfo;


        /// <summary>
        /// Возвращает объект по типу MethodInfo в стандартной рефлексии, но без лишних методов и с существенным отличием.
        /// ВЫ ДОЛЖНЫ ПЕРЕДАВАТЬ ИМЕННО InspectedType из ReflectionMap при вызове методов, а остальной путь к свойству будет проложен автоматически.
        /// </summary>
        /// <param name="longNameOfPropOrMethod">Имя свойства по типу 'user.get'. Обратите внимание, 
        /// что для именования теперь будет использоваться underscope или то имя, что вы указали в атрибуте.
        /// Обязательно вручную указывайте разные имена для методов с перегрузкой.</param>
        public IMethod_ReflectionMap GetMethod(string longNameOfPropOrMethod)
        {
            return _longNameAndMethod[longNameOfPropOrMethod];
        }

        /// <summary>
        /// Возвращает информацию о свойстве или методе из списка.
        /// </summary>
        public IInfo_ReflectionMap GetInfo(string longNameOfPropOrMethod)
        {
            return _longNameAndInfo[longNameOfPropOrMethod];
        }

        /// <summary>
        /// Кроме как для удобного просмотра карты всех апи этот метод ни для чего не служит.
        /// </summary>
        public string AsString()
        {
            StringBuilder res = new StringBuilder();
            foreach (var item in _longNameAndInfo)
            {
                string newStr = item.Key;
                if (_longNameAndMethod.ContainsKey(item.Key))
                {
                    newStr += "(";
                    bool isFirst = true;
                    foreach (var parameter in _longNameAndMethod[item.Key].Parameters)
                    {
                        if (!isFirst)
                        {
                            newStr += ", ";

                        }
                        isFirst = false;
                        newStr += parameter.ParamType.Name + "  " 
                            + TextExtensions.ToUnderscoreCase(parameter.ParamName);
                    }
                    newStr += ")";
                }
                newStr += ";";
                if (item.Value.Description != null)
                {
                    newStr += $"  /*{item.Value.Description}*/";
                }
                res.AppendLine(newStr);
            }
            return res.ToString();
        }

        /// <summary>
        /// Создает ReflectionMap для типа inspectedType. 
        /// После этого вы можете использовать этот объект для вызовов методов класса при помощи выражений типа 'user.get'.
        /// Не забудьте, что свойства и методы должны быть помечены одним из атрибутов.
        /// Кстати, вы всегда должны полностью указывать путь к методу или подсвойтву, вы не можете получить часть ReflevtionMap. 
        /// Это было сделано осознанно, такой подход больше схож с настоящим RestApi, где вы не можете получить часть апи 
        /// в виде объекта записанного в свойство (и тут не можете).
        /// </summary>
        public static ReflectionMap CreateMap(Type inspectedType,string splitter= "/")
        {
            var res = new ReflectionMap(splitter);
            res.InspectedType = inspectedType;
            Func<object, object> first_funcToGetLocalInstanceFromGlobal = (globalInst) =>
            {
                return globalInst;
            };

            var classInfoAttr= inspectedType.GetCustomAttribute<ClassInfo_ReflectionMapAttribute>();
            string prefix = (classInfoAttr==null || classInfoAttr.Prefix==null) ? "" : (classInfoAttr.Prefix + splitter) ;

            //Ищем методы
            res.inspectMetods(
                prefix, "",
                inspectedType,
                first_funcToGetLocalInstanceFromGlobal
                );

            //Ищем простые свойства
            res.inspectSimpleProps(
                prefix, "",
                inspectedType,
                first_funcToGetLocalInstanceFromGlobal
                );

            //Ищем свойства-категории
            res.inspectCategoryProps(
                prefix, "",
                inspectedType,
                first_funcToGetLocalInstanceFromGlobal
                );
            return res;
        }

        /// <summary>
        /// Ищем методы.
        /// </summary>
        /// <param name="t">Исследуемый тип, не путать с InspectedType, ведь с погружением в его поля это будет их тип, соответственно.</param>
        /// <param name="funcToGetLocalInstanceFromGlobal">Делегат для получения объекта с типом t из InspectedType.</param>
        void inspectMetods(string prefix, string realNamePrefix, Type t, Func<object, object> funcToGetLocalInstanceFromGlobal)
        {
            var methodInfoList = getAllMethods(t);

            foreach (var item in methodInfoList)
            {
                var attr = item.GetCustomAttribute(typeof(Method_ReflectionMapAttribute)) as Method_ReflectionMapAttribute;
                if (attr != null)
                {
                    var newInfo = new Info_ReflectionMap()
                    {
                        DisplayName = prefix + (attr.DisplayName ?? TextExtensions.ToUnderscoreCase(item.Name)),
                        RealName = realNamePrefix + item.Name,
                        Description = attr.Description,
                    };

                    var newMethod = new Method_ReflectionMap()
                    {
                        Parameters = parameterInfoArrayToParamsArray(item.GetParameters()),
                        ReturnType=item.ReturnType
                    };
                    newMethod.InvokeAction = (globalInst, parameters) =>
                    {
                        var locInst = funcToGetLocalInstanceFromGlobal(globalInst);
                        return item.Invoke(locInst, parameters);
                    };
                    checkIfTaskAndDoDirtWork(item, newMethod);


                    _longNameAndInfo.Add(
                       newInfo.DisplayName,
                       newInfo
                       );

                    _longNameAndMethod.Add(
                        newInfo.DisplayName,
                        newMethod
                        );
                }
            }

        }

        Parameter[] parameterInfoArrayToParamsArray(ParameterInfo[] arr)
        {
            var resArr = new Parameter[arr.Length];
            for(int i = 0; i < arr.Length; i++)
            {
                resArr[i] = new Parameter()
                {
                    ParamName = arr[i].Name,
                    ParamType = arr[i].ParameterType
                };
            }
            return resArr;
        }

        /// <summary>
        /// Default reflection method don`t return methods of interfaces, that type implement. This method can do it.
        /// </summary>
        List<MethodInfo> getAllMethods(Type t)
        {

            List<MethodInfo> methodInfoList = new List<MethodInfo>();
            methodInfoList.AddRange(t.GetMethods());

            foreach (var interfaceType in t.GetInterfaces())
            {
                methodInfoList.AddRange(interfaceType.GetMethods());
                
            }
            return methodInfoList;
        }

        /// <summary>
        /// Ищем простые свойства (которые будут сконвертированы в методы с приставкой get и set.
        /// </summary>
        /// <param name="t">Исследуемый тип, не путать с InspectedType, ведь с погружением в его поля это будет их тип, соответственно.</param>
        /// <param name="funcToGetLocalInstanceFromGlobal">Делегат для получения объекта с типом t из InspectedType.</param>
        void inspectSimpleProps(string prefix, string realNamePrefix, Type t, Func<object, object> funcToGetLocalInstanceFromGlobal)
        {
            //Dictionary<string, MethodInfo_ReflectionMap> res =new Dictionary<string, MethodInfo_ReflectionMap>();
            foreach (var item in t.GetProperties())
            {
                var attr = item.GetCustomAttribute(typeof(SimpleProp_ReflectionMapAttribute)) as SimpleProp_ReflectionMapAttribute;
                if (attr != null)
                {
                    if (attr.CanGet && item.CanRead)
                    {
                        var newInfo = new Info_ReflectionMap()
                        {
                            DisplayName = prefix + ("get_" + (attr.DisplayName ?? TextExtensions.ToUnderscoreCase(item.Name))),
                            RealName = realNamePrefix  + item.Name,
                            Description = attr.Description
                        };

                        var newMethod = new Method_ReflectionMap()
                        {
                            Parameters = new Parameter[] { },
                            ReturnType = item.PropertyType,
                            Kind = MethodKind.PropertyGetter
                        };

                        var getter = item.GetMethod;
                        newMethod.InvokeAction = (globalInst, parameters) =>
                        {
                            var locInst = funcToGetLocalInstanceFromGlobal(globalInst);
                            return getter.Invoke(locInst, parameters);
                        };
                        checkIfTaskAndDoDirtWork(getter, newMethod);

                        _longNameAndInfo.Add(
                            newInfo.DisplayName,
                            newInfo
                        );


                        _longNameAndMethod.Add(
                            newInfo.DisplayName,
                            newMethod
                            );
                    }

                    if (attr.CanSet && item.CanWrite)
                    {
                        var newInfo = new Info_ReflectionMap()
                        {
                            DisplayName = prefix + ("set_" + (attr.DisplayName ?? TextExtensions.ToUnderscoreCase(item.Name))),
                            RealName =realNamePrefix + "Set"+item.Name,
                            Description = attr.Description
                        };

                        var param = new Parameter
                        {
                            ParamType = item.PropertyType,
                            ParamName = "val"
                        };

                        var newMethod = new Method_ReflectionMap()
                        {
                            Parameters = new Parameter[] { param },
                            ReturnType = typeof(void),
                            Kind = MethodKind.PropertySetter
                        };


                        var setter = item.SetMethod;
                        newMethod.InvokeAction = (globalInst, parameters) =>
                        {
                            var locInst = funcToGetLocalInstanceFromGlobal(globalInst);
                            return setter.Invoke(locInst, parameters);
                        };
                        checkIfTaskAndDoDirtWork(setter, newMethod);

                        _longNameAndInfo.Add(
                            newInfo.DisplayName,
                            newInfo
                            );


                        _longNameAndMethod.Add(
                            newInfo.DisplayName,
                            newMethod
                            );
                    }
                }
            }

        }

        /// <summary>
        /// Ищем свойства-категории. Все методы из типа свойства будут добавленны в reflection map с префиксом в виде имени свойства (учитывая погружение).
        /// </summary>
        /// <param name="t">Исследуемый тип, не путать с InspectedType, ведь с погружением в его поля это будет их тип, соответственно.</param>
        /// <param name="funcToGetLocalInstanceFromGlobal">Делегат для получения объекта с типом t из InspectedType.</param>
        void inspectCategoryProps(string prefix, string realNamePrefix, Type t, Func<object, object> funcToGetLocalInstanceFromGlobal)
        {
            foreach (var item in t.GetProperties())
            {
                var attr = item.GetCustomAttribute(typeof(CategoryProp_ReflectionMapAttribute)) as CategoryProp_ReflectionMapAttribute;
                if (attr != null)
                {
                    var newInfo = new Info_ReflectionMap()
                    {
                        DisplayName = prefix + (attr.DisplayName ?? TextExtensions.ToUnderscoreCase(item.Name)),
                        RealName= realNamePrefix+ item.Name,
                        Description = attr.Description
                    };

                    _longNameAndInfo.Add(
                        prefix + newInfo.DisplayName,
                        newInfo
                        );

                    var categoryGetter=item.GetMethod;
                    Func<object, object> new_funcToGetLocalInstanceFromGlobal = (globalInst) =>
                    {
                        var parentInst = funcToGetLocalInstanceFromGlobal(globalInst);
                        var res=categoryGetter.Invoke(parentInst, new object[0]);
                        return res;


                    };

                    var newPrefix = prefix + newInfo.DisplayName + _splitter;
                    var newRealNamePrefix = realNamePrefix +newInfo.RealName+".";
                    inspectMetods(
                        newPrefix,
                        newRealNamePrefix,
                        item.PropertyType,
                        new_funcToGetLocalInstanceFromGlobal
                        );

                    inspectSimpleProps(
                        newPrefix,
                        newRealNamePrefix,
                        item.PropertyType,
                        new_funcToGetLocalInstanceFromGlobal
                        );

                    inspectCategoryProps(
                        newPrefix,
                        newRealNamePrefix,
                        item.PropertyType,
                        new_funcToGetLocalInstanceFromGlobal
                        );
                }
            }
        }

        void checkIfTaskAndDoDirtWork(MethodInfo mi, Method_ReflectionMap newMethod)
        {
            if (typeof(Task).IsAssignableFrom(mi.ReturnType))
            {
                newMethod.IsAsync = true;
                newMethod.ReflectionToGetResultFromGenericTask = mi.ReturnType.GetProperty("Result").GetMethod;
            }
        }
    }
}
