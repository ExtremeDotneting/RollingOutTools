using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace RollingOutTools.SimpleIoc
{
    public static class IocGlob 
    {
        public static bool IsBuilded { get; private set; }

        static IIocBuilder _builder;
        static IIocBuilder Builder
        {
            get
            {
                if (IsBuilded)
                {
                    throw new Exception("You can`t use builder when ioc container was builded.");
                }
                if (_builder == null)
                {
                    InitBuilder();
                }
                return _builder;
            }
        }

        static ICommonIoc _container;        
        static ICommonIoc Container
        {
            get
            {
                if (!IsBuilded)
                {
                    throw new Exception("You can`t use container before ioc is builded.");
                }
                return _container;
            }
        }

        public static void Build()
        {
            _container=Builder.Build();
            IsBuilded = true;
        }

        public static void RegisterType<TImplemention, TBase>()
            where TImplemention : TBase
         => Builder.RegisterType<TImplemention, TBase>();

        public static object Resolve(Type baseType)
         => Container.Resolve(baseType);

        public static TBase Resolve<TBase>()
         => Container.Resolve<TBase>();

        public static void RegisterInstance<TImplemention, TBase>(TImplemention inst)
            where TImplemention : class, TBase
            where TBase : class
         => Builder.RegisterInstance<TImplemention, TBase>(inst);

        /// <summary>
        /// Search assingnable types in app domain  and return first finded type or throw exception.
        /// </summary>
        /// <param name="assemblyesNames">If null, it will search in current app domain assemblies.</param>
        /// <returns></returns>
        public static Type FindAssignable(Type baseType, string[] assemblyesNames)
        {
            List<Exception> innerExceptions=null;
            List<Assembly> asms;
            if (assemblyesNames == null)
            {
                asms = AppDomain.CurrentDomain.GetAssemblies().ToList();
            }
            else
            {
                asms = new List<Assembly>();
                for (int i = 0; i < assemblyesNames.Length; i++)
                {
                    try
                    {
                        var asm = Assembly.LoadFile(
                            Path.Combine(
                                Environment.CurrentDirectory,
                                $"{assemblyesNames[i]}.dll"
                                )
                            );

                        asms.Add(asm);
                    }
                    catch(Exception ex)
                    {
                        if (innerExceptions == null)
                            innerExceptions = new List<Exception>();
                        innerExceptions.Add(ex);
                    }
                }
            }
            
            var types = asms.SelectMany(s => s.GetTypes())
                .Where(p => baseType.IsAssignableFrom(p) && !p.IsAbstract && p.IsClass).ToList();

            types.Remove(baseType);
            if (types.Any())
            {
                return types.First();
            }
            else
            {
                string msg = $"Can`t find type that implement '{baseType.Name}'. It seems you have forgot to include implementing dll to main project.";
                if (innerExceptions == null)
                {
                    throw new Exception(msg);
                }
                else
                {
                    throw new AggregateException(
                        msg,
                        innerExceptions
                        );
                }
                
            }
        }

        /// <summary>
        /// Search assignable types in app domain  and return first finded type or throw exception.
        /// </summary>
        /// <param name="assemblyesNames">If null, it will search in current app domain assemblies.</param>
        /// <returns></returns>
        public static Type FindAssignable<TBase>(string[] assemblyesNames)
        {
            return FindAssignable(typeof(TBase), assemblyesNames);
        }

        public static TResult CreateTypeInstance<TResult>(Type t)
        {
            ConstructorInfo ctor = t.GetConstructor(new Type[0]);
            if (ctor == null)
            {
                throw new Exception($"Can`t find constructor without parameters for type {t.Name}");
            }
            object instance = ctor.Invoke(new object[] { });
            return (TResult)instance;
        }

        /// <summary>
        /// Manual set handler. Use for fast initialization.
        /// </summary>
        public static void InitBuilder(IIocBuilder builder)
        {
            _builder = builder;
        }

        /// <summary>
        /// Automaticaly find handler.
        /// </summary>
        public static void InitBuilder()
        {
            _builder = CreateTypeInstance<IIocBuilder>(
                FindAssignable<IIocBuilder>(
                    new string[] { "RollingOutTools.SimpleIoc.OnAutofac" }
                    )
                );
        }
    }
}