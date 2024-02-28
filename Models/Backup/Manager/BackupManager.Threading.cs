using System;
using System.Collections.Generic;

namespace EasySave
{
    public partial class BackupManager
    {
        // Méthode privée pour nettoyer les threads et les ressources associées
        private void ThreadCleanup(string jobName)
        {
            if (_backupThreads.ContainsKey(jobName))
            {
                _backupThreads.Remove(jobName);
                _cancellationTokens.Remove(jobName);
                _pauseEvents.Remove(jobName);
            }
        }

        private void LogCurrentBackupThreads(string message)
        {
            Console.WriteLine($"--- {message} ---");
            foreach (var item in _backupThreads)
            {
                Console.WriteLine($"Job: {item.Key}, Thread State: {item.Value.ThreadState}");
            }
        }
        
        
        
    }
}