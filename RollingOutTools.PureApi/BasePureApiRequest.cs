namespace RollingOutTools.PureApi
{
    public class BasePureApiRequest
    {
        public string MethodName { get; set; }
        public string AccessToken { get; set; }
        public float ProtocolVersion { get; set; }

    }
}
