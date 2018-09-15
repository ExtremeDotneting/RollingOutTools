using System;
using System.Globalization;
using System.Threading.Tasks;

namespace RollingOutTools.Localization
{
    /// <summary>
    /// File with translate.
    /// </summary>
    public class JsonFileLocalizationService : ILocalizationService
    {
        public JsonFileLocalizationService(string path, bool checkRowsCountIsSame = false)
        {
            //Json structure
            /*
            [
                //sourceCulture
                {
                    cultureName:"ru-RF",
                    strings:[
                        "привет",
                        "что",
                        "мир"                        
                    ]
                },
                {
                    cultureName:"en-US",
                    strings:[
                        "hello",
                        "---",
                        "world"                       
                    ]
                }

            ]
            */
            //!Удали после написания кода.
            //!В данном случае, каждая строка соответствует аналогичной по порядковому номеру.
            //!Потому количество строк всех локализаций должно совпадать, проверяем если checkRowsCountIsSame.
            //!НО строка вида "---" означает, что локализации для этой строки нет. 
        }

        public async Task<string> GetTranslated(string sourceString, CultureInfo sourceCultureInfo, CultureInfo translateCultureInfo)
        {

            throw new NotImplementedException();
        }
    }
}
