using System.Text;

namespace RollingOutTools.ReflectionVisit
{
    class Info_ReflectionMap : IInfo_ReflectionMap
    {
        public string DisplayName { get; internal set; }
        public string RealName { get; internal set; }
        public string Description { get; internal set; }
    }
}
