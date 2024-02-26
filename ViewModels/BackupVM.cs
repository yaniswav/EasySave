using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;

namespace EasySave
{
    public class BackupVM
    {
        private readonly ResourceManager _resourceManager;
        private readonly ConfigModel _configModel;
        public BackupManager BackupManager { get; private set; }

        public BackupVM(ConfigModel configModel, Utilities messageDisplay)
        {
            _resourceManager = new ResourceManager("EasySave.Resources.Languages.Messages", typeof(Program).Assembly);
            _configModel = configModel ?? throw new ArgumentNullException(nameof(configModel), "ConfigModel is required");
            BackupManager = new BackupManager();
        }

        private bool IsValidBackupType(string type)
        {
            var validTypes = new[] { "Complete", "Differential" };
            return validTypes.Contains(type);
        }

        private bool IsValidPath(string path)
        {
            var validPathPrefixes = new List<string> { "C:\\", "D:\\", "E:\\", "F:\\", "G:\\", "H:\\" };
            return !string.IsNullOrWhiteSpace(path) && validPathPrefixes.Any(prefix => path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
        }

        public bool TryCreateBackup(string name, string sourceDir, string destinationDir, string type, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (_configModel.BackupJobExists(name))
            {
                errorMessage = _resourceManager.GetString("JobAlreadyExists", CultureInfo.CurrentUICulture);
                return false;
            }

            if (!IsValidPath(sourceDir) || !IsValidPath(destinationDir))
            {
                errorMessage = _resourceManager.GetString("InvalidPath", CultureInfo.CurrentUICulture);
                return false;
            }

            if (!IsValidBackupType(type))
            {
                errorMessage = _resourceManager.GetString("InvalidType", CultureInfo.CurrentUICulture);
                return false;
            }

            BackupJobConfig newJob = new BackupJobConfig { Name = name, SourceDir = sourceDir, DestinationDir = destinationDir, Type = type };
            _configModel.AddBackupJob(newJob);
            return true;
        }

        public bool TryEditBackup(string jobName, string newSourceDir, string newDestinationDir, string newType, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (!IsValidPath(newSourceDir) || !IsValidPath(newDestinationDir))
            {
                errorMessage = _resourceManager.GetString("InvalidPath", CultureInfo.CurrentUICulture);
                return false;
            }

            if (!IsValidBackupType(newType))
            {
                errorMessage = _resourceManager.GetString("InvalidType", CultureInfo.CurrentUICulture);
                return false;
            }

            BackupJobConfig modifiedJob = new BackupJobConfig { Name = jobName, SourceDir = newSourceDir, DestinationDir = newDestinationDir, Type = newType };
            _configModel.ModifyBackupJob(jobName, modifiedJob);
            return true;
        }

        public bool TryDeleteBackup(string jobName)
        {
            return _configModel.DeleteBackupJob(jobName);
        }

        public void ExecuteBackups()
        {
            Console.WriteLine();
            BackupManager.LoadBackupJobs();

            while (true)
            {
                Console.WriteLine(_resourceManager.GetString("ExecuteBackupMessage", CultureInfo.CurrentUICulture));
                string input = Console.ReadLine();

                if (string.Equals(input, "exit", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                string[] jobNames = input.Split(',');
                if (!ValidateJobNames(jobNames, BackupManager))
                {
                    Console.WriteLine($"Not valid: {string.Join(", ", jobNames)}");
                    Console.WriteLine(_resourceManager.GetString("InvalidEntry", CultureInfo.CurrentUICulture));
                    continue;
                }

                BackupManager.ExecuteJobs(jobNames);
                break;
            }
        }

        private static bool ValidateJobNames(string[] jobNames, BackupManager backupManager)
        {
            return jobNames.All(jobName => !string.IsNullOrWhiteSpace(jobName) && backupManager.JobExists(jobName.Trim()));
        }
    }
}
