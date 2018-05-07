using System;
using System.Collections.Generic;
using System.Text;

namespace RollingOutTools.PureApi
{
    /// <summary>
    /// ИСПОЛЬЗУЙТЕ ВСЕГДА, КОГДА АПИ ОТРАБОТАЛО НЕ ПО ИДЕАЛЬНОМУ И ВАРИАНТОВ ИСПОЛЬЗОВАНИЯ.
    /// Используйте эти исключения везде в апи, если хотите чтоб сервер передал поле ServerResponse в качестве ответа клиенту. 
    /// Все остальные исключения на клиенте будут классифицироваться как непредусмотренные и сервер вместо них будет возвращать просто текст об непредвиденной ошибке.
    /// </summary>
    public class PureApiException : Exception
    {
        public PureApiException(string responseInfo) : base(responseInfo)
        {
          
        }

    }


}
