using System.Globalization;

namespace RollingOutTools.Localization
{
    public class TranslatedRecord
    {
        public string Key => GetKey();
        public string SourceString { get; set; }
        public string TranslatedString { get; set; }
        public CultureInfo SourceCultureInfo { get; set; }
        public CultureInfo TranslateCultureInfo { get; set; }

        string GetKey()
        {
            string res = SourceString + "__" + SourceCultureInfo.Name + "__" + TranslateCultureInfo.Name;
            return res;
        }
    }
}
