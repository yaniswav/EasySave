using System;
using System.Globalization;
using System.Resources;

namespace EasySaveConsole
{
    public class ViewModel
    {
        public static void ExecuteBackups()
        {
            BackupManager backupManager = new BackupManager();
            backupManager.LoadBackupJobs();

            while (true)
            {
                Console.Write(
                    "Entrez le nom du travail de sauvegarde à exécuter (séparés par une virgule) ou tapez 'exit' pour quitter: ");
                string input = Console.ReadLine();

                if (string.Equals(input, "exit", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                string[] jobNames = input.Split(',');

                // Validation
                bool isValid = ValidateJobNames(jobNames, backupManager);
                if (!isValid)
                {
                    Console.WriteLine("Entrée invalide. Veuillez réessayer.");
                    continue;
                }

                backupManager.ExecuteJobs(jobNames);
                break;
            }
        }

        private static bool ValidateJobNames(string[] jobNames, BackupManager backupManager)
        {
            foreach (string jobName in jobNames)
            {
                if (string.IsNullOrWhiteSpace(jobName) || !backupManager.JobExists(jobName.Trim()))
                {
                    return false;
                }
            }

            return true;
        }
    }
}