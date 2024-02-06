using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

namespace EasySaveConsole
{
    public class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public string BackupName { get; set; }
        public string Action { get; set; }
        public string Details { get; set; }

        public LogEntry(string backupName, string action, string details)
        {
            Timestamp = DateTime.Now;
            BackupName = backupName;
            Action = action;
            Details = details;
        }
    }

    public class LogModel
    {
        private readonly string _logFilePath;
        private readonly List<LogEntry> _logEntries;

        public LogModel(string logFilePath)
        {
            _logFilePath = logFilePath;
            _logEntries = new List<LogEntry>();
        }

        public void AddEntry(string backupName, string action, string details)
        {
            var logEntry = new LogEntry(backupName, action, details);
            _logEntries.Add(logEntry);
            WriteLogToFile(logEntry);
        }

        private void WriteLogToFile(LogEntry logEntry)
        {
            string json = JsonSerializer.Serialize(logEntry, new JsonSerializerOptions { WriteIndented = true });
            File.AppendAllText(_logFilePath, json + Environment.NewLine);
        }
    }
}