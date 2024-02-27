using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace EasySave
{
    public class XmlLogger : LoggingModel
    {
        protected override void WriteLogToFile()
        {
            string logFilePath = GetLogFilePath(".xml");

            lock (FileLock)
            {
                if (!File.Exists(logFilePath))
                {
                    // Create a new file with a root element
                    CreateNewLogFile(logFilePath);
                }

                // Append the log entry to the existing file
                AppendLogEntryToFile(logFilePath, this);
            }
        }

        private void CreateNewLogFile(string filePath)
        {
            using (var writer = XmlWriter.Create(filePath, new XmlWriterSettings { Indent = true }))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Logs");
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        private void AppendLogEntryToFile(string filePath, LoggingModel logEntry)
        {
            var doc = new XmlDocument();
            doc.Load(filePath);

            var root = doc.DocumentElement;

            var serializer = new XmlSerializer(typeof(LoggingModel));
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, logEntry);
                stream.Position = 0;
                var logEntryDoc = new XmlDocument();
                logEntryDoc.Load(stream);
                var importedNode = doc.ImportNode(logEntryDoc.DocumentElement, true);
                
                root.AppendChild(importedNode);

                using (var writer = XmlWriter.Create(filePath, new XmlWriterSettings { Indent = true }))
                {
                    doc.Save(writer);
                }
            }
        }
    }
}
