
namespace RollingOutTools.PureApi
{
    public interface IPureApiResponse
    {
        string ResponseInfo { get;  }

        object ResponseValue_NonGeneric { get;  }

        bool IsError { get; }
    }
}