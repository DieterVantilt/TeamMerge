using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Logic.Utils
{
    public interface IConfigManager
    {
        T GetValue<T>(string key);
        void AddValue<T>(string key, T value);
        void SaveDictionary();
    }

    public class ConfigManager
        : IConfigManager
    {
        private readonly IConfigFileHelper _configFileHelper;

        private IDictionary<string, object> _currentDictionary;

        public ConfigManager(IConfigFileHelper configFileHelper)
        {
            _configFileHelper = configFileHelper;
        }

        public T GetValue<T>(string key)
        {
            var dictionary = GetDictionary();
            var result = default(T);

            if (dictionary.TryGetValue(key, out var value))
            {
                if (value is T outValue)
                {
                    result = outValue;
                }
                else if (value is JArray jsonArray) //If value is a JArray, we can safely assume it is a collection. To get the actual .NET Collection, we need to do ToObject
                {
                    result = jsonArray.ToObject<T>();
                }
                else if (typeof(T).IsEnum)
                {
                    result = (T)Convert.ChangeType(value, TypeCode.Int32);
                }
                else if (value is null)
                {
                    //intentional: default is always null
                }
                else
                {
                    //If this exception happened make sur you modify the configmanagertest for extra tests after you solved it.
                    throw new NotImplementedException("Can't convert the value to a type of " + typeof(T));
                }
            }

            return result;
        }

        public void AddValue<T>(string key, T value)
        {
            var dictionary = GetDictionary();

            if (dictionary.ContainsKey(key))
            {
                dictionary.Remove(key);
            }

            dictionary.Add(key, value);
        }

        public void SaveDictionary()
        {
            if (_currentDictionary != null)
            {
                _configFileHelper.SaveDictionary(_currentDictionary);
            }
        }

        private IDictionary<string, object> GetDictionary()
        {
            if (_currentDictionary == null)
            {
                _currentDictionary = _configFileHelper.GetDictionary();
            }

            return _currentDictionary;
        }
    }
}