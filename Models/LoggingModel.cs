using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace EasySaveConsole
{
    // Represents the model for logging backup job details
    public class LoggingModel
    {
        // Properties to store log details
        public string Name { get; set; }
        public string FileSource { get; set; }
        public string FileTarget { get; set; }
        public long FileSize { get; set; }
        public double FileTransferTime { get; set; }
        public string Time { get; set; }

        // Retrieves the path to the log file, ensuring the directory exists
        private static string GetLogFilePath()
        {
            //Use an appropriate path for client file system
            string logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            if (!Directory.Exists(logDirectory))
                Directory.CreateDirectory(logDirectory);

            string logFileName = DateTime.Now.ToString("yyyy-MM-dd") + ".json";
            return Path.Combine(logDirectory, logFileName);
        }

        // Logs details of a file transfer to a JSON file
        public static void LogFileTransfer(string name, string source, string target, long fileSize,
            double transferTime, bool error = false)
        {
            // Create a new log entry
            var log = new LoggingModel
            {
                Name = name,
                FileSource = source,
                FileTarget = target,
                FileSize = fileSize,
                FileTransferTime = error ? -transferTime : transferTime,
                Time = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")
            };

            List<LoggingModel> logs = new List<LoggingModel>();
            string filePath = GetLogFilePath();

            if (File.Exists(filePath))
            {
                string existingLogs = File.ReadAllText(filePath);
                logs = JsonSerializer.Deserialize<List<LoggingModel>>(existingLogs) ?? new List<LoggingModel>();
            }

            logs.Add(log);

            string jsonString = JsonSerializer.Serialize(logs, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, jsonString);
        }
    }
}