using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TeamMerge.Utils
{
    public static class FileHelper
    {
        public static string CreateFileAndReturnPath(string fileName, params string[] paths)
        {
            var roamingPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            var pathsList = new List<string> { roamingPath };
            pathsList.AddRange(paths.ToList());

            var autoMergeFolder = Path.Combine(pathsList.ToArray());
            if (!Directory.Exists(autoMergeFolder))
            {
                Directory.CreateDirectory(autoMergeFolder);
            }

            var settingFilePath = Path.Combine(autoMergeFolder, fileName);
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