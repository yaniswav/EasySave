using System.Collections.ObjectModel;
using System.ComponentModel;
using EasySave; // Assurez-vous que le chemin d'accès est correct
using System;
using System.Linq;
using System.Threading;

namespace EasySave.ViewModels
{
    public class BackupVM : INotifyPropertyChanged
    {
        private ObservableCollection<BackupJobConfig> _backupJobs;
        private ConfigModel _configModel;

        public event PropertyChangedEventHandler PropertyChanged;

        public BackupVM()
        {
            _configModel = new ConfigModel();
            LoadBackupJobs(); // Chargement des jobs de sauvegarde à partir des configurations
        }

        public ObservableCollection<BackupJobConfig> BackupJobs
        {
            get => _backupJobs;
            set
            {
                _backupJobs = value;
                OnPropertyChanged(nameof(BackupJobs));
            }
        }

        // Charge les jobs de sauvegarde depuis la configuration
        private void LoadBackupJobs()
        {
            var jobs = new ObservableCollection<BackupJobConfig>(_configModel.LoadBackupJobs());
            BackupJobs = jobs;
        }

        // Actualise la liste des jobs de sauvegarde
        public void RefreshBackupJobs()
        {
            LoadBackupJobs(); // Rechargement des configurations
        }

        // Ajoute un nouveau job de sauvegarde
        public void AddBackupJob(string name, string sourceDir, string destinationDir, string type)
        {
            var jobConfig = new BackupJobConfig
            {
                Name = name,
                SourceDir = sourceDir,
                DestinationDir = destinationDir,
                Type = type
            };

            _configModel.AddBackupJob(jobConfig); // Persiste la configuration
            RefreshBackupJobs(); // Actualise la liste des jobs
        }

        // Supprime un job de sauvegarde
        public void DeleteBackupJob(string jobName)
        {
            _configModel.DeleteBackupJob(jobName); // Supprime la configuration
            RefreshBackupJobs(); // Actualise la liste des jobs
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
