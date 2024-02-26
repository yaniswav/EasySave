using System.ComponentModel;
using System.Windows.Input;

namespace EasySave.ViewModels
{
    public class SettingsVM : INotifyPropertyChanged
    {
        private string? _backupLocation;
        private bool _enableNotifications;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string? BackupLocation
        {
            get => _backupLocation;
            set
            {
                _backupLocation = value;
                OnPropertyChanged(nameof(BackupLocation));
            }
        }

        public bool EnableNotifications
        {
            get => _enableNotifications;
            set
            {
                _enableNotifications = value;
                OnPropertyChanged(nameof(EnableNotifications));
            }
        }

        private ICommand? _saveSettingsCommand;
        public ICommand SaveSettingsCommand => _saveSettingsCommand ??= new RelayCommand(SaveSettings);

        private void SaveSettings()
        {
            // Logique pour sauvegarder les paramètres.
            // Exemple : SettingsManager.Save(BackupLocation, EnableNotifications);
        }

        protected void OnPropertyChanged(string? propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}