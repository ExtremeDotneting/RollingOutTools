using System;
using System.Globalization;
using System.Threading.Tasks;

namespace RollingOutTools.Localization
{
    public class DictionaryCacheLocalizationService : ICahceLocalizationService
    {
        public int Limit { get; }

        public DictionaryCacheLocalizationService(int limit=100)
        {
            Limit = limit;
        }

        public async Task<string> GetTranslated(string sourceString, CultureInfo sourceCultureInfo, CultureInfo translateCultureInfo)
        {
            //!Удали после написания кода.
            //!Используй для поиска свойство Key у TranslatedRecord.
            throw new NotImplementedException();
        }

        public Task SaveTranslated(TranslatedRecord translatedRecord)
        {
            //Clear if limit here.
            throw new NotImplementedException();
        }
    }
}
