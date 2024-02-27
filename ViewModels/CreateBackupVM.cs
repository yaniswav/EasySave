using System;
using ReactiveUI;
using System.Reactive;
using EasySave;
using System.Threading;

namespace EasySave.ViewModels
{
    public class CreateBackupVM : ReactiveObject
    {
        private string _backupName = string.Empty;
        private string _sourceDirectory = string.Empty;
        private string _targetDirectory = string.Empty;
        private string _backupType = string.Empty;
        private readonly ReactiveCommand<Unit, Unit> _createBackupCommand;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private ManualResetEvent _pauseEvent = new ManualResetEvent(true);

        public string BackupName
        {
            get => _backupName;
            set => this.RaiseAndSetIfChanged(ref _backupName, value);
        }

        public string SourceDirectory
        {
            get => _sourceDirectory;
            set => this.RaiseAndSetIfChanged(ref _sourceDirectory, value);
        }

        public string TargetDirectory
        {
            get => _targetDirectory;
            set => this.RaiseAndSetIfChanged(ref _targetDirectory, value);
        }

        public string BackupType
        {
            get => _backupType;
            set => this.RaiseAndSetIfChanged(ref _backupType, value);
        }

        public ReactiveCommand<Unit, Unit> CreateBackupCommand => _createBackupCommand;

        public CreateBackupVM()
        {
            var canExecute = this.WhenAnyValue(
                x => x.BackupName,
                x => x.SourceDirectory,
                x => x.TargetDirectory,
                x => x.BackupType,
                (name, source, target, type) =>
                    !string.IsNullOrWhiteSpace(name) &&
                    !string.IsNullOrWhiteSpace(source) &&
                    !string.IsNullOrWhiteSpace(target) &&
                    !string.IsNullOrWhiteSpace(type));

            _createBackupCommand = ReactiveCommand.Create(ExecuteBackup, canExecute);
        }

        private void ExecuteBackup()
        {
            BackupJob backupJob;
            switch (BackupType)
            {
                case "Complete":
                    backupJob = new CompleteBackup(BackupName, SourceDirectory, TargetDirectory);
                    break;
                case "Differential":
                    backupJob = new DifferentialBackup(BackupName, SourceDirectory, TargetDirectory);
                    break;
                default:
                    throw new InvalidOperationException("Unsupported backup type.");
            }

            try
            {
                // Execute the backup job with proper threading and error handling
                Thread backupThread = new Thread(() =>
                {
                    backupJob.Start(_cancellationTokenSource.Token, _pauseEvent);
                });
                backupThread.Start();
                Console.WriteLine($"Backup {BackupName} started.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting backup {BackupName}: {ex.Message}");
            }
        }

        public void PauseBackup()
        {
            _pauseEvent.Reset();
            Console.WriteLine("Backup paused.");
        }

        public void ResumeBackup()
        {
            _pauseEvent.Set();
            Console.WriteLine("Backup resumed.");
        }

        public void CancelBackup()
        {
            _cancellationTokenSource.Cancel();
            Console.WriteLine("Backup cancelled.");
        }
    }
}
