// Supposition : Le fichier est placé dans un dossier correspondant à cet espace de noms.
namespace EasySave.ViewModels
{
    using ReactiveUI;
    using System;
    using System.Reactive;

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

            _createBackupCommand = ReactiveCommand.Create(CreateBackup, canExecute);
        }

        private void CreateBackup()
        {
            // Implémentez ici la logique pour créer la sauvegarde.
            Console.WriteLine($"Création d'une sauvegarde: {BackupName}");
        }
    }
}
