using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EasySave.ViewModels
{
    public class BackupJobViewModel : ViewModelBase
    {
        private string _name;
        private string _sourceDir;
        private string _destinationDir;
        private string _type; // "Complete" ou "Differential"
        private ObservableCollection<string> _extensionsToEncrypt;
        private bool _isBusinessSoftwareRunning;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string SourceDir
        {
            get => _sourceDir;
            set => SetProperty(ref _sourceDir, value);
        }

        public string DestinationDir
        {
            get => _destinationDir;
            set => SetProperty(ref _destinationDir, value);
        }

        public string Type
        {
            get => _type;
            set => SetProperty(ref _type, value);
        }

        public ObservableCollection<string> ExtensionsToEncrypt
        {
            get => _extensionsToEncrypt;
            set => SetProperty(ref _extensionsToEncrypt, value);
        }

        public bool IsBusinessSoftwareRunning
        {
            get => _isBusinessSoftwareRunning;
            set => SetProperty(ref _isBusinessSoftwareRunning, value);
        }

        public BackupJobViewModel()
        {
            ExtensionsToEncrypt = new ObservableCollection<string>();
        }

        // Méthodes pour la logique de sauvegarde ici
    }
}