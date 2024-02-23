using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace EasySave
{
    public class BackupManager
    {
        private ConfigModel _configModel = new ConfigModel();
        private List<BackupJob> _backupJobs = new List<BackupJob>();
        private Dictionary<string, Thread> _backupThreads = new Dictionary<string, Thread>();

        private Dictionary<string, CancellationTokenSource> _cancellationTokens =
            new Dictionary<string, CancellationTokenSource>();

        private Dictionary<string, ManualResetEvent> _pauseEvents = new Dictionary<string, ManualResetEvent>();

        // Constructeur
        public BackupManager()
        {
        }

        // Méthode publique pour charger les travaux de sauvegarde
        public void LoadBackupJobs()
        {
            var jobConfigs = _configModel.LoadBackupJobs();
            foreach (var jobConfig in jobConfigs)
            {
                AddBackupJobBasedOnType(jobConfig);
            }
        }

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

        // Méthode publique pour vérifier si un travail de sauvegarde existe
        public bool JobExists(string jobName)
        {
            return _backupJobs.Any(job => job.Name.Equals(jobName, StringComparison.OrdinalIgnoreCase));
        }

        // Méthode publique pour récupérer la progression de la sauvegarde
        public Dictionary<string, double> GetBackupProgress()
        {
            return _backupJobs.ToDictionary(job => job.Name, job => job.Progression);
        }

        // Propriété publique pour vérifier si tous les travaux de sauvegarde sont terminés
        public bool AllJobsCompleted
        {
            get { return _backupThreads.Values.All(thread => !thread.IsAlive); }
        }

        // Méthode publique pour exécuter les travaux de sauvegarde
        public void ExecuteJobs(string[] jobNames)
        {
            foreach (string jobName in jobNames)
            {
                if (JobExists(jobName))
                {
                    if (!_backupThreads.ContainsKey(jobName))
                    {
                        LogCurrentBackupThreads("Avant la création du thread");
                        Console.WriteLine($"Starting execution of backup job '{jobName}'...");
                        var jobToExecute = _backupJobs.First(job =>
                            job.Name.Equals(jobName, StringComparison.OrdinalIgnoreCase));
                        var cancellationTokenSource = new CancellationTokenSource();
                        var pauseEvent = new ManualResetEvent(true);

                        var thread = new Thread(() =>
                        {
                            try
                            {
                                Console.WriteLine(
                                    $"[{DateTime.Now}] Backup job '{jobName}' starting on thread {Thread.CurrentThread.ManagedThreadId}.");
                                jobToExecute.Start(cancellationTokenSource.Token, pauseEvent);
                                Console.WriteLine(
                                    $"[{DateTime.Now}] Backup job '{jobName}' completed on thread {Thread.CurrentThread.ManagedThreadId}.");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Exception occurred in backup job '{jobName}': {ex.Message}");
                            }
                            finally
                            {
                                // Nettoyage après la fin du travail
                                ThreadCleanup(jobName);
                            }
                        });

                        Console.WriteLine(
                            $"Adding backup job '{jobName}' to the list of running jobs... with thread {thread.ManagedThreadId}.");
                        Console.WriteLine($"Cancellation token source: {cancellationTokenSource.Token.CanBeCanceled}");
                        Console.WriteLine($"Pause event: {pauseEvent.WaitOne(0)}");

                        _backupThreads.Add(jobName, thread);
                        _cancellationTokens.Add(jobName, cancellationTokenSource);
                        _pauseEvents.Add(jobName, pauseEvent);
                        thread.Start();
                        Console.WriteLine($"[{DateTime.Now}] Starting execution of backup job '{jobName}'...");
                    }
                    else
                    {
                        Console.WriteLine($"Backup job '{jobName}' already running.");
                    }
                }
                else
                {
                    Console.WriteLine($"Backup job '{jobName}' not found for execution.");
                }
            }

            LogCurrentBackupThreads("Après l'ajout du thread");
        }


        private void LogCurrentBackupThreads(string message)
        {
            Console.WriteLine($"--- {message} ---");
            foreach (var item in _backupThreads)
            {
                Console.WriteLine($"Job: {item.Key}, Thread State: {item.Value.ThreadState}");
            }
        }


        // Méthode privée pour ajouter un travail de sauvegarde en fonction du type
        private void AddBackupJobBasedOnType(dynamic job)
        {
            string type = job.Type;
            string name = job.Name;
            string sourceDir = job.SourceDir;
            string destinationDir = job.DestinationDir;

            switch (type)
            {
                case "Complete":
                    _backupJobs.Add(new CompleteBackup(name, sourceDir, destinationDir));
                    break;
                case "Differential":
                    _backupJobs.Add(new DifferentialBackup(name, sourceDir, destinationDir));
                    break;
                default:
                    throw new InvalidOperationException("Unknown backup type");
            }
        }

        // Méthode privée pour nettoyer les threads et les ressources associées
        private void ThreadCleanup(string jobName)
        {
            if (_backupThreads.ContainsKey(jobName))
            {
                _backupThreads.Remove(jobName);
                _cancellationTokens.Remove(jobName);
                _pauseEvents.Remove(jobName);
                Console.WriteLine($"Cleanup of backup job '{jobName}' completed.");
                LogCurrentBackupThreads("Après nettoyage");
            }
        }
    }
}