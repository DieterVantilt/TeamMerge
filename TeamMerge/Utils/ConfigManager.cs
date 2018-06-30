using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace TeamMerge.Utils
{
    public static class ConfigManager
    {
        private static readonly string VS_TEAM_MERGE = "Visual Studio Team Merge";
        private static readonly string CONFIG_NAME = "teammerge.conf";

        private static IDictionary<string, object> _currentDictionary;

        public static T GetValue<T>(string key)
        {
            var dictionary = GetDictionary();
            var result = default(T);

            if (dictionary.TryGetValue(key, out var value))
            {
                result = (T) Convert.ChangeType(value, typeof(T));
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

                if (string.IsNullOrEmpty(file))
                {
                    _currentDictionary = new Dictionary<string, object>();
                }
                else
                {
                    _currentDictionary = JsonConvert.DeserializeObject<IDictionary<string, object>>(file);
                }
            }

            return _currentDictionary;
        }

        private static string GetSettingFilePath()
        {
            var roamingPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var autoMergeFolder = Path.Combine(roamingPath, VS_TEAM_MERGE);
            if (!Directory.Exists(autoMergeFolder))
            {
                Directory.CreateDirectory(autoMergeFolder);
            }
            var settingFilePath = Path.Combine(autoMergeFolder, CONFIG_NAME);
            if (!File.Exists(settingFilePath))
            {
                using (File.Create(settingFilePath))
                {
                }
            }
            return settingFilePath;
        }
    }
}
