using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace RollingOutTools.Localization
{
    public class DefaultLocalizationService : ILocalizationService
    {
        public CultureInfo DefaultCultureInfo { get; }

        public DefaultLocalizationService(CultureInfo defaultCultureInfo)
        {
            DefaultCultureInfo = defaultCultureInfo;
        }

        public async Task<string> GetTranslated(string sourceString, CultureInfo sourceCultureInfo, CultureInfo translateCultureInfo)
        {
            return sourceString;
        }
    }
}
