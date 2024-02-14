using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace EasySave
{
    // Represents the state of a backup job including details like file paths, progress, etc.
    public class StateModel
    {
        // Properties to store state information
        public string Name { get; set; }
        public string SourceFilePath { get; set; }
        public string TargetFilePath { get; set; }
        public string State { get; set; }
        public int TotalFilesToCopy { get; set; }
        public long TotalFilesSize { get; set; }
        public int NbFilesLeftToDo { get; set; }
        public double Progression { get; set; }

        // Loads state data from a JSON file, returning a list of StateModel instances
        public static List<StateModel> LoadFromJson(string filePath)
        {
            try
            {
                // Check if the file exists to avoid FileNotFoundException
                if (!File.Exists(filePath))
                    return new List<StateModel>();

                // Read the Json file content
                string jsonString = File.ReadAllText(filePath);
                // Console.WriteLine("Content of JSON file : " + jsonString); // Add for debug
                return JsonSerializer.Deserialize<List<StateModel>>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (JsonException ex)
            {
                // Handle JSON deserialization errors
                Console.WriteLine($"Erreur de désérialisation JSON : {ex.Message}");
                return new List<StateModel>(); // Return an empty list on error
            }
        }

        // Saves a list of StateModel instances to a JSON file
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
            // Create or update a StateModel instance with the current job's state
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

            // Load existing state data from JSON
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