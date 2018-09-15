using System;
using System.Globalization;
using System.Threading.Tasks;

namespace RollingOutTools.Localization
{
    public class LitedbCacheLocalizationService : ICahceLocalizationService
    {
        public LitedbCacheLocalizationService(string path)
        {
        }

        public async Task<string> GetTranslated(string sourceString, CultureInfo sourceCultureInfo, CultureInfo translateCultureInfo)
        {
            //!Удали после написания кода.
            //!Используй для поиска свойство Key у TranslatedRecord.
            //!Не забудь проиндексировать.
            throw new NotImplementedException();
        }

        public Task SaveTranslated(TranslatedRecord translatedRecord)
        {
            throw new NotImplementedException();
        }
    }
}
