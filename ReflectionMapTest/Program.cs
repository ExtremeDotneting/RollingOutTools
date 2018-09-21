using RollingOutTools.ReflectionVisit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReflectionMapTest
{
    class Program
    {
        static ReflectionMap refMap;
        static MainObj inspectedObj;

        static void Main(string[] args)
        {
            refMap = new ReflectionMap(typeof(MainObj), ".", true, "prefixName");
            inspectedObj = new MainObj();
            Test().Wait();
        }

        public static async Task Test()
        {
            string strRepresentation = refMap.AsString();
            Console.WriteLine(strRepresentation);
            Console.ReadLine();

            await TestParamsFromJsonArray();
            await TestParamsFromComplexObj();
            await TestSimpleCall();
            await TestReturnRes();
            
        }

        public static async Task TestSimpleCall()
        {
            var method = refMap.LongNameAndMethod["prefixName.CurrentSubObj.AnotherIncludedObj.ItsToDeep"];
            await method.ExecuteAndAwait(inspectedObj, new object[] { "*invoked param*" });
            Console.ReadLine();
        }

        public static async Task TestReturnRes()
        {
            var methodWithResult = refMap.LongNameAndMethod["prefixName.MyMethod"];
            var res = await methodWithResult.ExecuteAndAwait(inspectedObj, new object[] { "Congratulations" });
            Console.WriteLine(res);
            Console.ReadLine();
        }

        public static async Task TestParamsFromJsonArray()
        {
            var method = refMap.LongNameAndMethod["prefixName.CurrentSubObj.AnotherIncludedObj.Sum"];
            object[] parameters = JsonToParamsBindings.Inst.ResolveFromArray("[10,2]", method.Parameters).ToArray();
            var res = await method.ExecuteAndAwait(inspectedObj, parameters);
            Console.WriteLine("Not all params [10,2]. Res " + res);

            parameters = JsonToParamsBindings.Inst.ResolveFromArray("[10,2,4]", method.Parameters).ToArray();
            res = await method.ExecuteAndAwait(inspectedObj, parameters);
            Console.WriteLine("All params [10,2,4]. Res " + res);

            parameters = JsonToParamsBindings.Inst.ResolveFromArray("[10,'it`s str',4,9,9,9,false]", method.Parameters).ToArray();
            res = await method.ExecuteAndAwait(inspectedObj, parameters);
            Console.WriteLine("Too much params and wrong type of second param [10,'it`s str',4,9,9,9,false]. Res " + res);
            Console.ReadLine();
        }

        public static async Task TestParamsFromComplexObj()
        {
            var jsonStr="{\"notMyParam\":\"it`s string\",\"num1\":5,\"num2\":6,\"num3\":4}";
            var method = refMap.LongNameAndMethod["prefixName.CurrentSubObj.AnotherIncludedObj.Sum"];
            object[] parameters = JsonToParamsBindings.Inst.ResolveFromComplexObj(
                jsonStr, 
                method.Parameters).ToArray();
            var res = await method.ExecuteAndAwait(inspectedObj, parameters);
            Console.WriteLine("String with complex object and not sorted fields.");
            Console.WriteLine(jsonStr);
            Console.WriteLine("Res " + res);

            Console.ReadLine();
        }


    }
}
