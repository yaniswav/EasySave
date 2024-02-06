using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace EasySaveConsole

{
    public class ConfigModel
    {
        private const string ConfigFilePath = "backupConfig.json";
        private const int MaxBackupJobs = 5;

        public List<BackupJobConfig> LoadBackupJobs()
        {
            if (!File.Exists(ConfigFilePath))
            {
                return new List<BackupJobConfig>();
            }

            string json = File.ReadAllText(ConfigFilePath);
            return JsonSerializer.Deserialize<List<BackupJobConfig>>(json) ?? new List<BackupJobConfig>();
        }

        public void AddBackupJob(BackupJobConfig jobConfig)
        {
            var backupJobs = LoadBackupJobs();

            if (backupJobs.Count >= MaxBackupJobs)
            {
                throw new InvalidOperationException("Maximum number of backup jobs reached.");
            }

            backupJobs.Add(jobConfig);
            string json = JsonSerializer.Serialize(backupJobs, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ConfigFilePath, json);
        }
    }

    public class BackupJobConfig
    {
        public string Name { get; set; }
        public string SourceDir { get; set; }
        public string DestinationDir { get; set; }
        public string Type { get; set; }
    }

    // Remaining classes (BackupJob, CompleteBackup, DifferentialBackup, and BackupManager) remain unchanged
}