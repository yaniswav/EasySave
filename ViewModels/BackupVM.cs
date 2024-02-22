using System.ComponentModel;
using System.Runtime.CompilerServices; // Nécessaire pour [CallerMemberName]

namespace EasySave.ViewModels
{
    // Correction du nom de classe selon la convention de nommage suggérée
    public class BackupVm : INotifyPropertyChanged
    {
        // Initialisation des champs comme nullable pour gérer la non-assignation initiale
        private string? _name;
        private string? _sourceDirectory;
        private string? _targetDirectory;
        private string? _backupType;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string? Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public string? SourceDirectory
        {
            get => _sourceDirectory;
            set
            {
                _sourceDirectory = value;
                OnPropertyChanged();
            }
        }

        public string? TargetDirectory
        {
            get => _targetDirectory;
            set
            {
                _targetDirectory = value;
                OnPropertyChanged();
            }
        }

        public string? BackupType
        {
            get => _backupType;
            set
            {
                _backupType = value;
                OnPropertyChanged();
            }
        }

        // Utilisation de [CallerMemberName] pour éviter de devoir spécifier le nom de la propriété manuellement
        // Cela rend le code moins sujet aux erreurs et plus facile à maintenir
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}