using Xunit;
using EasySave;
using System.Threading;

namespace EasySave.Tests
{
    public class BackupManagerTesting
    {
        private BackupManager _backupManager;

        public BackupManagerTesting()
        {
            // Initialisation de BackupManager avant chaque test
            _backupManager = new BackupManager();
        }

        [Fact]
        public void TestPauseAndResumeJob()
        {
            string jobName = "D";

            // Ajoutez ici le code pour démarrer un travail de sauvegarde avec jobName
            // Exemple: _backupManager.StartJob(jobName);

            // Pause du travail
            _backupManager.PauseJob(jobName);
            
            // Ajoutez ici le code pour vérifier que le travail est bien en pause
            // Exemple: Assert.True(CheckIfJobIsPaused(jobName));

            // Attente pour simuler le temps de pause
            Thread.Sleep(1000); // Attendre 1 seconde

            // Reprise du travail
            _backupManager.ResumeJob(jobName);
            // Ajoutez ici le code pour vérifier que le travail a repris
            // Exemple: Assert.False(CheckIfJobIsPaused(jobName));

            // Arrêt du travail (facultatif)
            _backupManager.StopJob(jobName);
            // Ajoutez ici le code pour vérifier que le travail est bien arrêté
            // Exemple: Assert.True(CheckIfJobIsStopped(jobName));
        }

        // Implémentez les méthodes CheckIfJobIsPaused et CheckIfJobIsStopped selon votre logique d'application
    }
}