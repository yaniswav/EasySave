using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using EasySave; // Assuming the namespace for BackupJob and related classes
using CommunityToolkit.Mvvm.ComponentModel;

namespace EasySave.ViewModels
{
    public class CommunicationVm 
    {
        private BackupJob _currentBackupJob; // A reference to the current backup job
        private string _extensionToAdd = "";

        public CommunicationVm(BackupJob currentBackupJob)
        {
            _currentBackupJob = currentBackupJob; // Initialize with an existing backup job
            if (_currentBackupJob.ExtensionsToEncrypt == null)
            {
                _currentBackupJob.ExtensionsToEncrypt = new List<string>(); // Ensure the list is initialized
            }
        }

        public List<string> ExtensionsToEncrypt
        {
            get => _currentBackupJob.ExtensionsToEncrypt;
            set
            {
                _currentBackupJob.ExtensionsToEncrypt = value;
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
            if (!string.IsNullOrWhiteSpace(_extensionToAdd) && !_currentBackupJob.ExtensionsToEncrypt.Contains(_extensionToAdd.ToLower()))
            {
                _currentBackupJob.ExtensionsToEncrypt.Add(_extensionToAdd.ToLower());
                ExtensionToAdd = ""; // Reset after adding
                OnPropertyChanged(nameof(ExtensionsToEncrypt));
            }
        }

        public void RemoveExtension(string extension)
        {
            if (_currentBackupJob.ExtensionsToEncrypt.Contains(extension.ToLower()))
            {
                _currentBackupJob.ExtensionsToEncrypt.Remove(extension.ToLower());
                OnPropertyChanged(nameof(ExtensionsToEncrypt));
            }
        }

      

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
           
        }
    }
}
