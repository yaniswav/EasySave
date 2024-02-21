using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using ReactiveUI;

namespace EasySave.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, INotifyDataErrorInfo
    {
        public void DeleteBackupJob(string jobName)
        {
            if (string.IsNullOrWhiteSpace(jobName))
            {
                Console.WriteLine("Le nom du travail de sauvegarde est vide.");
                return;
            }

            var isDeleted = _backupManager.DeleteJob(jobName);
            if (isDeleted)
            {
                Console.WriteLine($"Le travail de sauvegarde '{jobName}' a été supprimé.");
                // Mise à jour de l'interface utilisateur si nécessaire, par exemple, recharger la liste des travaux.
                LoadBackupJobs();
            }
            else
            {
                Console.WriteLine($"Le travail de sauvegarde '{jobName}' n'a pas été trouvé.");
            }
        }

        private readonly BackupManager _backupManager;
        private Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public bool HasErrors => _errors.Any();

        public MainWindowViewModel()
        {
            _backupManager = new BackupManager();
            LoadBackupJobs();
        }

        public void LoadBackupJobs()
        {
            ValidateBackupJobs(_backupManager.Jobs);
        }

        // Correction : Type de retour spécifié comme IEnumerable (non générique) à IEnumerable<string>
        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName) || !_errors.ContainsKey(propertyName))
            {
                // Correction : Retourne un IEnumerable vide plutôt que null
                return Enumerable.Empty<string>();
            }

            return _errors[propertyName];
        }

        private void ValidateBackupJobs(List<BackupJob> jobs)
        {
            foreach (var job in jobs)
            {
                if (string.IsNullOrWhiteSpace(job.SourceDir) || string.IsNullOrWhiteSpace(job.DestinationDir))
                {
                    AddError(job.Name, "Le répertoire source et destination ne peuvent pas être vides.");
                }
                else
                {
                    RemoveError(job.Name);
                }
            }
        }

        private void AddError(string propertyName, string error)
        {
            if (!_errors.ContainsKey(propertyName))
                _errors[propertyName] = new List<string>();

            if (!_errors[propertyName].Contains(error))
            {
                _errors[propertyName].Add(error);
                OnErrorsChanged(propertyName);
            }
        }

        private void RemoveError(string propertyName)
        {
            if (_errors.ContainsKey(propertyName))
            {
                _errors.Remove(propertyName);
                OnErrorsChanged(propertyName);
            }
        }

        private void OnErrorsChanged([CallerMemberName] string propertyName = null)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }
    }
}
