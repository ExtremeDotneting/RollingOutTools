using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace RollingOutTools.ReflectionVisit
{
    public static class ReflectionVisitHelpers
    {
        public static string GetNormalTypeName(this Type t)
        {
            if (t.IsGenericType)
            {
                string name = t.Namespace + ".";
                //Remove generic declaration part
                name += t.Name.Remove(t.Name.IndexOf("`")) + "<";
                var genericArgs = t.GetGenericArguments();
                var firstArg = genericArgs[0];
                name += GetNormalTypeName(firstArg);
                for (int i = 1; i < genericArgs.Length; i++)
                {
                    name += ", " + GetNormalTypeName(genericArgs[i]);
                }
                name += ">";
                return name;
            }
            else
            {
                return t.Namespace + "." + t.Name;
            }
        }

        public static object CreateDefaultValue(this Type t)
        {
            if (t.IsValueType)
            {
                return Activator.CreateInstance(t);
            }
            else
            {
                return null;
            }
        }

        public static IEnumerable<ParameterInfo> ToParamInfo(this IEnumerable<Parameter> parameters)
        {
            return parameters.Select(x => x.Info);
        }

        public static IEnumerable<Parameter> ToParam(this IEnumerable<ParameterInfo> parameters)
        {
            return parameters.Select(x => new Parameter
            {
                ParamName = x.Name,
                Info = x
            });
        }

        public static bool IsNumericType(this Type t)
        {
            switch (Type.GetTypeCode(t))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }
    }
}
