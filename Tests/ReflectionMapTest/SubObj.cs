using RollingOutTools.ReflectionVisit;
using RollingOutTools.ReflectionVisit.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReflectionMapTest
{
    class SubObj
    {
        [IncludedObjReflectionMap]
        public SubSubObj AnotherIncludedObj { get; } = new SubSubObj();

        [MethodReflectionMap]
        public void HiWorld(int param)
        {

        }

        [MethodReflectionMap]
        public void Foo(DateTime dtParam, string strParam, bool boolParam, int intParam)
        {

        }
    }
}
