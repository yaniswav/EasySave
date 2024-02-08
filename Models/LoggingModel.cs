using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace EasySaveConsole
{
    public class LogModel
    {
        public string Name { get; set; }
        public string FileSource { get; set; }
        public string FileTarget { get; set; }
        public long FileSize { get; set; }
        public double FileTransferTime { get; set; }
        public DateTime Time { get; set; }

        public static void WriteLog(LogModel log, string logFilePath)
        {
            List<LogModel> logs = new List<LogModel>();
            if (File.Exists(logFilePath))
            {
                string existingLogs = File.ReadAllText(logFilePath);
                logs = JsonSerializer.Deserialize<List<LogModel>>(existingLogs) ?? new List<LogModel>();
            }

            logs.Add(log);

            string jsonString = JsonSerializer.Serialize(logs, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(logFilePath, jsonString);
        }
    }
}