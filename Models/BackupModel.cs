using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Configuration;

namespace EasySaveConsole
{
    //class representing a backup job with properties
    public class BackupJob
    {
        public string Name { get; set; }
        public string SourceDir { get; set; }
        public string DestinationDir { get; set; }
        public string Type { get; set; }

        protected int TotalFilesToCopy = 0;
        protected long TotalFilesSize = 0;
        protected int NbFilesLeftToDo = 0;
        protected double Progression = 0;

        //Constructor to initialize a new BackupJob with basic details
        public BackupJob(string name, string sourceDir, string destinationDir, string type)
        {
            Name = name;
            SourceDir = sourceDir;
            DestinationDir = destinationDir;
            Type = type;
        }


        // Starts the backup process
        public virtual void Start()
        {
            Console.WriteLine($"Starting backup: {Name}");
        }

        protected const int MaxBufferSize = 1024 * 1024; // 1 MB

        // Copies a file from source to destination with a buffer to optimize performance
        protected void CopyFileWithBuffer(string sourceFile, string destinationFile)
        {
            long fileSize = new FileInfo(sourceFile).Length;

            int bufferSize = DetermineBufferSize(fileSize);

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
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

                stopwatch.Stop();

                // Log in log.json file
                LoggingModel.LogFileTransfer(
                    this.Name, //Supposed you have an attribute Name in your class to identify backup
                    sourceFile,
                    destinationFile,
                    fileSize,
                    stopwatch.ElapsedMilliseconds
                );
                Console.WriteLine(
                    $"Log de transfert pour {sourceFile} effectué. Temps de transfert : {stopwatch.ElapsedMilliseconds} ms.");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                Console.WriteLine($"Erreur lors de la copie du fichier : {ex.Message}");
                //Be sure to have error management in your LoggingModel
                LoggingModel.LogFileTransfer(
                    this.Name, //Supposed you have an attribute Name in your class to identify your backup
                    sourceFile,
                    destinationFile,
                    fileSize,
                    stopwatch.ElapsedMilliseconds,
                    true // Return an error 
                );
            }
        }


        // Determines the buffer size based on the file size
        protected int DetermineBufferSize(long fileSize)
        {
            if (fileSize <= MaxBufferSize)
                return (int)fileSize;
            else
                return MaxBufferSize;
        }


