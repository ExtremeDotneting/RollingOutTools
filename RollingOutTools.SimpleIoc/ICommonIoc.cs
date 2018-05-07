using System;

namespace RollingOutTools.SimpleIoc
{
    public interface ICommonIoc
    {        
        object Resolve(Type baseType);
        TBase Resolve<TBase>();
    }
}