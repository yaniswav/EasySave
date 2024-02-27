using EasySave;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;

namespace EasySave.ViewModels
{
    public class ModifyBackupVm : INotifyPropertyChanged
    {
        private ObservableCollection<BackupJob> _backupJobs;
        private BackupJob _selectedBackupJob;
        private ObservableCollection<string> _extensionsToEncrypt;
        private string _newExtension;

        public event PropertyChangedEventHandler PropertyChanged;

        public ModifyBackupVm()
        {
            _backupJobs = new ObservableCollection<BackupJob>();
            _extensionsToEncrypt = new ObservableCollection<string>();
            LoadBackupJobs();
        }

        public ObservableCollection<BackupJob> BackupJobs
        {
            get => _backupJobs;
            set
            {
                _backupJobs = value;
                OnPropertyChanged();
            }
        }

        public BackupJob SelectedBackupJob
        {
            get => _selectedBackupJob;
            set
            {
                _selectedBackupJob = value;
                OnPropertyChanged();
                LoadExtensionsToEncrypt();
            }
        }

        public ObservableCollection<string> ExtensionsToEncrypt
        {
            get => _extensionsToEncrypt;
            set
            {
                _extensionsToEncrypt = value;
                OnPropertyChanged();
            }
        }

        public string NewExtension
        {
            get => _newExtension;
            set
            {
                _newExtension = value;
                OnPropertyChanged();
            }
        }

        public void AddExtension()
        {
            if (!string.IsNullOrWhiteSpace(NewExtension) && !ExtensionsToEncrypt.Contains(NewExtension))
            {
                ExtensionsToEncrypt.Add(NewExtension);
                SelectedBackupJob.ExtensionsToEncrypt.Add(NewExtension);
                NewExtension = string.Empty;
            }
        }

        public void RemoveExtension(string extension)
        {
            if (ExtensionsToEncrypt.Contains(extension))
            {
                ExtensionsToEncrypt.Remove(extension);
                SelectedBackupJob.ExtensionsToEncrypt.Remove(extension);
            }
        }

        private void LoadBackupJobs()
        {
            // Simuler le chargement des BackupJobs
            // Dans une application réelle, cette méthode récupérerait les jobs de sauvegarde depuis un gestionnaire de configuration ou une base de données
        }

        private void LoadExtensionsToEncrypt()
        {
            ExtensionsToEncrypt.Clear();
            if (SelectedBackupJob != null)
            {
                foreach (var extension in SelectedBackupJob.ExtensionsToEncrypt)
                {
                    ExtensionsToEncrypt.Add(extension);
                }
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
