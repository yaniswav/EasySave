using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Xml.Serialization;

namespace EasySave
{
    public abstract class LoggingModel
    {
        // Propriétés communes
        public string Name { get; set; }
        public string FileSource { get; set; }
        public string FileTarget { get; set; }
        public long FileSize { get; set; }
        public double FileTransferTime { get; set; }
        public string Time { get; set; }
        public bool Error { get; set; }

        // Méthodes abstraites pour l'écriture et le chargement des logs
        public abstract void WriteLog(LoggingModel log);
        protected abstract List<LoggingModel> LoadLogs(string filePath);

        // Constructeur sans paramètre
        protected LoggingModel()
        {
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