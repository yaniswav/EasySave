using EasySave;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EasySave.ViewModels
{
    public class ModifyBackupVm : INotifyPropertyChanged
    {
        private ObservableCollection<BackupJobConfig> _backupJobConfigs;
        private BackupJobConfig _selectedBackupJobConfig;
        private ConfigModel _configModel = new ConfigModel();

        public event PropertyChangedEventHandler PropertyChanged;

        public ModifyBackupVm()
        {
            _backupJobConfigs = new ObservableCollection<BackupJobConfig>(_configModel.LoadBackupJobs());
        }

        public ObservableCollection<BackupJobConfig> BackupJobConfigs
        {
            get => _backupJobConfigs;
            set
            {
                _backupJobConfigs = value;
                OnPropertyChanged();
            }
        }

        public BackupJobConfig SelectedBackupJobConfig
        {
            get => _selectedBackupJobConfig;
            set
            {
                _selectedBackupJobConfig = value;
                OnPropertyChanged();
                // LoadExtensionsToEncrypt(); // This method will be adjusted or removed based on new logic.
            }
        }

        // Add or modify methods to interact with backup jobs through configurations rather than direct manipulation.

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}