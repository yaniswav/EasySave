using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace EasySave
{
    public class JsonLogger : LoggingModel
    {
        // Cette méthode est appelée par le thread de journalisation dans la classe de base pour écrire les données dans le fichier JSON.
        protected override void WriteLogToFile()
        {
            lock (FileLock) // Utilisez le verrou de fichier de la classe de base
            {
                List<LoggingModel> logs = LoadLogs<LoggingModel>(GetLogFilePath(".json"), LogFormat.Json) ??
                                          new List<LoggingModel>();
                logs.Add(this); // Ajoute l'entrée actuelle au journal
                string jsonString = JsonConvert.SerializeObject(logs, Formatting.Indented);
                File.WriteAllText(GetLogFilePath(".json"), jsonString);
            }
        }
    }
}