using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace EasySave
{
    public class XmlLogger : LoggingModel
    {
        // Cette méthode est appelée par le thread de journalisation dans la classe de base pour écrire les données dans le fichier XML.
        protected override void WriteLogToFile()
        {
            lock (FileLock) // Assurez-vous qu'un seul thread écrit dans le fichier à la fois
            {
                List<LoggingModel> logs = LoadLogs<LoggingModel>(GetLogFilePath(".xml"), LogFormat.Xml) ?? new List<LoggingModel>();
                logs.Add(this); // Ajoute l'entrée actuelle au journal
                using (StreamWriter writer = new StreamWriter(GetLogFilePath(".xml"), false))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<LoggingModel>));
                    serializer.Serialize(writer, logs);
                }
            }
        }
    }
}