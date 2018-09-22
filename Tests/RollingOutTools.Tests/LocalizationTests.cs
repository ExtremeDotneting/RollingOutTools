using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using RollingOutTools.Localization;
using System.Globalization;

namespace RollingOutTools.Tests
{
    public class LocalizationTests
    {
        [Fact]
        public async void TestSingle()
        {
            ILocalizationService _localizationService = new ComplexLocalizationService(
                new List<ILocalizationService>(){
                    new DictionaryCacheLocalizationService(),
                    new JsonFileLocalizationService("test.json"),
                    new LitedbCacheLocalizationService("test_cache.db"),
                    new GoogleTranslateLocalizationService()
            });
            var record = await _localizationService.GetTranslated("привет",
                CultureInfo.GetCultureInfo("ru-ru"),
                CultureInfo.GetCultureInfo("en-us"));
            Assert.Equal("hi", record);
        }
    }
}
