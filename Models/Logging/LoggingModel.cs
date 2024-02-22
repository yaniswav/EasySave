using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace EasySave
{
    [XmlInclude(typeof(XmlLogger))]
    public class LoggingModel
    {
        // Propriétés communes
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

        // Méthodes abstraites pour l'écriture et le chargement des logs
        public virtual void WriteLog(LoggingModel log)
        {
        }


        public static List<T> LoadLogs<T>(string filePath, LogFormat format)
        {
            if (!File.Exists(filePath) || new FileInfo(filePath).Length == 0)
            {
                // Console.WriteLine("Log file is empty or not found. Creating a new list.");
                return new List<T>();
            }

            try
            {
                string content = File.ReadAllText(filePath);
                if (string.IsNullOrWhiteSpace(content))
                {
                    // Console.WriteLine("Log file is empty. Creating a new list.");
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


        // Méthode helper pour obtenir le chemin du fichier de log
        protected static string GetLogFilePath(string extension)
        {
            string logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            Directory.CreateDirectory(logDirectory); // Crée le répertoire s'il n'existe pas

            string logFileName = DateTime.Now.ToString("yyyy-MM-dd") + extension;
            return Path.Combine(logDirectory, logFileName);
        }
    }
}