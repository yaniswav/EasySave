using EasySave.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Avalonia.Input;
﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;



namespace EasySave.ViewModels
{
    public class CreateBackupVM : ViewModelBase
    {
        private string _jobName;
        private string _sourceDirectory;
        private string _destinationDirectory;
        private string _backupType;

        public CreateBackupVM()
        {
            CreateBackupCommand = new RelayCommand(CreateBackup, CanCreateBackup);
        }

        public string JobName
        {
            get => _jobName;
            set => SetProperty(ref _jobName, value);
        }

        public string SourceDirectory
        {
            get => _sourceDirectory;
            set => SetProperty(ref _sourceDirectory, value);
        }

        public string DestinationDirectory
        {
            get => _destinationDirectory;
            set => SetProperty(ref _destinationDirectory, value);
        }

        public string BackupType
        {
            get => _backupType;
            set => SetProperty(ref _backupType, value);
        }

        public ICommand CreateBackupCommand { get; private set; }

        private void CreateBackup(object parameter)
        {
            // Your logic to create backup job
            // This could involve creating a BackupJobConfig and passing it to ConfigModel or directly to BackupManager
        }

        private bool CanCreateBackup(object parameter)
        {
            // Logic to determine if the backup can be created
            // For example, ensure all fields are filled out and valid
            return !string.IsNullOrEmpty(JobName) && !string.IsNullOrEmpty(SourceDirectory) &&
                   !string.IsNullOrEmpty(DestinationDirectory) && !string.IsNullOrEmpty(BackupType);
        }
    }
}