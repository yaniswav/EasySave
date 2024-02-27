using System.Collections.ObjectModel;
using System.ComponentModel;
using EasySave; // Assurez-vous que le chemin d'accès est correct
using System;

namespace EasySave.ViewModels
{
    public class BackupVM : INotifyPropertyChanged
    {
        private ConfigModel _configModel;
        private ObservableCollection<BackupJobConfig> _backupJobs;

        public event PropertyChangedEventHandler PropertyChanged;

        public BackupVM()
        {
            _configModel = ConfigModel.Instance;
            BackupJobs = new ObservableCollection<BackupJobConfig>(_configModel.LoadBackupJobs());
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

        // Command pour rafraîchir la liste des travaux de sauvegarde
        public void RefreshBackupJobs()
        {
            BackupJobs.Clear();
            var jobs = _configModel.LoadBackupJobs();
            foreach (var job in jobs)
            {
                BackupJobs.Add(job);
            }
        }

        // Affiche une liste des travaux de sauvegarde configurés
        public void ListBackups()
        {
            Console.WriteLine();
            var backupJobs = _configModel.LoadBackupJobs();
            if (backupJobs.Count == 0)
            {
                Console.WriteLine("No backup jobs configured.");
            }
            else
            {
                foreach (var job in backupJobs)
                {
                    Console.WriteLine(
                        $"Name: {job.Name}, Source: {job.SourceDir}, Destination: {job.DestinationDir}, Type: {job.Type}");
                }
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}