using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;

namespace TeamMerge.Utils
{
    public static class ConfigManager
    {
        private const string VS_TEAM_MERGE = "Visual Studio Team Merge";
        private static readonly string ConfigName = "teammerge.conf";

        private static IDictionary<string, object> _currentDictionary;

        public static T GetValue<T>(string key)
        {
            var dictionary = GetDictionary();
            var result = default(T);

            if (dictionary.TryGetValue(key, out var value))
            {
                //If value is given type, cast it and return it
                if (value is T castedValue)
                {
                    result = castedValue;
                }
                else
                {
                    //If value is a JArray, we can safely assume it is a collection. To get the actual .NET Collection, we need to do ToObject
                    if (value is JArray jsonArray)
                    {
                        result = jsonArray.ToObject<T>();
                    }
                }
            }

            return result;
        }

        public static void AddValue<T>(string key, T value)
        {
            var dictionary = GetDictionary();

            if (dictionary.ContainsKey(key))
            {
                dictionary.Remove(key);
            }

            dictionary.Add(key, value);
        }

        public static void SaveDictionary()
        {
            if (_currentDictionary != null)
            {
                File.WriteAllText(GetSettingFilePath(), JsonConvert.SerializeObject(_currentDictionary));
            }
        }

        private static IDictionary<string, object> GetDictionary()
        {
            if (_currentDictionary == null)
            {
                var filePath = GetSettingFilePath();
                var file = File.ReadAllText(filePath);

                _currentDictionary = string.IsNullOrWhiteSpace(file) ? new Dictionary<string, object>() : JsonConvert.DeserializeObject<IDictionary<string, object>>(file);
            }

            return _currentDictionary;
        }

        private static string GetSettingFilePath()
        {
            return FileHelper.CreateFileAndReturnPath(ConfigName, VS_TEAM_MERGE);
        }
    }
}
