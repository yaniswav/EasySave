using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EasySaveConsole
{
    public class BackupJob
    {
        public string Name { get; set; }
        public string SourceDir { get; set; }
        public string DestinationDir { get; set; }
        public string Type { get; set; }

        public BackupJob(string name, string sourceDir, string destinationDir, string type)
        {
            Name = name;
            SourceDir = sourceDir;
            DestinationDir = destinationDir;
            Type = type;
        }

        public virtual void Start()
        {
            Console.WriteLine($"Starting backup: {Name}");
        }
    }

    public class DifferentialBackup : BackupJob
    {
        public DifferentialBackup(string name, string sourceDir, string destinationDir)
            : base(name, sourceDir, destinationDir, "Differential") { }

        public override void Start()
        {
            base.Start();
            // Implement differential backup logic here
            Console.WriteLine("Differential backup completed.");
        }
    }

    public class CompleteBackup : BackupJob
    {
        public CompleteBackup(string name, string sourceDir, string destinationDir)
            : base(name, sourceDir, destinationDir, "Complete") { }

        public override void Start()
        {
            base.Start();
            // Implement complete backup logic here
            Console.WriteLine("Complete backup completed.");
        }
    }

    public class BackupManager
    {
        private List<BackupJob> _backupJobs = new List<BackupJob>();

        public void AddBackupJob(BackupJob job)
        {
            _backupJobs.Add(job);
        }

        public void RemoveBackupJob(string jobName)
        {
            _backupJobs.RemoveAll(job => job.Name == jobName);
        }

        public void ExecuteAllJobs()
        {
            foreach (var job in _backupJobs)
            {
                try
                {
                    job.Start();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error executing job {job.Name}: {ex.Message}");
                }
            }
        }
    }
}
