using System;

namespace EasySave
{
    public partial class BackupManager
    {
        private Dictionary<string, CancellationTokenSource> _cancellationTokens =
            new Dictionary<string, CancellationTokenSource>();

        private Dictionary<string, ManualResetEvent> _pauseEvents = new Dictionary<string, ManualResetEvent>();

        // Méthode publique pour mettre en pause un travail de sauvegarde
        public void PauseJob(string jobName)
        {
            if (_pauseEvents.ContainsKey(jobName))
            {
                _pauseEvents[jobName].Reset();
                Console.WriteLine($"Backup job '{jobName}' paused.");
            }
        }

        // Méthode publique pour reprendre un travail de sauvegarde mis en pause
        public void ResumeJob(string jobName)
        {
            if (_pauseEvents.ContainsKey(jobName))
            {
                _pauseEvents[jobName].Set();
                Console.WriteLine($"Backup job '{jobName}' resumed.");
            }
        }

        // Méthode publique pour arrêter un travail de sauvegarde
        public void StopJob(string jobName)
        {
            if (_cancellationTokens.ContainsKey(jobName))
            {
                _cancellationTokens[jobName].Cancel();
                Console.WriteLine($"Backup job '{jobName}' stopped.");
            }
        }
    }
}