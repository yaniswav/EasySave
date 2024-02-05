using System;
using System.Collections.Generic;
using System.IO;

namespace EasySaveConsole
{
        
    // BackupJob main class
    public class BackupJob
    {
        public string Name { get; set; }
        public string SourceDir { get; set; }
        public string DestinationDir { get; set; }
        public string Type { get; set; } // Complet ou Différentiel

        public BackupJob(string name, string sourceDir, string destinationDir, string type)
        {
            Name = name;
            SourceDir = sourceDir;
            DestinationDir = destinationDir;
            Type = type;
        }
    }

    // BackupExec class (inherits from BackupJob)
    public class BackupExec : BackupJob
    {
        public BackupExec(string name, string sourceDir, string destinationDir, string type)
            : base(name, sourceDir, destinationDir, type) { }

        public void Save()
        {
            // TODO -> implement save logic here
        }
    }

    // BackupManager class (inherits from BackupJob)
    public class BackupManager
    {
        private List<BackupJob> backupJobs;

        public BackupManager()
        {
            backupJobs = new List<BackupJob>();
            LoadJobsFromJson(); // Load BackupJobs from JSON file
        }

        public void Create(BackupJob job)
        {
            backupJobs.Add(job);
            SaveJobsToJson();
        }

        public void Edit(string jobName, BackupJob updatedJob)
        {
            var job = backupJobs.Find(j => j.Name == jobName);
            if (job != null)
            {
                backupJobs.Remove(job);
                backupJobs.Add(updatedJob);
                SaveJobsToJson();
            }
            // TODO (handle error)
        }

        public void Delete(string jobName)
        {
            var job = backupJobs.Find(j => j.Name == jobName);
            if (job != null)
            {
                backupJobs.Remove(job);
                SaveJobsToJson();
            }
            // TODO (handle error)
        }

        private void SaveJobsToJson()
        {
            // TODO
        }

        private void LoadJobsFromJson()
        {
            // TODO
        }
    }
}
