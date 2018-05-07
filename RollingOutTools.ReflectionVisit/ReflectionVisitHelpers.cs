using System;
using System.Collections.Generic;
using System.Text;

namespace RollingOutTools.ReflectionVisit
{
    public static class ReflectionVisitHelpers
    {
        public static string GetNormalTypeName(Type t)
        {
            if (t.IsGenericType)
            {
                string name = t.Namespace + ".";
                //Remove generic declaration part
                name += t.Name.Remove(t.Name.IndexOf("`"))+"<";
                var genericArgs = t.GetGenericArguments();
                var firstArg = genericArgs[0];
                name += GetNormalTypeName(firstArg);
                for (int i = 1; i < genericArgs.Length; i++)
                {
                    name += ", "+GetNormalTypeName(genericArgs[i]);
                }
                name += ">";
                return name;
            }
            else
            {
                return t.Namespace +"."+ t.Name;
            }
        }
    }
}
