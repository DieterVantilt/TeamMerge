using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Logic.Utils
{
    public interface IConfigFileHelper
    {
        IDictionary<string, object> GetDictionary();
        void SaveDictionary(IDictionary<string, object> dictionary);
    }

    public class ConfigFileHelper
        : IConfigFileHelper
    {
        private const string VS_TEAM_MERGE = "Visual Studio Team Merge";
        private static readonly string ConfigName = "teammerge.conf";

        public IDictionary<string, object> GetDictionary()
        {
            var filePath = GetSettingFilePath();
            var file = File.ReadAllText(filePath);

            return string.IsNullOrWhiteSpace(file)
                ? new Dictionary<string, object>()
                : JsonConvert.DeserializeObject<IDictionary<string, object>>(file);
        }

        public void SaveDictionary(IDictionary<string, object> dictionary)
        {
            File.WriteAllText(GetSettingFilePath(), JsonConvert.SerializeObject(dictionary));
        }

        private static string GetSettingFilePath()
        {
            return FileHelper.CreateFileAndReturnPath(ConfigName, VS_TEAM_MERGE);
        }
    }
}