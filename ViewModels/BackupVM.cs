using System.Collections.ObjectModel;
using System.ComponentModel;
using EasySave; // Make sure this namespace is correct
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace EasySave.ViewModels
{
    public class BackupVM : INotifyPropertyChanged
    {
        private ObservableCollection<BackupJob> _backupJobs;
        public ConfigModel _configModel = ConfigModel.Instance;


        public event PropertyChangedEventHandler PropertyChanged;

        public BackupVM()
        {
            _backupJobs = new ObservableCollection<BackupJob>();
            // Initialize with existing backup jobs if necessary
        }

        public ObservableCollection<BackupJob> BackupJobs
        {
            get => _backupJobs;
            set
            {
                _backupJobs = value;
                OnPropertyChanged(nameof(BackupJobs));
            }
        }
        
        public bool TryCreateBackup(string name, string sourceDir, string destinationDir, string type)
        {
            if (_configModel.BackupJobExists(name))
            {
                return false;
            }

            if (!IsValidPath(sourceDir) || !IsValidPath(destinationDir))
            {
                Console.WriteLine("Invalid create path");
                return false;
                
            }

            if (!IsValidBackupType(type))
            {
                Console.WriteLine("Invalid backup type");
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
            return true; // Indicate success
        }
        
        public bool TryEditBackup(string jobName, string newSourceDir, string newDestinationDir, string newType)
        {
            if (!_configModel.BackupJobExists(jobName))
            {
                return false;
            }
            
            if (!IsValidPath(newSourceDir) || !IsValidPath(newDestinationDir))
            {
                Console.WriteLine("Invalid edit path");
                return false;
            }

            if (!IsValidBackupType(newType))
            {
                Console.WriteLine("Invalid backup type");
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
            return true;
        }
        
        public bool IsValidBackupType(string type)
        {
            var validTypes = new[] { "Complete", "Differential" }; // Supported backup types
            return validTypes.Contains(type); // Check if the provided type is in the list of valid types
        }

        // Validates if the provided path is considered valid based on predefined criteria
        public bool IsValidPath(string path)
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

        public bool TryDeleteBackup(string jobName)
        {
            return _configModel.DeleteBackupJob(jobName);
        }

        // Additional methods to manage backup jobs (e.g., start, stop) could be added here

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}