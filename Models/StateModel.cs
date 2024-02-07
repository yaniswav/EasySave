using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace EasySaveConsole
{
    public class StateModel
    {
        public string Name { get; set; }
        public string SourceFilePath { get; set; }
        public string TargetFilePath { get; set; }
        public string State { get; set; }
        public int TotalFilesToCopy { get; set; }
        public long TotalFilesSize { get; set; }
        public int NbFilesLeftToDo { get; set; }
        public double Progression { get; set; }

        public static List<StateModel> LoadFromJson(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    return new List<StateModel>();

                string jsonString = File.ReadAllText(filePath);
                // Console.WriteLine("Contenu du fichier JSON : " + jsonString); // Ajout pour le débogage
                return JsonSerializer.Deserialize<List<StateModel>>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Erreur de désérialisation JSON : {ex.Message}");
                return new List<StateModel>();
            }
        }

        public static void SaveToJson(List<StateModel> stateModels, string filePath)
        {
            string jsonString = JsonSerializer.Serialize(stateModels, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(filePath, jsonString);
        }


        // Method to update the state of a backup job
        public static void UpdateBackupState(BackupJob job, string state, int totalFilesToCopy, long totalFilesSize,
            int nbFilesLeftToDo, double progression, string stateFilePath)
        {
            StateModel stateModel = new StateModel
            {
                Name = job.Name,
                SourceFilePath = job.SourceDir,
                TargetFilePath = job.DestinationDir,
                State = state,
                TotalFilesToCopy = totalFilesToCopy,
                TotalFilesSize = totalFilesSize,
                NbFilesLeftToDo = nbFilesLeftToDo,
                Progression = progression
            };

            // Load existing state data
            List<StateModel> stateModels = LoadFromJson(stateFilePath);

            // Update or add the current job's state in the list
            var existingStateModel = stateModels.FirstOrDefault(s => s.Name == job.Name);
            if (existingStateModel != null)
            {
                existingStateModel.State = state;
                existingStateModel.TotalFilesToCopy = totalFilesToCopy;
                existingStateModel.TotalFilesSize = totalFilesSize;
                existingStateModel.NbFilesLeftToDo = nbFilesLeftToDo;
                existingStateModel.Progression = progression;
            }
            else
            {
                stateModels.Add(stateModel);
            }

            // Save updated state to JSON file using the new method
            SaveToJson(stateModels, stateFilePath);
        }
    }
}