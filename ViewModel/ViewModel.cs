using System;
using System.Globalization;
using System.Resources;

namespace EasySave
{
    // This class manage the interaction between the view and the model, handling business logic
    public class ViewModel
    {
        private Utilities _messageDisplay;
        private ResourceManager _resourceManager;
        private ConfigModel _configModel;
        public BackupManager BackupManager { get; private set; }

        public ViewModel(ConfigModel configModel, Utilities messageDisplay)
        {
            // Initialize resources and utilities
            var resourceManager =
                new ResourceManager("EasySave.Resources.Languages.Messages", typeof(Program).Assembly);
            _messageDisplay = new Utilities(resourceManager);
            _configModel = configModel;
            BackupManager = new BackupManager();
        }


        // Validates if the provided backup type is valid
        private bool IsValidBackupType(string type)
        {
            var validTypes = new[] { "Complete", "Differential" }; // Supported backup types
            return validTypes.Contains(type); // Check if the provided type is in the list of valid types
        }

        // Validates if the provided path is considered valid based on predefined criteria
        private bool IsValidPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return false; // Invalid if the path is null, empty, or whitespace
            }

            // List of valid path prefixes (e.g., drives on a Windows system)
            var validPathPrefixes = new List<string>
                { "C:\\", "D:\\", "E:\\", "F:\\", "G:\\", "H:\\" };

            // Check if the path starts with any of the valid prefixes
            return validPathPrefixes.Any(prefix => path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
        }

        // Tries to create a backup job with the provided details, returns false if unsuccessful with an error message
        public bool TryCreateBackup(string name, string sourceDir, string destinationDir, string type,
            out string errorMessage)
        {
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

            // Create and add the new backup job configuration
            BackupJobConfig newJob = new BackupJobConfig
            {
                Name = name,
                SourceDir = sourceDir,
                DestinationDir = destinationDir,
                Type = type
            };

            _configModel.AddBackupJob(newJob); // Add the job to the configuration
            errorMessage = "";
            return true; // Indicate success
        }


        // Tries to edit an existing backup job, returns false if unsuccessful with an error message
        public bool TryEditBackup(string jobName, string newSourceDir, string newDestinationDir, string newType,
            out string errorMessage)
        {
            // Validate new paths and backup type
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

            // Modify the existing backup job with new details
            BackupJobConfig modifiedJob = new BackupJobConfig
            {
                Name = jobName,
                SourceDir = newSourceDir,
                DestinationDir = newDestinationDir,
                Type = newType
            };

            _configModel.ModifyBackupJob(jobName, modifiedJob); // Apply the changes
            errorMessage = "";
            return true;
        }

        // Tries to delete an existing backup job, always returns true indicating success
        public bool TryDeleteBackup(string jobName)
        {
            return _configModel.DeleteBackupJob(jobName);
        }


        // Initiates the execution of backup jobs based on user input
        public void ExecuteBackups()
        {
            Console.WriteLine();
            BackupManager.LoadBackupJobs();

            while (true)
            {
                _messageDisplay.DisplayMessage("ExecuteBackupMessage"); // Prompt user for input
                string input = Console.ReadLine();


                // Exit loop if user types "exit"
                if (string.Equals(input, "exit", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                string[] jobNames = input.Split(','); // Split input into individual job names

                // Validation
                bool isValid = ValidateJobNames(jobNames, BackupManager);
                if (!isValid)
                {
                    Console.WriteLine($"Not valid: {string.Join(", ", jobNames)}");
                    _messageDisplay.DisplayMessage("InvalidEntry");
                    continue;
                }

                // Execute specified backup jobs
                BackupManager.ExecuteJobs(jobNames);
                break;
            }
        }

        // Validates the existence of specified backup job names
        private static bool ValidateJobNames(string[] jobNames, BackupManager backupManager)
        {
            foreach (string jobName in jobNames)
            {
                // Check if each job name is non-empty and exists
                if (string.IsNullOrWhiteSpace(jobName) || !backupManager.JobExists(jobName.Trim()))
                {
                    return false;
                }
            }

            return true; // True if all job names are valid
        }
    }
}