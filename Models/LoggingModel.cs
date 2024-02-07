using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

//Define namespace to organize classes related to logs functionality

namespace EasySaveConsole
{
    //Class for LogEntry 
    
    public class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public string BackupName { get; set; }
        public string Action { get; set; }
        public string Details { get; set; }

        //Constructor to initialize new entry in log with specified details 
        
        public LogEntry(string backupName, string action, string details)
        {
            Timestamp = DateTime.Now;
            BackupName = backupName;
            Action = action;
            Details = details;
        }
    }

    //Class to manage log model and log writing entries
    
    public class LogModel
    {
        private readonly string _logFilePath;
        private readonly List<LogEntry> _logEntries;

        
        //Constructor to initialize log model with specified path
        
        public LogModel(string logFilePath)
        {
            _logFilePath = logFilePath;
            _logEntries = new List<LogEntry>();
        }

        //Method to add a new entry in log and file entry
        
        public void AddEntry(string backupName, string action, string details)
        {
            var logEntry = new LogEntry(backupName, action, details);
            _logEntries.Add(logEntry);
            WriteLogToFile(logEntry);
        }

        //Private method to write an log entry in LogFile
        
        private void WriteLogToFile(LogEntry logEntry)
        {
            string json = JsonSerializer.Serialize(logEntry, new JsonSerializerOptions { WriteIndented = true });
            File.AppendAllText(_logFilePath, json + Environment.NewLine);
        }
    }
}