using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;

namespace EasySave
{
    // Represents the state of a backup job including details like file paths, progress, etc.
    public class StateModel
    {
        private static readonly object _queueLock = new object();
        private static Queue<StateModel> _updateQueue = new Queue<StateModel>();
        private static Thread _updateThread;


        // Properties to store state information
        public string Name { get; set; }
        public string SourceFilePath { get; set; }
        public string TargetFilePath { get; set; }
        public string State { get; set; }
        public int TotalFilesToCopy { get; set; }
        public long TotalFilesSize { get; set; }
        public int NbFilesLeftToDo { get; set; }
        public double Progression { get; set; }

        static StateModel()
        {
            _updateThread = new Thread(ProcessQueue);
            _updateThread.Start();
        }


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
        public static void UpdateBackupState(BackupJob job)
        {
            StateModel stateModel = new StateModel
            {
                Name = job.Name,
                SourceFilePath = job.SourceDir,
                TargetFilePath = job.DestinationDir,
                State = job.State,
                TotalFilesToCopy = job.TotalFilesToCopy,
                TotalFilesSize = job.TotalFilesSize,
                NbFilesLeftToDo = job.NbFilesLeftToDo,
                Progression = job.Progression
            };

            lock (_queueLock)
            {
                _updateQueue.Enqueue(stateModel);
                Monitor.Pulse(_queueLock);
            }
        }


        private static void ProcessQueue()
        {
            while (true)
            {
                StateModel stateModel = null;

                lock (_queueLock)
                {
                    while (_updateQueue.Count == 0)
                        Monitor.Wait(_queueLock);

                    stateModel = _updateQueue.Dequeue();
                }

                if (stateModel != null)
                {
                    try
                    {
                        string stateFilePath = "State/state.json"; // Update with the actual file path
                        List<StateModel> existingData = LoadFromJson(stateFilePath) ?? new List<StateModel>();
                        existingData.Add(stateModel);
                        SaveToJson(existingData, stateFilePath);
                    }
                    catch (Exception ex)
                    {
                        // Log or handle exceptions related to file writing or JSON serialization
                        Console.WriteLine($"Error while updating state file: {ex.Message}");
                    }
                }
            }
        }
    }
}