using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace EasySave.ViewModels
{
    public class BackupSettingsViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        private Dictionary<string, List<string>> _propertyErrors = new Dictionary<string, List<string>>();

        private string _sourceDirectory;
        private string _targetDirectory;
        private List<string> _extensionsToEncrypt = new List<string>();
        private string _businessSoftwareName;

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public string SourceDirectory
        {
            get => _sourceDirectory;
            set
            {
                _sourceDirectory = value;
                OnPropertyChanged();
                ValidateProperty(nameof(SourceDirectory));
            }
        }

        public string TargetDirectory
        {
            get => _targetDirectory;
            set
            {
                _targetDirectory = value;
                OnPropertyChanged();
                ValidateProperty(nameof(TargetDirectory));
            }
        }

        public List<string> ExtensionsToEncrypt
        {
            get => _extensionsToEncrypt;
            set
            {
                _extensionsToEncrypt = value;
                OnPropertyChanged();
                // Pas besoin de validation spécifique ici, liste acceptée telle quelle
            }
        }

        public string BusinessSoftwareName
        {
            get => _businessSoftwareName;
            set
            {
                _businessSoftwareName = value;
                OnPropertyChanged();
                ValidateProperty(nameof(BusinessSoftwareName));
            }
        }

        public bool HasErrors => _propertyErrors.Any();

        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName) || !_propertyErrors.ContainsKey(propertyName))
            {
                return null;
            }

            return _propertyErrors[propertyName];
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ValidateProperty(string propertyName)
        {
            // Clear previous errors
            _propertyErrors.Remove(propertyName);

            switch (propertyName)
            {
                case nameof(SourceDirectory):
                    if (string.IsNullOrWhiteSpace(SourceDirectory))
                    {
                        AddError(propertyName, "Source directory cannot be empty.");
                    }
                    break;
                case nameof(TargetDirectory):
                    if (string.IsNullOrWhiteSpace(TargetDirectory))
                    {
                        AddError(propertyName, "Target directory cannot be empty.");
                    }
                    break;
                case nameof(BusinessSoftwareName):
                    // Exemple de validation supplémentaire si nécessaire
                    break;
            }

            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        private void AddError(string propertyName, string error)
        {
            if (!_propertyErrors.ContainsKey(propertyName))
            {
                _propertyErrors[propertyName] = new List<string>();
            }

            _propertyErrors[propertyName].Add(error);
        }
    }
}
