// FilePath: ViewModel/SettingsViewModel.cs

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using EasySave;

public class SettingsViewModel : INotifyPropertyChanged
{
    private ConfigModel _configModel = ConfigModel.Instance;

    public event PropertyChangedEventHandler PropertyChanged;

    // General Application Settings from ConfigModel
    public string CurrentLocale
    {
        get => _configModel.CurrentLocale;
        set
        {
            if (_configModel.CurrentLocale != value)
            {
                Console.WriteLine($"Updating CurrentLocale: {value}");
                _configModel.CurrentLocale = value;
                OnPropertyChanged(nameof(CurrentLocale));
            }
        }
    }

    // BackupJobConfig integration
    public List<BackupJobConfig> BackupJobs => _configModel.LoadBackupJobs();

    // FilesConfig integration
    public int MaxBackupFileSize
    {
        get => _configModel.MaxBackupFileSize;
        set
        {
            if (_configModel.MaxBackupFileSize != value)
            {
                _configModel.MaxBackupFileSize = value;
                OnPropertyChanged(nameof(MaxBackupFileSize));
            }
        }
    }


    public int MaxBackupSize
    {
        get => _configModel.MaxBackupFileSize;
        set
        {
            if (_configModel.MaxBackupFileSize != value)
            {
                Console.WriteLine($"Updating MaxBackupSize: {value}");
                _configModel.MaxBackupFileSize = value;
                OnPropertyChanged(nameof(MaxBackupSize));
            }
        }
    }

    public List<string> ExtToEncrypt
    {
        get => _configModel.ExtToEncrypt;
        set
        {
            if (!_configModel.ExtToEncrypt.SequenceEqual(value))
            {
                Console.WriteLine($"Updating ExtToEncrypt: {String.Join(", ", value)}");
                _configModel.ExtToEncrypt = value;
                OnPropertyChanged(nameof(ExtToEncrypt));
            }
        }
    }

    public List<string> ExtPrio
    {
        get => _configModel.ExtPrio;
        set
        {
            if (!_configModel.ExtPrio.SequenceEqual(value))
            {
                Console.WriteLine($"Updating ExtPrio: {String.Join(", ", value)}");
                _configModel.ExtPrio = value;
                OnPropertyChanged(nameof(ExtPrio));
            }
        }
    }

    public SettingsViewModel()
    {
        _configModel = ConfigModel.Instance;
    }

    protected virtual void OnPropertyChanged(string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}