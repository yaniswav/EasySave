using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using EasySaveConsole;

namespace EasySave.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly BackupManager _backupManager;

        public ObservableCollection<BackupJob> BackupJobs { get; }

        public ICommand StartBackupCommand { get; }
        public ICommand LoadBackupJobsCommand { get; }

        public MainWindowViewModel()
        {
            BackupJobs = new ObservableCollection<BackupJob>();
            _backupManager = new BackupManager();

            // Utilisation de la méthode correcte pour charger les jobs de sauvegarde
            LoadBackupJobs();

            LoadBackupJobsCommand = new RelayCommand(_ => LoadBackupJobs());
            StartBackupCommand = new RelayCommand(StartBackup, CanStartBackup);
        }

        // Méthode pour charger les jobs de sauvegarde à partir de BackupManager
        private void LoadBackupJobs()
        {
            BackupJobs.Clear();
            _backupManager.LoadBackupJobs(); // Charge les jobs à partir de la configuration
            foreach (var job in _backupManager.Jobs) // Utilise la nouvelle propriété publique Jobs de BackupManager
            {
                BackupJobs.Add(job);
            }
        }

        private bool CanStartBackup(object parameter)
        {
            // Logique pour déterminer si la commande StartBackup peut s'exécuter
            return parameter is string jobName && !string.IsNullOrWhiteSpace(jobName);
        }

        private void StartBackup(object parameter)
        {
            if (parameter is string jobName)
            {
                _backupManager.ExecuteJobs(new string[] { jobName });
            }
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public event EventHandler CanExecuteChanged;

        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke(parameter) ?? true;
        }

        public void Execute(object parameter)
        {
            _execute?.Invoke(parameter);
        }

        public void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
