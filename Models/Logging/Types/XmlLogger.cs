using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace EasySave
{
    public class XmlLogger : LoggingModel
    {
        // Méthode pour écrire le log au format XML
        public override void WriteLog(LoggingModel log)
        {
            try
            {
                Console.WriteLine("Début de l'écriture du log en XML.");
                string filePath = GetLogFilePath(".xml");
                Console.WriteLine($"Chemin du fichier de log XML : {filePath}");

                var logs = LoadLogs<LoggingModel>(filePath, LogFormat.Xml);
                Console.WriteLine($"Logs XML chargés. Nombre de logs existants : {logs.Count}");

                logs.Add(log);
                XmlSerializer serializer = new XmlSerializer(typeof(List<LoggingModel>));
                Console.WriteLine($"XML Serializer : {serializer}");
                using (StreamWriter writer = new StreamWriter(filePath, false)) // false to overwrite the file

                {
                    serializer.Serialize(writer, logs);
                    Console.WriteLine(
                        $"Sérialisation des logs XML terminée. Nombre de logs : {logs.Count}, chemin : {filePath}, writer : {writer}, serializer : {serializer}");
                }

                Console.WriteLine("Log XML écrit dans le fichier.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error during the log writing: " + e.Message);
                if (e.InnerException != null)
                {
                    Console.WriteLine("Inner exception: " + e.InnerException.Message);
                }
            }

        }
    }
}