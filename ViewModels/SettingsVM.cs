using System.Collections.Generic;
using System.Windows.Input;
using Newtonsoft.Json;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.Input;



namespace EasySave.ViewModels
{
    public class SettingsVM : ViewModelBase
    {
        private string _selectedLanguage;
        private List<string> _extensionsToEncrypt;
        private List<string> _priorityExtensions;
        private int _maxBackupFileSize;

        public SettingsVM()
        {
            SaveSettingsCommand = new RelayCommand(SaveSettings);
            // Load current settings from ConfigModel
        }

        // Properties for each setting
        // ICommand for saving settings

        private void SaveSettings(object parameter)
        {
            // Logic to save the modified settings back to ConfigModel
        }

        // Methods to load and validate settings
    }
}