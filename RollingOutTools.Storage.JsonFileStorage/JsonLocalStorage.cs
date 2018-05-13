using Newtonsoft.Json;
using RollingOutTools.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Storage in json file.
    /// </summary>
    public class JsonLocalStorage : IKeyValueStorage
    {
        const int TimeoutSeconds = 30;
        readonly object Locker = new object();
        readonly string _storageFilePath;
        readonly string _syncFilePath;
        long _currentSyncIteration;
        Dictionary<string, object> _storageDict;
        JsonSerializerSettings _serializeOpt;
        static object writingLock = 1;


        public JsonLocalStorage() : this("def_localstorage")
        {

        }

        public JsonLocalStorage(string valuesNamespace) : this(valuesNamespace, Environment.CurrentDirectory)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="valuesNamespace">File name or same.</param>
        public JsonLocalStorage(string valuesNamespace, string dirPath)
        {
            _serializeOpt = new JsonSerializerSettings();
            if (Debugger.IsAttached)
            {
                _serializeOpt.Formatting = Formatting.Indented;
            }

            _storageFilePath = Path.Combine(dirPath, valuesNamespace + ".json");
            _syncFilePath= Path.Combine(dirPath, valuesNamespace + "_sync.txt");
            CommonHelpers.TryCreateFileIfNotExists(_storageFilePath);
            CommonHelpers.TryCreateFileIfNotExists(_syncFilePath);

        }

        /// <summary>
        /// Automatically synchronized with Get. If 'null' - will remove value.
        /// If you not closing application, recommended not await task.
        /// </summary>
        public async Task Set(string key, object value)
        {
            await Task.Run(() =>
            {
                string serializedDict;
                lock (Locker)
                {
                    _LoadStorageState();     
                    
                    if (value == null)
                    {
                        _storageDict.Remove(key);
                    }
                    else
                    {
                        _storageDict[key] = value;
                    }

                    serializedDict = JsonConvert.SerializeObject(
                       _storageDict,
                       _serializeOpt
                       );

                    _SaveStorageState(serializedDict);
                }
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Automatically synchronized with Set. 
        /// If key not exists - will return null for reference type and default value for value types.
        /// </summary>
        public async Task<T> Get<T>(string key)
        {
            return await Task.Run(() =>
            {
                return (T)_Get(key, typeof(T));
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Remember, that null value is equals that it not exists. 
        /// So, if before you set 'key_name' value to null, then ContainsKey will return false;
        /// </summary>
        public async Task<bool> ContainsKey(string key)
        {
            object value = await Get<object>(key);
            return value == null;
        }

        object _Get(string key, Type t)
        {
            lock (Locker)
            {
                _LoadStorageState();

                if (!_storageDict.ContainsKey(key))
                {
                    //return default value for structs or null for class
                    if (t.IsValueType)
                        return Activator.CreateInstance(t);
                    else
                        return null;
                }


                string serializedStr = JsonConvert.SerializeObject(_storageDict[key]);
                object res = JsonConvert.DeserializeObject(serializedStr, t);

                


                return res;
            }

        }

        void _LoadStorageState()
        {
            long fromFileSyncIteration = _ReadSyncIteration();
            if (_storageDict == null || _currentSyncIteration < fromFileSyncIteration)
            {
                _storageDict = _ReadStorage();
                _currentSyncIteration = fromFileSyncIteration;
            }
        }

        void _SaveStorageState(string storage)
        {
            _WriteStorage(storage);
            _WriteSyncIteration(++_currentSyncIteration);
        }

        Dictionary<string, object> _ReadStorage()
        {
            Dictionary<string, object> res = null;
            try
            {
                CommonHelpers.TryReadAllText(_storageFilePath, out string strFromFile, TimeoutSeconds);
                res = JsonConvert.DeserializeObject<Dictionary<string, object>>(
                    strFromFile
                    );
            }
            catch { }
            if (res == null)
                res = new Dictionary<string, object>();
            return res;
        }

        void _WriteStorage(string storage)
        {
            CommonHelpers.TryCreateFileIfNotExists(_storageFilePath);
            CommonHelpers.TryWriteAllText(_storageFilePath, storage, TimeoutSeconds);
        }

        long _ReadSyncIteration()
        {
            try
            {
                bool success = CommonHelpers.TryReadAllText(_syncFilePath, out string str, TimeoutSeconds);
                long res = success ? Convert.ToInt64(str) : 0;
                return res;
            }
            catch
            {
                return 0;
            }
            
        }

        void _WriteSyncIteration(long newIteration)
        {
            CommonHelpers.TryCreateFileIfNotExists(_syncFilePath);
            CommonHelpers.TryWriteAllText(_syncFilePath, newIteration.ToString(), TimeoutSeconds);
        }

        


    }
}