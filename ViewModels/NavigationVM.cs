using System;
using System.Windows.Input;
using EasySave; 
using System.Threading;

namespace EasySave.ViewModels
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool>? _canExecute;
        private event EventHandler? _canExecuteChanged;

        public RelayCommand(Action<object> execute, Func<object, bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add => _canExecuteChanged += value;
            remove => _canExecuteChanged -= value;
        }

        public bool CanExecute(object? parameter) => _canExecute == null || _canExecute(parameter);

        public void Execute(object? parameter) => _execute(parameter);
    }

    public class BackupCommandViewModel
    {
        private readonly BackupManager _backupManager;
        public ICommand StartBackupCommand { get; }
        public ICommand PauseBackupCommand { get; }
        public ICommand ResumeBackupCommand { get; }
        public ICommand StopBackupCommand { get; }

        public BackupCommandViewModel()
        {
            _backupManager = new BackupManager();
            _backupManager.LoadBackupJobs(); // Load jobs at initialization

            StartBackupCommand = new RelayCommand(param => StartBackup(param), param => CanExecuteBackupCommand(param));
            PauseBackupCommand = new RelayCommand(param => PauseBackup(param), param => CanExecuteBackupCommand(param));
            ResumeBackupCommand = new RelayCommand(param => ResumeBackup(param), param => CanExecuteBackupCommand(param));
            StopBackupCommand = new RelayCommand(param => StopBackup(param), param => CanExecuteBackupCommand(param));
        }

        private bool CanExecuteBackupCommand(object param)
        {
            // Example validation to enable/disable the command
            return true; // Or implement actual validation logic
        }

        private void StartBackup(object param)
        {
            string jobName = param as string;
            if (!string.IsNullOrEmpty(jobName))
            {
                // Execute the backup job
                _backupManager.ExecuteJobs(new[] { jobName });
            }
        }

        private void PauseBackup(object param)
        {
            string jobName = param as string;
            if (!string.IsNullOrEmpty(jobName))
            {
                _backupManager.PauseJob(jobName);
            }
        }

        private void ResumeBackup(object param)
        {
            string jobName = param as string;
            if (!string.IsNullOrEmpty(jobName))
            {
                _backupManager.ResumeJob(jobName);
            }
        }

        private void StopBackup(object param)
        {
            string jobName = param as string;
            if (!string.IsNullOrEmpty(jobName))
            {
                _backupManager.StopJob(jobName);
            }
        }
    }
}
