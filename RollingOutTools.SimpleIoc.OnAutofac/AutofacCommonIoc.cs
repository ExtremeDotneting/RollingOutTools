using Autofac;
using System;
using System.Collections.Generic;

namespace RollingOutTools.SimpleIoc.OnAutofac
{
    class AutofacCommonIoc : ICommonIoc
    {
        IContainer _container;

        internal AutofacCommonIoc(IContainer container)
        {
            _container = container;
        }

        public TBase Resolve<TBase>()
        {
            return _container.Resolve<TBase>();
        }

        public object Resolve(Type baseType)
        {
            return _container.Resolve(baseType);
        }

        
    }
}
