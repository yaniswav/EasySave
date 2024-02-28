using EasySave.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using Windows.Media.ClosedCaptioning;
using System.IO;
using CommunityToolkit.Mvvm.Input;


namespace EasySave.ViewModels
{
    public class ModifyBackupVM : ViewModelBase
    {
        private BackupJobConfig _selectedBackupJob;
        private List<BackupJobConfig> _backupJobs;
        private readonly ConfigModel _configModel;

        public ModifyBackupVM()
        {
            _configModel = new ConfigModel();
            LoadBackupJobs();
            ModifyBackupCommand = new RelayCommand(ModifyBackup, CanModifyBackup);
        }

        // List of backup jobs for selection
        public List<BackupJobConfig> BackupJobs
        {
            get => _backupJobs;
            set => SetProperty(ref _backupJobs, value);
        }

        // The selected backup job to modify
        public BackupJobConfig SelectedBackupJob
        {
            get => _selectedBackupJob;
            set
            {
                if (SetProperty(ref _selectedBackupJob, value))
                {
                    // Update fields to reflect the selected job's configuration
                    JobName = _selectedBackupJob?.Name;
                    SourceDirectory = _selectedBackupJob?.SourceDir;
                    DestinationDirectory = _selectedBackupJob?.DestinationDir;
                    BackupType = _selectedBackupJob?.Type;
                }
            }
        }

        // Properties bound to the UI fields for editing
        public string JobName { get; set; }
        public string SourceDirectory { get; set; }
        public string DestinationDirectory { get; set; }
        public string BackupType { get; set; }

        // Command to save modifications
        public ICommand ModifyBackupCommand { get; }

        // Loads the backup jobs for selection
        private void LoadBackupJobs()
        {
            BackupJobs = _configModel.LoadBackupJobs();
        }

        // Modifies the selected backup job with new values
        private void ModifyBackup(object parameter)
        {
            if (SelectedBackupJob == null) return;

            var modifiedJobConfig = new BackupJobConfig
            {
                Name = JobName,
                SourceDir = SourceDirectory,
                DestinationDir = DestinationDirectory,
                Type = BackupType
            };

            _configModel.ModifyBackupJob(SelectedBackupJob.Name, modifiedJobConfig);
            LoadBackupJobs(); // Refresh the list to reflect changes

            // Optionally, notify the user about the successful modification
            System.Windows.MessageBox.Show($"Backup job '{JobName}' has been successfully modified.");
        }

        // Determines whether the ModifyBackup command can execute
        private bool CanModifyBackup(object parameter)
        {
            return SelectedBackupJob != null &&
                   !string.IsNullOrWhiteSpace(JobName) &&
                   !string.IsNullOrWhiteSpace(SourceDirectory) &&
                   !string.IsNullOrWhiteSpace(DestinationDirectory) &&
                   !string.IsNullOrWhiteSpace(BackupType);
        }
    }
}