        // Updates the state of the backup job with progress and other details
        protected void UpdateState(string state)
        {
            StateModel.UpdateBackupState(this, state, TotalFilesToCopy, TotalFilesSize, NbFilesLeftToDo, Progression,
                "state.json");
        }
    }

    // Derived class for performing a complete backup
    public class CompleteBackup : BackupJob
    {
        public CompleteBackup(string name, string sourceDir, string destinationDir)
            : base(name, sourceDir, destinationDir, "Complete")
        {
        }

        // Overrides the Start method to perform a complete backup
        public override void Start()
        {
            base.Start();
            InitializeTrackingProperties();
            UpdateState("ACTIVE");

            try
            {
                CopyDirectory(SourceDir, DestinationDir);
                UpdateProgress(null);
                UpdateState("END");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during backup: {ex.Message}");
                UpdateState("ERROR");
            }
        }

        // Initializes properties to track progress and size of the backup
        private void InitializeTrackingProperties()
        {
            var allFiles = Directory.GetFiles(SourceDir, "*", SearchOption.AllDirectories);
            TotalFilesToCopy = allFiles.Length;
            TotalFilesSize = allFiles.Sum(file => new FileInfo(file).Length);
            NbFilesLeftToDo = TotalFilesToCopy;
        }

        // Updates the progress of the backup process
        private void UpdateProgress(string fileCopied)
        {
            if (NbFilesLeftToDo > 0)
            {
                NbFilesLeftToDo--;
            }

            Progression = TotalFilesToCopy > 0 ? 100.0 * (TotalFilesToCopy - NbFilesLeftToDo) / TotalFilesToCopy : 100;
            UpdateState("ACTIVE");
        }


        // Recursively copies directories and files from source to destination
        private void CopyDirectory(string sourceDir, string destinationDir)
        {
            if (!Directory.Exists(destinationDir))
                Directory.CreateDirectory(destinationDir);

            foreach (var file in Directory.GetFiles(sourceDir))
            {
                var destFile = Path.Combine(destinationDir, Path.GetFileName(file));
                CopyFileWithBuffer(file, destFile);
                UpdateProgress(file);
            }

            foreach (var directory in Directory.GetDirectories(sourceDir))
            {
                var destDir = Path.Combine(destinationDir, Path.GetFileName(directory));
                CopyDirectory(directory, destDir);
            }
        }
    }

    // Derived class for performing a differential backup
    public class DifferentialBackup : BackupJob
    {
        public DifferentialBackup(string name, string sourceDir, string destinationDir)
            : base(name, sourceDir, destinationDir, "Differential")
        {
        }

        // Overrides the Start method to perform a differential backup
        public override void Start()
        {
            base.Start();
            InitializeTrackingProperties();
            UpdateState("ACTIVE");

            try
            {
                PerformDifferentialBackup(SourceDir, DestinationDir);
                UpdateProgress(null);
                UpdateState("END");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during backup: {ex.Message}");
                UpdateState("ERROR");
            }
        }

        // Initializes properties for tracking which files need to be copied
        private void InitializeTrackingProperties()
        {
            var allFiles = Directory.GetFiles(SourceDir, "*", SearchOption.AllDirectories);
            TotalFilesSize = allFiles.Sum(file => new FileInfo(file).Length);

            TotalFilesToCopy = allFiles.Count(file =>
            {
                string destFile = Path.Combine(DestinationDir, file.Substring(SourceDir.Length + 1));
                return ShouldCopyFile(file, destFile);
            });

            NbFilesLeftToDo = TotalFilesToCopy;
        }

        // Updates the progress of the backup process
        private void UpdateProgress(string fileCopied)
        {
            if (NbFilesLeftToDo > 0)
            {
                NbFilesLeftToDo--;
            }

            Progression = TotalFilesToCopy > 0 ? 100.0 * (TotalFilesToCopy - NbFilesLeftToDo) / TotalFilesToCopy : 100;
            UpdateState("ACTIVE");
        }


        // Performs the differential backup by copying only modified or new files
        private void PerformDifferentialBackup(string sourceDir, string destinationDir)
        {
            if (!Directory.Exists(destinationDir))
                Directory.CreateDirectory(destinationDir);

            foreach (var sourceFile in Directory.GetFiles(sourceDir))
            {
                var destFile = Path.Combine(destinationDir, Path.GetFileName(sourceFile));
                if (ShouldCopyFile(sourceFile, destFile))
                {
                    CopyFileWithBuffer(sourceFile, destFile);
                }
                else
                {
                    Console.WriteLine($"Unchanged: {sourceFile}");
                }
            }

            foreach (var directory in Directory.GetDirectories(sourceDir))
            {
                var destDir = Path.Combine(destinationDir, Path.GetFileName(directory));
                PerformDifferentialBackup(directory, destDir);
            }
        }

        // Determines if a file should be copied based on modification date and size
        private bool ShouldCopyFile(string sourceFile, string destinationFile)
        {
            if (!File.Exists(destinationFile))
            {
                Console.WriteLine($"New file: {sourceFile}");
                return true;
            }

            var sourceFileInfo = new FileInfo(sourceFile);
            var destFileInfo = new FileInfo(destinationFile);

            bool isModified = sourceFileInfo.LastWriteTime > destFileInfo.LastWriteTime ||
                              sourceFileInfo.Length != destFileInfo.Length;

            if (isModified)
            {
                Console.WriteLine($"Replacing: {sourceFile}");
                Console.WriteLine(
                    $"  Source Last Modified: {sourceFileInfo.LastWriteTime}, Size: {sourceFileInfo.Length} bytes");
                Console.WriteLine(
                    $"  Dest. Last Modified: {destFileInfo.LastWriteTime}, Size: {destFileInfo.Length} bytes");
            }

            return isModified;
        }
    }


    // Manages backup jobs, loading configurations, and executing specified jobs
    public class BackupManager
    {
        private List<BackupJob> _backupJobs = new List<BackupJob>();
        private ConfigModel _configModel = new ConfigModel();

        // Loads backup job configurations from a source and initializes jobs based on those configurations
        public void LoadBackupJobs()
        {
            var jobConfigs = _configModel.LoadBackupJobs();
            foreach (var jobConfig in jobConfigs)
            {
                AddBackupJobBasedOnType(jobConfig);
            }
        }

        // Adds a backup job to the list based on its type
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

        // Checks if a job with the specified name exists
        public bool JobExists(string jobName)
        {
            return _backupJobs.Any(job => job.Name.Equals(jobName, StringComparison.OrdinalIgnoreCase));
        }

        // Executes the specified backup jobs by name
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