using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EasySave.ViewModels
{
    public class BackupVM : INotifyPropertyChanged
    {
        private readonly ConfigModel _configModel;
        private List<BackupJobConfig> _backupJobs;

        public event PropertyChangedEventHandler PropertyChanged;

        public List<BackupJobConfig> BackupJobs
        {
            get => _backupJobs;
            private set
            {
                _backupJobs = value;
                OnPropertyChanged();
            }
        }

        public BackupVM(ConfigModel configModel)
        {
            _configModel = configModel ?? throw new ArgumentNullException(nameof(configModel));
            _backupJobs = new List<BackupJobConfig>();
        }

        // Method to notify the UI (or console) of property changes
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Method to load and display backup jobs
        public void ListBackups()
        {
            Console.WriteLine();
            BackupJobs = _configModel.LoadBackupJobs(); // Load the backup jobs
            if (BackupJobs.Count == 0)
            {
                Console.WriteLine("No backup jobs configured.");
            }
            else
            {
                foreach (var job in BackupJobs)
                {
                    Console.WriteLine($"Name: {job.Name}, Source: {job.SourceDir}, Destination: {job.DestinationDir}, Type: {job.Type}");
                }
            }
        }
    }
}