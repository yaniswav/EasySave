using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace EasySave
{
    public class JsonLogger : LoggingModel
    {
        public override void WriteLog(LoggingModel log)
        {
            // Console.WriteLine("Début de l'écriture du log en JSON.");
            string filePath = GetLogFilePath(".json");
            // Console.WriteLine($"Chemin du fichier de log JSON : {filePath}");

            var logs = LoadLogs<LoggingModel>(filePath, LogFormat.Json);
            // Console.WriteLine($"Logs JSON chargés. Nombre de logs existants : {logs.Count}");

            logs.Add(log);
            string jsonString = JsonConvert.SerializeObject(logs, Formatting.Indented);
            File.WriteAllText(filePath, jsonString);
            // Console.WriteLine("Log JSON écrit dans le fichier.");
        }
    }
}