using System.Threading.Tasks;

namespace RollingOutTools.Localization
{
    public interface ICahceLocalizationService:ILocalizationService
    {
        /// <summary>
        /// Save new translated string (with override).
        /// </summary>
        public async Task SaveTranslated(TranslatedRecord translatedRecord);
    }
}
