using System;
using System.ComponentModel;
using System.Windows.Input;
using EasySave; // Ensure this namespace is correct for access to Models

namespace EasySave.ViewModels
{
    public class SettingsVM : INotifyPropertyChanged
    {
        private ConfigModel _configModel = new ConfigModel();
        private string _selectedLocale;
        private string _logOutputFormat;
        private ICommand _saveSettingsCommand;

        public event PropertyChangedEventHandler PropertyChanged;

        public SettingsVM()
        {
            // Initialize settings with current values
            _selectedLocale = _configModel.Locale;
            _logOutputFormat = ConfigurationHelper.GetOutputFormat();
        }

        public string SelectedLocale
        {
            get => _selectedLocale;
            set
            {
                if (_selectedLocale != value)
                {
                    _selectedLocale = value;
                    OnPropertyChanged(nameof(SelectedLocale));
                }
            }
        }

        public string LogOutputFormat
        {
            get => _logOutputFormat;
            set
            {
                if (_logOutputFormat != value)
                {
                    _logOutputFormat = value;
                    OnPropertyChanged(nameof(LogOutputFormat));
                }
            }
        }

        public ICommand SaveSettingsCommand
        {
            get
            {
                return _saveSettingsCommand ?? (_saveSettingsCommand = new RelayCommand(param => SaveSettings(), param => CanSaveSettings()));
            }
        }

        private bool CanSaveSettings()
        {
            // Add logic here if there are conditions that must be met before saving
            return true;
        }

        private void SaveSettings()
        {
            try
            {
                // Save locale setting
                _configModel.SetLocale(SelectedLocale);

                // Save log output format
                ConfigurationHelper.SetOutputFormat(LogOutputFormat);

                // Potential additional settings save operations can be performed here

                Console.WriteLine("Settings have been successfully saved.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving settings: {ex.Message}");
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ConfigurationHelper
    {
        public static void SetOutputFormat(string logOutputFormat)
        {
            throw new NotImplementedException();
        }

        public static string? GetOutputFormat()
        {
            throw new NotImplementedException();
        }
    }

    // Assuming RelayCommand is implemented elsewhere in the project
}