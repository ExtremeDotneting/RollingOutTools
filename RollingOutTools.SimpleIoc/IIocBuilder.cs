namespace RollingOutTools.SimpleIoc
{
    public interface IIocBuilder
    {
        void RegisterInstance<TImplemention, TBase>(TImplemention inst)
            where TImplemention : class, TBase
            where TBase : class;
        void RegisterType<TImplemention, TBase>() where TImplemention : TBase;        
        ICommonIoc Build();
    }
}