using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Threading;

namespace EasySave
{
    [XmlInclude(typeof(XmlLogger))]
    public abstract class LoggingModel
    {
        private static readonly ConcurrentQueue<LoggingModel> LogQueue = new ConcurrentQueue<LoggingModel>();
        private static readonly AutoResetEvent LogSignal = new AutoResetEvent(false);
        private static readonly Thread LogThread;
        protected static readonly object FileLock = new object();

        public string Name { get; set; }
        public string FileSource { get; set; }
        public string FileTarget { get; set; }
        public long FileSize { get; set; }
        public double FileTransferTime { get; set; }
        public string Time { get; set; }
        public bool Error { get; set; }

        public enum LogFormat
        {
            Json,
            Xml
        }

        static LoggingModel()
        {
            LogThread = new Thread(ProcessLogQueue)
            {
                IsBackground = true
            };
            LogThread.Start();
        }

        private static void ProcessLogQueue()
        {
            while (true)
            {
                LogSignal.WaitOne();
                while (LogQueue.TryDequeue(out var logEntry))
                {
                    logEntry.WriteLogToFile();
                }
            }
        }

        public static void EnqueueLog(LoggingModel logEntry)
        {
            LogQueue.Enqueue(logEntry);
            LogSignal.Set();
        }

        protected abstract void WriteLogToFile();

        public static List<T> LoadLogs<T>(string filePath, LogFormat format)
        {
            if (!File.Exists(filePath) || new FileInfo(filePath).Length == 0)
            {
                return new List<T>();
            }

            try
            {
                string content = File.ReadAllText(filePath);
                if (string.IsNullOrWhiteSpace(content))
                {
                    return new List<T>();
                }

                return format == LogFormat.Json
                    ? JsonConvert.DeserializeObject<List<T>>(content)
                    : (List<T>)new XmlSerializer(typeof(List<T>)).Deserialize(new StringReader(content));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during deserialization: {ex.Message}");
                return new List<T>();
            }
        }

        protected static string GetLogFilePath(string extension)
        {
            string logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            Directory.CreateDirectory(logDirectory);
            string logFileName = DateTime.Now.ToString("yyyy-MM-dd") + extension;
            return Path.Combine(logDirectory, logFileName);
        }
    }
}
