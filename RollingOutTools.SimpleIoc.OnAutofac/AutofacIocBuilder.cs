using Autofac;

namespace RollingOutTools.SimpleIoc.OnAutofac
{
    public class AutofacIocBuilder : IIocBuilder
    {
        ContainerBuilder containerBuilder = new ContainerBuilder();

        public void RegisterType<TImplemention, TBase>()
            where TImplemention : TBase
        {
            containerBuilder.RegisterType<TImplemention>().As<TBase>();
        }

        public void RegisterInstance<TImplemention, TBase>(TImplemention inst)
            where TImplemention : class, TBase
            where TBase : class
        {
            containerBuilder.RegisterInstance<TImplemention>(inst).As<TBase>();
        }

        public ICommonIoc Build()
        {
            var container = containerBuilder.Build();
            return new AutofacCommonIoc(container);
        }
    }
}
