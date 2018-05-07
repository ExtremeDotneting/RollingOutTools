using System;
using System.Collections.Generic;
using System.Text;

namespace RollingOutTools.PureApi
{
    /// <summary>
    /// Все результаты разных api, включая api своего сервера, должны передаваться в объектах этого класса. 
    /// Это необходимо для большей гибкости, но при этом стандартизации запросов.
    /// </summary>
    public class PureApiResponse<T> : IPureApiResponse
    {
        /// <summary>
        /// Возвращаемое значение. 
        /// </summary>
        public T ResponseValue { get; set; }

        public object ResponseValue_NonGeneric {
            get
            {
                return ResponseValue;
            }
        }

        /// <summary>
        /// Просто дает лучше понять о том как отработал метод.
        /// </summary>
        public string ResponseInfo { get; set; }

        public bool IsError { get; }

        public PureApiResponse(T responseValue, string responseInfo, bool isError=false)
        {
            IsError = isError;
            ResponseValue = responseValue;
            ResponseInfo = responseInfo;
        }

    }

    /// <summary>
    /// Служит результатом для методов без возвращаемых данных. Передает только информацию о том как отработал метод.
    /// </summary>
    public class PureApiResponse : IPureApiResponse
    {
        public PureApiResponse(string responseInfo, bool isError=false)
        {
            IsError = isError;
            ResponseInfo = responseInfo;
        }

        public string ResponseInfo { get; }

        public object ResponseValue_NonGeneric => null;

        public bool IsError { get; }
    }



}
