using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace RollingOutTools.Storage.JsonFileStorage
{
    /// <summary>
    /// Реализация хранилища в виде json файла.
    /// </summary>
    public class JsonLocalStorage : IKeyValueStorage
    {
        readonly string _filePath;
        Dictionary<string, object> _storageDict;
        JsonSerializerSettings _serializeOpt;
        static object writingLock=1;


        public JsonLocalStorage() : this("def_localstorage")
        {

        }

        public JsonLocalStorage(string valuesNamespace) : this(valuesNamespace, Environment.CurrentDirectory)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="valuesNamespace">Имя файла.</param>
        public JsonLocalStorage(string valuesNamespace, string dirPath)
        {
            _serializeOpt = new JsonSerializerSettings()
            {
#if DEBUG
                Formatting = Formatting.Indented,
#endif
            };

            _filePath = Path.Combine(dirPath, valuesNamespace + ".json");
            try
            {
                _storageDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(
                    File.ReadAllText(_filePath)
                    );
            }
            catch
            {
                File.AppendAllText(_filePath, "");
                _storageDict = new Dictionary<string, object>();
            }
            if(_storageDict==null)
                _storageDict = new Dictionary<string, object>();
        }

        /// <summary>
        /// Не нуждается в синхронизации с Get, все автоматически.
        /// </summary>
        public Task Set(string key, object value)
        {
            AutoResetEvent are = new AutoResetEvent(true);
            var res = Task.Run(() =>
            {
                string serializedDict;
                lock(_storageDict)
                {
                    are.Set();
                    if (value == null)
                    {
                        _storageDict.Remove(key);
                    }
                    else
                    {
                        //_storageDict[key] = value;
                        

                        //var serializedValue = JsonConvert.SerializeObject(
                        //   value,
                        //   _serializeOpt
                        //   );
                        _storageDict[key] = value;
                        
                    }

                    serializedDict = JsonConvert.SerializeObject(
                       _storageDict,
                       _serializeOpt
                       );
                }

                //Лок на запись в файл.
                lock (writingLock)
                {
                   
                    File.WriteAllText(_filePath, serializedDict);
                }
            });
            are.WaitOne();
            return res;
        }

        /// <summary>
        /// Не нуждается в синхронизации с Set, все автоматически.
        /// </summary>
        public async Task<T> Get<T>(string key) 
        {
            return await Task.Run(() =>
            {
                return (T)_get(key, typeof(T));
            }).ConfigureAwait(false);
        }

        object _get(string key, Type t)
        {
            lock (_storageDict)
            {
                if (!_storageDict.ContainsKey(key))
                    return null;

                string serializedStr = JsonConvert.SerializeObject(_storageDict[key]);
                object res = JsonConvert.DeserializeObject(serializedStr, t);
                return res;
            }

        }
    }
}