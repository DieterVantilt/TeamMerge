using System;
using System.IO;

namespace TeamMerge.Utils
{
    public interface ILogger
    {
        void LogException(Exception exception);
    }

    public class Logger
        : ILogger
    {
        private static readonly string VS_TEAM_MERGE = "Visual Studio Team Merge";
        private static readonly string LOGGER_PATH_NAME = "Logging";
        private static readonly string LOGGING_FILE_NAME = "logging.log";

        public void LogException(Exception exception)
        {
            var logginData = File.ReadAllText(GetLoggingFilePath());

            //makes sur the logging doesn't get to big after a while. (This is an easy fix)
            if (logginData.Length > 50000)
            {
                logginData = logginData.Remove(0, 25000);
            }

            logginData += DateTime.Now + Environment.NewLine + exception.ToString() + Environment.NewLine + Environment.NewLine;

            File.WriteAllText(GetLoggingFilePath(), logginData);
        }

        public string GetLoggingFilePath()
        {
            return FileHelper.CreateFileAndReturnPath(LOGGING_FILE_NAME, VS_TEAM_MERGE, LOGGER_PATH_NAME);
        }
    }
}