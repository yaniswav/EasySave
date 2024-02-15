// WriteXML.cs

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace EasySave
{
    public class WriteXML : LoggingModel
    {
        // Méthode pour écrire le log au format XML
        public override void WriteLog(LoggingModel log)
        {
            string filePath = GetLogFilePath(".xml");

            var serializer = new XmlSerializer(typeof(LoggingModel));
            using (var writer = new FileStream(filePath, FileMode.Create))
            {
                serializer.Serialize(writer, log);
            }
        }

        // Méthode pour charger les logs à partir d'un fichier XML
        protected override List<LoggingModel> LoadLogs(string filePath)
        {
            var serializer = new XmlSerializer(typeof(List<LoggingModel>));
            using (var reader = new FileStream(filePath, FileMode.Open))
            {
                return (List<LoggingModel>)serializer.Deserialize(reader);
            }
        }

        // Adapter la méthode GetLogFilePath pour les fichiers XML
        protected static new string GetLogFilePath(string extension)
        {
            // Utiliser la logique existante pour construire le chemin de fichier pour les fichiers XML
            return LoggingModel.GetLogFilePath(extension);
        }
    }
}