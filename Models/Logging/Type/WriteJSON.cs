using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json; // Utilisez Newtonsoft.Json

namespace EasySave
{
    public class WriteJSON : LoggingModel
    {
        public override void WriteLog(LoggingModel log)
        {
            Console.WriteLine("Début de l'écriture du log."); // Logging
            string filePath = GetLogFilePath(".json");
            Console.WriteLine($"Chemin du fichier de log : {filePath}"); // Logging

            var logs = LoadLogs(filePath);
            Console.WriteLine($"Logs chargés. Nombre de logs existants : {logs.Count}"); // Logging

            // logs.Add(log);
            //
            // string jsonString =
            //     JsonConvert.SerializeObject(logs, Formatting.Indented); // Newtonsoft.Json pour la sérialisation
            // File.WriteAllText(filePath, jsonString);
            // Console.WriteLine("Log écrit dans le fichier."); // Logging
        }

        protected override List<LoggingModel> LoadLogs(string filePath)
        {
            Console.WriteLine($"Chargement des logs depuis le fichier : {filePath}"); // Logging

            if (File.Exists(filePath))
            {
                try
                {
                    string existingLogs = File.ReadAllText(filePath);
                    Console.WriteLine("Logs existants chargés."); // Logging
                    Console.WriteLine($"Logs existants : {existingLogs}"); // Logging
                    return JsonConvert
                        .DeserializeObject<List<LoggingModel>>(existingLogs); // Newtonsoft.Json pour la désérialisation
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Erreur de désérialisation JSON : {ex.Message}"); // Logging
                    return new List<LoggingModel>();
                }
            }

            Console.WriteLine("Fichier de log non trouvé, création d'une nouvelle liste."); // Logging
            return new List<LoggingModel>();
        }
    }
}