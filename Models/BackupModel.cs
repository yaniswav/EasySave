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
    
    public class CompleteBackup : BackupJob
    {
        private const int MaxBufferSize = 1024 * 1024; // 1 MB

        public CompleteBackup(string name, string sourceDir, string destinationDir)
            : base(name, sourceDir, destinationDir, "Complete")
        {
        }

        public override void Start()
        {
            base.Start();
            try
            {
                CopyDirectory(SourceDir, DestinationDir);
                Console.WriteLine("Complete backup completed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during backup: {ex.Message}");
            }
        }

        private void CopyDirectory(string sourceDir, string destinationDir)
        {
            if (!Directory.Exists(destinationDir))
                Directory.CreateDirectory(destinationDir);

            foreach (var file in Directory.GetFiles(sourceDir))
            {
                var destFile = Path.Combine(destinationDir, Path.GetFileName(file));
                CopyFileWithBuffer(file, destFile);
            }

            foreach (var directory in Directory.GetDirectories(sourceDir))
            {
                var destDir = Path.Combine(destinationDir, Path.GetFileName(directory));
                CopyDirectory(directory, destDir);
            }
        }

        private void CopyFileWithBuffer(string sourceFile, string destinationFile)
        {
            long fileSize = new FileInfo(sourceFile).Length;
            int bufferSize = DetermineBufferSize(fileSize);

            using (FileStream sourceStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read))
            {
                using (FileStream destStream = new FileStream(destinationFile, FileMode.Create, FileAccess.Write))
                {
                    byte[] buffer = new byte[bufferSize];
                    int bytesRead;
                    while ((bytesRead = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        destStream.Write(buffer, 0, bytesRead);
                    }
                }
            }
        }

        private int DetermineBufferSize(long fileSize)
        {
            if (fileSize <= MaxBufferSize)
                return (int)fileSize;
            else
                return MaxBufferSize;
        }
    }

    public class DifferentialBackup : BackupJob
    {
        public DifferentialBackup(string name, string sourceDir, string destinationDir)
            : base(name, sourceDir, destinationDir, "Differential")
        {
        }

        public override void Start()
        {
            base.Start();
            // Implement differential backup logic here
            Console.WriteLine("Differential backup completed.");
        }
    }

    public class BackupManager
    {
        private List<BackupJob> _backupJobs = new List<BackupJob>();
        private ConfigModel _configModel = new ConfigModel();

        public void LoadBackupJobs()
        {
            var jobConfigs = _configModel.LoadBackupJobs();
            foreach (var jobConfig in jobConfigs)
            {
                AddBackupJobBasedOnType(jobConfig);
            }
        }

        private void AddBackupJobBasedOnType(dynamic job)
        {
            string type = job.Type;
            string name = job.Name;
            string sourceDir = job.SourceDir;
            string destinationDir = job.DestinationDir;

            switch (type)
            {
                case "Complete":
                    _backupJobs.Add(new CompleteBackup(name, sourceDir, destinationDir));
                    break;
                case "Differential":
                    _backupJobs.Add(new DifferentialBackup(name, sourceDir, destinationDir));
                    break;
                default:
                    throw new InvalidOperationException("Unknown backup type");
            }
        }

        public void ExecuteJobs(string[] jobNames)
        {
            foreach (string jobName in jobNames)
            {
                BackupJob jobToExecute =
                    _backupJobs.FirstOrDefault(job => job.Name.Equals(jobName, StringComparison.OrdinalIgnoreCase));
                if (jobToExecute != null)
                {
                    jobToExecute.Start();
                }
                else
                {
                    Console.WriteLine($"Backup job '{jobName}' not found.");
                }
            }
        }
    }
}