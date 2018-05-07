namespace RollingOutTools.ReflectionVisit
{
    /// <summary>
    /// Информация о методе или свойстве.
    /// </summary>
    public interface IInfo_ReflectionMap
    {
        string Description { get; }
        string DisplayName { get; }
        string RealName { get; }
    }
}