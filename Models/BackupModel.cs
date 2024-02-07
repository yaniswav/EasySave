using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Configuration;

namespace EasySaveConsole
{
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

        protected const int MaxBufferSize = 1024 * 1024; // 1 MB

        protected void CopyFileWithBuffer(string sourceFile, string destinationFile)
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

        protected int DetermineBufferSize(long fileSize)
        {
            if (fileSize <= MaxBufferSize)
                return (int)fileSize;
            else
                return MaxBufferSize;
        }


        protected void UpdateState(string state)
        {
            StateModel.UpdateBackupState(this, state, TotalFilesToCopy, TotalFilesSize, NbFilesLeftToDo, Progression,
                "state.json");
        }
    }

    public class CompleteBackup : BackupJob
    {
        public CompleteBackup(string name, string sourceDir, string destinationDir)
            : base(name, sourceDir, destinationDir, "Complete")
        {
        }

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

        private void InitializeTrackingProperties()
        {
            var allFiles = Directory.GetFiles(SourceDir, "*", SearchOption.AllDirectories);
            TotalFilesToCopy = allFiles.Length;
            TotalFilesSize = allFiles.Sum(file => new FileInfo(file).Length);
            NbFilesLeftToDo = TotalFilesToCopy;
        }

        private void UpdateProgress(string fileCopied)
        {
            if (NbFilesLeftToDo > 0)
            {
                NbFilesLeftToDo--;
            }

            Progression = TotalFilesToCopy > 0 ? 100.0 * (TotalFilesToCopy - NbFilesLeftToDo) / TotalFilesToCopy : 100;
            UpdateState("ACTIVE");
        }


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

    public class DifferentialBackup : BackupJob
    {
        public DifferentialBackup(string name, string sourceDir, string destinationDir)
            : base(name, sourceDir, destinationDir, "Differential")
        {
        }

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

        private void UpdateProgress(string fileCopied)
        {
            if (NbFilesLeftToDo > 0)
            {
                NbFilesLeftToDo--;
            }

            Progression = TotalFilesToCopy > 0 ? 100.0 * (TotalFilesToCopy - NbFilesLeftToDo) / TotalFilesToCopy : 100;
            UpdateState("ACTIVE");
        }


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

        public bool JobExists(string jobName)
        {
            return _backupJobs.Any(job => job.Name.Equals(jobName, StringComparison.OrdinalIgnoreCase));
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