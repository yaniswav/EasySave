using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text.Json;

namespace EasySaveConsole
{
    public class BackupConfig
    {
        private const int MaxBackupJobs = 5; // Or load this from App.config if you prefer

        public List<BackupJobConfig> LoadBackupJobs()
        {
            Console.WriteLine("Loading backup jobs...");

            var backupJobsConfigSection =
                ConfigurationManager.GetSection("backupJobsConfig") as BackupJobsConfigSection;
            if (backupJobsConfigSection == null)
            {
                Console.WriteLine("Backup config section not found.");
                return new List<BackupJobConfig>();
            }

            var backupJobs = new List<BackupJobConfig>();
            foreach (BackupJobConfigElement element in backupJobsConfigSection.Jobs)
            {
                backupJobs.Add(new BackupJobConfig
                {
                    Name = element.Name,
                    SourceDir = element.SourceDir,
                    DestinationDir = element.DestinationDir,
                    Type = element.Type
                });
            }

            Console.WriteLine("Backup jobs loaded successfully.");
            return backupJobs;
        }

        public void RemoveBackupJob(BackupJobConfig jobConfig)
        {
            var backupJobsConfigSection =
                ConfigurationManager.GetSection("backupJobsConfig") as BackupJobsConfigSection;
            if (backupJobsConfigSection == null)
            {
                Console.WriteLine("Backup config section not found.");
                return;
            }

            var backupJobConfigElement = new BackupJobConfigElement
            {
                Name = jobConfig.Name,
                SourceDir = jobConfig.SourceDir,
                DestinationDir = jobConfig.DestinationDir,
                Type = jobConfig.Type
            };

            backupJobsConfigSection.Jobs.Remove(backupJobConfigElement);
        }

        public void AddBackupJob(BackupJobConfig jobConfig)
        {
            var backupJobsConfigSection =
                ConfigurationManager.GetSection("backupJobsConfig") as BackupJobsConfigSection;
            if (backupJobsConfigSection == null)
            {
                Console.WriteLine("Backup config section not found.");
                return;
            }

            var backupJobConfigElement = new BackupJobConfigElement
            {
                Name = jobConfig.Name,
                SourceDir = jobConfig.SourceDir,
                DestinationDir = jobConfig.DestinationDir,
                Type = jobConfig.Type
            };

            backupJobsConfigSection.Jobs.Add(backupJobConfigElement);
        }
    }


    public class BackupJobConfig
    {
        public string Name { get; set; }
        public string SourceDir { get; set; }
        public string DestinationDir { get; set; }
        public string Type { get; set; }
    }

    // BackupJobsConfigSection, BackupJobConfigCollection, and BackupJobConfigElement classes go here
}