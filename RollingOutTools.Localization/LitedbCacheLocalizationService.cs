using System;
using System.Globalization;
using System.Threading.Tasks;
using LiteDB;

namespace RollingOutTools.Localization
{
    public class LitedbCacheLocalizationService : ICahceLocalizationService, IDisposable
    {
        private LiteDatabase database;
        private LiteCollection<TranslatedRecord> records;

        public LitedbCacheLocalizationService(string path)
        {
            database = new LiteDatabase(path);
            records = database.GetCollection<TranslatedRecord>("translatedRecords");
            records.EnsureIndex("Key");
        }

        public async Task<string> GetTranslated(string sourceString, CultureInfo sourceCultureInfo, CultureInfo translateCultureInfo)
        {
            var wantedKey = TranslatedRecord.GetKey(sourceString, 
                sourceCultureInfo, 
                translateCultureInfo);
            return records.FindOne(x => x.Key == wantedKey).TranslatedString;
        }

        public async Task SaveTranslated(TranslatedRecord translatedRecord)
        {
            records.Insert(translatedRecord);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    database.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose() => Dispose(true);
        #endregion
    }
}
