using System;
using ReactiveUI;
using System.Reactive;
using EasySave; // Ensure this namespace matches where your Models are located
using System.Threading.Tasks;

namespace EasySave.ViewModels
{
    public class CreateBackupVM : ReactiveObject
    {
        private string _backupName = string.Empty;
        private string _sourceDirectory = string.Empty;
        private string _targetDirectory = string.Empty;
        private string _backupType = string.Empty;
        private readonly ReactiveCommand<Unit, Unit> _createBackupCommand;

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

            _createBackupCommand = ReactiveCommand.CreateFromTask(ExecuteBackupAsync, canExecute);
        }

        private async Task ExecuteBackupAsync()
        {
            BackupJob backupJob = BackupType switch
            {
                "Complete" => new CompleteBackup(BackupName, SourceDirectory, TargetDirectory),
                "Differential" => new DifferentialBackup(BackupName, SourceDirectory, TargetDirectory),
                _ => throw new InvalidOperationException("Unsupported backup type.")
            };

            try
            {
                // No need for explicit threading, Task.Run can be used if necessary
                await Task.Run(() => backupJob.Start());
                Console.WriteLine($"Backup {BackupName} started successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting backup {BackupName}: {ex.Message}");
            }
        }
    }
}
