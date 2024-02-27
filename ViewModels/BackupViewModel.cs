using System.Collections.ObjectModel;
using System.Windows.Input;
using ReactiveUI;
using EasySave;

namespace EasySave.ViewModels
{
    
    public class BackupViewModel : ReactiveObject
    {
        public ObservableCollection<BackupJob> Jobs { get; }

        // Déclaration des commandes
        public ICommand CreateBackupCommand { get; }
        public ICommand EditBackupCommand { get; }
        public ICommand DeleteBackupCommand { get; }
        public ICommand ExecuteBackupCommand { get; }
        public ICommand PauseBackupCommand { get; }
        public ICommand ResumeBackupCommand { get; }
        public ICommand StopBackupCommand { get; }

        public BackupViewModel()
        {
            Jobs = new ObservableCollection<BackupJob>();

            // Initialisation des commandes
            CreateBackupCommand = ReactiveCommand.Create(CreateBackup);
            EditBackupCommand = ReactiveCommand.Create(EditBackup);
            DeleteBackupCommand = ReactiveCommand.Create(DeleteBackup);
            ExecuteBackupCommand = ReactiveCommand.Create(ExecuteBackup);
            PauseBackupCommand = ReactiveCommand.Create(PauseBackup);
            ResumeBackupCommand = ReactiveCommand.Create(ResumeBackup);
            StopBackupCommand = ReactiveCommand.Create(StopBackup);
        }

        private void CreateBackup()
        {
            // Implémenter la logique de création de sauvegarde
        }

        private void EditBackup()
        {
            // Implémenter la logique de modification de sauvegarde
        }

        private void DeleteBackup()
        {
            // Implémenter la logique de suppression de sauvegarde
        }

        private void ExecuteBackup()
        {
            // Implémenter la logique d'exécution de sauvegarde
        }

        private void PauseBackup()
        {
            // Implémenter la logique de pause de sauvegarde
        }

        private void ResumeBackup()
        {
            // Implémenter la logique de reprise de sauvegarde
        }

        private void StopBackup()
        {
            // Implémenter la logique d'arrêt de sauvegarde
        }
    }
}
