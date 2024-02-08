using System;
using System.Globalization;
using System.Resources;

namespace EasySaveConsole
{
    public class ViewModel
    {
        private Utilities _messageDisplay;
        private ResourceManager _resourceManager;
        private ConfigModel _configModel;

        public ViewModel(ConfigModel configModel, Utilities messageDisplay)
        {
            var resourceManager = new ResourceManager("easySave_console.Resources.Messages", typeof(Program).Assembly);
            _messageDisplay = new Utilities(resourceManager);
            _configModel = configModel;
        }


        private bool IsValidBackupType(string type)
        {
            var validTypes = new[] { "Complete", "Differential" };
            return validTypes.Contains(type);
        }

        private bool IsValidPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return false;
            }

            var validPathPrefixes = new List<string>
                { "C:\\", "D:\\", "E:\\", "F:\\", "G:\\", "H:\\" };

            return validPathPrefixes.Any(prefix => path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
        }

        public bool TryCreateBackup(string name, string sourceDir, string destinationDir, string type,
            out string errorMessage)
        {
            if (_configModel.BackupJobExists(name))
            {
                errorMessage = "JobAlreadyExists";
                return false;
            }

            if (!IsValidPath(sourceDir) || !IsValidPath(destinationDir))
            {
                errorMessage = "InvalidPath";
                return false;
            }

            if (!IsValidBackupType(type))
            {
                errorMessage = "InvalidType";
                return false;
            }

            BackupJobConfig newJob = new BackupJobConfig
            {
                Name = name,
                SourceDir = sourceDir,
                DestinationDir = destinationDir,
                Type = type
            };

            _configModel.AddBackupJob(newJob);
            errorMessage = "";
            return true;
        }


        public bool TryEditBackup(string jobName, string newSourceDir, string newDestinationDir, string newType,
            out string errorMessage)
        {
            if (!IsValidPath(newSourceDir) || !IsValidPath(newDestinationDir))
            {
                errorMessage = "InvalidPath";
                return false;
            }

            if (!IsValidBackupType(newType))
            {
                errorMessage = "InvalidType";
                return false;
            }

            BackupJobConfig modifiedJob = new BackupJobConfig
            {
                Name = jobName,
                SourceDir = newSourceDir,
                DestinationDir = newDestinationDir,
                Type = newType
            };

            _configModel.ModifyBackupJob(jobName, modifiedJob);
            errorMessage = "";
            return true;
        }

        public bool TryDeleteBackup(string jobName)
        {
            _configModel.DeleteBackupJob(jobName);
            return true;
        }

        public void ExecuteBackups()
        {
            BackupManager backupManager = new BackupManager();
            backupManager.LoadBackupJobs();

            while (true)
            {
                _messageDisplay.DisplayMessage("ExecuteBackupMessage");
                string input = Console.ReadLine();


                if (string.Equals(input, "exit", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                string[] jobNames = input.Split(',');

                // Validation
                bool isValid = ValidateJobNames(jobNames, backupManager);
                if (!isValid)
                {
                    _messageDisplay.DisplayMessage("InvalidEntry");
                    continue;
                }

                backupManager.ExecuteJobs(jobNames);
                break;
            }
        }

        private static bool ValidateJobNames(string[] jobNames, BackupManager backupManager)
        {
            foreach (string jobName in jobNames)
            {
                if (string.IsNullOrWhiteSpace(jobName) || !backupManager.JobExists(jobName.Trim()))
                {
                    return false;
                }
            }

            return true;
        }
    }
}