using System;
using System.Windows.Input;
using EasySave; // Assuming EasySave is the namespace containing the Models
using System.Threading;

namespace EasySave.ViewModels
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }

    public class CommandManager
    {
        public static EventHandler RequerySuggested { get; set; }
    }

    public class BackupCommandViewModel
    {
        private readonly BackupManager _backupManager;

        public ICommand StartBackupCommand { get; private set; }
        public ICommand StopBackupCommand { get; private set; }

        public BackupCommandViewModel()
        {
            _backupManager = new BackupManager();
            _backupManager.LoadBackupJobs(); // Initialize backup jobs from configurations

            StartBackupCommand = new RelayCommand(
                execute: param => StartBackup(param),
                canExecute: _ => true); // Simplified for demonstration

            StopBackupCommand = new RelayCommand(
                execute: param => StopBackup(param),
                canExecute: _ => true); // Simplified for demonstration
        }

        private void StartBackup(object param)
        {
            if (param is string jobName && !string.IsNullOrWhiteSpace(jobName))
            {
                _backupManager.ExecuteJobs(new[] { jobName });
                Console.WriteLine($"Backup job '{jobName}' started.");
            }
        }

        private void StopBackup(object param)
        {
            if (param is string jobName && !string.IsNullOrWhiteSpace(jobName))
            {
                _backupManager.StopJob(jobName);
                Console.WriteLine($"Backup job '{jobName}' stopped.");
            }
        }
    }
}
