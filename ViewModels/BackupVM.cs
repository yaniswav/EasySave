using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace EasySave.ViewModels
{
    public class BackupVM : ViewModelBase
    {
        private BackupManager _backupManager;
        public ObservableCollection<BackupJobViewModel> BackupJobs { get; set; }

        public BackupVM()
        {
            _backupManager = new BackupManager();
            BackupJobs = new ObservableCollection<BackupJobViewModel>();
            LoadBackupJobs();

            ExecuteBackupCommand = new RelayCommand(ExecuteBackup, CanExecuteBackupCommand);
            PauseBackupCommand = new RelayCommand(PauseBackup, CanExecuteBackupCommand);
            ResumeBackupCommand = new RelayCommand(ResumeBackup, CanExecuteBackupCommand);
            StopBackupCommand = new RelayCommand(StopBackup, CanExecuteBackupCommand);
        }

        public ICommand ExecuteBackupCommand { get; }
        public ICommand PauseBackupCommand { get; }
        public ICommand ResumeBackupCommand { get; }
        public ICommand StopBackupCommand { get; }

        private void LoadBackupJobs()
        {
            // Assuming LoadBackupJobs fetches jobs from the BackupManager and populates BackupJobs
            // This is just a placeholder. Implement according to your application's logic.
            BackupJobs.Clear();
            foreach (var job in _backupManager.GetBackupJobs())
            {
                BackupJobs.Add(new BackupJobViewModel(job));
            }
        }

        private void ExecuteBackup(object parameter)
        {
            var selectedBackups = BackupJobs.Where(bj => bj.IsSelected).Select(bj => bj.Job).ToList();
            _backupManager.ExecuteBackup(selectedBackups);
            OnPropertyChanged(nameof(BackupJobs)); // Notify UI to update
        }

        private void PauseBackup(object parameter)
        {
            var selectedBackups = BackupJobs.Where(bj => bj.IsSelected).Select(bj => bj.Job).ToList();
            _backupManager.PauseBackups(selectedBackups);
            OnPropertyChanged(nameof(BackupJobs)); // Notify UI to update
        }

        private void ResumeBackup(object parameter)
        {
            var selectedBackups = BackupJobs.Where(bj => bj.IsSelected).Select(bj => bj.Job).ToList();
            _backupManager.ResumeBackups(selectedBackups);
            OnPropertyChanged(nameof(BackupJobs)); // Notify UI to update
        }

        private void StopBackup(object parameter)
        {
            var selectedBackups = BackupJobs.Where(bj => bj.IsSelected).Select(bj => bj.Job).ToList();
            _backupManager.StopBackups(selectedBackups);
            OnPropertyChanged(nameof(BackupJobs)); // Notify UI to update
        }

        private bool CanExecuteBackupCommand(object parameter)
        {
            // Adjust logic here if necessary, for instance:
            // Check if any job is selected before allowing commands to execute.
            return BackupJobs.Any(bj => bj.IsSelected);
        }
    }

    // Assuming BackupJobViewModel represents a BackupJob in the UI, including selection state.
    public class BackupJobViewModel : ViewModelBase
    {
        public BackupJob Job { get; }
        private bool _isSelected;

        public BackupJobViewModel(BackupJob job)
        {
            Job = job;
        }

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }
}
