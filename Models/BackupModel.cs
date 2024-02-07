using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EasySaveConsole
{
    //Define BackupJob Class to represent backup task
    
    public class BackupJob
    {
        public string Name { get; set; }
        public string SourceDir { get; set; }
        public string DestinationDir { get; set; }
        public string Type { get; set; }

        //Constructor to initialize a new BackupJob instance with parameters
        
        public BackupJob(string name, string sourceDir, string destinationDir, string type)
        {
            Name = name;
            SourceDir = sourceDir;
            DestinationDir = destinationDir;
            Type = type;
        }

        //Start method used to start backup
        
        public virtual void Start()
        {
            Console.WriteLine($"Starting backup: {Name}");
        }
    }
    
    //Derived Class from BackupJob for full backup
    
    public class CompleteBackup : BackupJob
    {
        private const int MaxBufferSize = 1024 * 1024; // 1 MB

        //Specified constructor for full backup 
        
        public CompleteBackup(string name, string sourceDir, string destinationDir)
            : base(name, sourceDir, destinationDir, "Complete")
        {
        }

        //Override Start method to implement full backup process
        
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

        //Recursive copy of a directory and contents
        
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

        //Copy file with a buffer to optimize performance
        
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

        //Determine buffer size based on file size
        
        private int DetermineBufferSize(long fileSize)
        {
            if (fileSize <= MaxBufferSize)
                return (int)fileSize;
            else
                return MaxBufferSize;
        }
    }

    //Derived class from BackupJob for differential backup
    
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

    //Class to manage execution of backup job
    
    public class BackupManager
    {
        private List<BackupJob> _backupJobs = new List<BackupJob>();
        private ConfigModel _configModel = new ConfigModel();

        //Load Backup Jobs from configuration 
        
        public void LoadBackupJobs()
        {
            var jobConfigs = _configModel.LoadBackupJobs();
            foreach (var jobConfig in jobConfigs)
            {
                AddBackupJobBasedOnType(jobConfig);
            }
        }

        //Add backup job based on his type
        
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

        //Execute specified backup jobs
        
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