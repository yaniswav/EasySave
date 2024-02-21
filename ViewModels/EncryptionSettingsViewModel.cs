using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;

namespace EasySave.ViewModels
{
    public class EncryptionSettingsViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        private List<string> _extensionsToEncrypt;
        private string _extensionToAdd = "";

        public EncryptionSettingsViewModel()
        {
            _extensionsToEncrypt = new List<string>();
        }

        public List<string> ExtensionsToEncrypt
        {
            get => _extensionsToEncrypt;
            set
            {
                _extensionsToEncrypt = value;
                OnPropertyChanged();
            }
        }

        public string ExtensionToAdd
        {
            get => _extensionToAdd;
            set
            {
                _extensionToAdd = value;
                OnPropertyChanged();
            }
        }

        public void AddExtension()
        {
            if (!string.IsNullOrWhiteSpace(_extensionToAdd) && !_extensionsToEncrypt.Contains(_extensionToAdd))
            {
                _extensionsToEncrypt.Add(_extensionToAdd);
                ExtensionToAdd = ""; // Reset after adding
                OnPropertyChanged(nameof(ExtensionsToEncrypt));
            }
        }

        public void RemoveExtension(string extension)
        {
            if (_extensionsToEncrypt.Contains(extension))
            {
                _extensionsToEncrypt.Remove(extension);
                OnPropertyChanged(nameof(ExtensionsToEncrypt));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Error => null;

        public string this[string columnName]
        {
            get
            {
                if (columnName == nameof(ExtensionToAdd))
                {
                    if (string.IsNullOrWhiteSpace(ExtensionToAdd))
                    {
                        return "Extension cannot be empty.";
                    }

                    if (ExtensionsToEncrypt.Contains(ExtensionToAdd))
                    {
                        return "Extension already added.";
                    }
                }
                return null;
            }
        }
    }
}
