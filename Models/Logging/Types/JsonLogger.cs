using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace EasySave
{
    public class JsonLogger : LoggingModel
    {
        protected override async void WriteLogToFile()
        {
            string logFilePath = GetLogFilePath(".json");

            // Serialize the log entry outside of the lock to improve performance
            string jsonString = JsonConvert.SerializeObject(this, Formatting.Indented);

            // Use asynchronous file access for improved performance
            await Task.Run(() =>
            {
                lock (FileLock)
                {
                    // Append the serialized log entry to the file
                    File.AppendAllText(logFilePath, jsonString + Environment.NewLine);
                }
            });
        }
    }
}