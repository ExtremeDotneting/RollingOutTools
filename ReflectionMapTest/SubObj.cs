﻿using RollingOutTools.ReflectionVisit;
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
    }
}
