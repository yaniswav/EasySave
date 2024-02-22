using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace EasySave;

// Manages backup jobs, loading configurations, and executing specified jobs
public class BackupManager
{
    private ConfigModel _configModel = new ConfigModel();

    private List<BackupJob> _backupJobs = new List<BackupJob>();
    private Dictionary<string, Thread> _backupThreads = new Dictionary<string, Thread>();

    private Dictionary<string, CancellationTokenSource> _cancellationTokens =
        new Dictionary<string, CancellationTokenSource>();

    private Dictionary<string, ManualResetEvent> _pauseEvents = new Dictionary<string, ManualResetEvent>();

    // Loads backup job configurations from a source and initializes jobs based on those configurations
    public void LoadBackupJobs()
    {
        var jobConfigs = _configModel.LoadBackupJobs();
        foreach (var jobConfig in jobConfigs)
        {
            AddBackupJobBasedOnType(jobConfig);
        }
    }

    // Adds a backup job to the list based on its type
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

    public void PauseJob(string jobName)
    {
        if (_pauseEvents.ContainsKey(jobName))
        {
            _pauseEvents[jobName].Reset(); // Met en pause le thread
            Console.WriteLine($"Backup job '{jobName}' paused.");
        }
    }

    public void ResumeJob(string jobName)
    {
        if (_pauseEvents.ContainsKey(jobName))
        {
            _pauseEvents[jobName].Set(); // Reprend l'exécution du thread
            Console.WriteLine($"Backup job '{jobName}' resumed.");
        }
    }

    public void StopJob(string jobName)
    {
        if (_cancellationTokens.ContainsKey(jobName))
        {
            _cancellationTokens[jobName].Cancel(); // Envoie une demande d'arrêt au thread
            Console.WriteLine($"Backup job '{jobName}' stopped.");
        }
    }

    // Executes the specified backup jobs by name
    public void ExecuteJobs(string[] jobNames)
    {
        foreach (string jobName in jobNames)
        {
            if (JobExists(jobName) && !_backupThreads.ContainsKey(jobName))
            {
                var jobToExecute =
                    _backupJobs.First(job => job.Name.Equals(jobName, StringComparison.OrdinalIgnoreCase));
                var cancellationTokenSource = new CancellationTokenSource();
                var pauseEvent = new ManualResetEvent(true); // Initialisé à l'état signalé (non bloqué)
                var thread = new Thread(() =>
                {
                    // Boucle pour vérifier l'état du ManualResetEvent et du CancellationToken
                    while (!cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        pauseEvent.WaitOne(); // Attend si le ManualResetEvent est dans l'état non signalé (bloqué)
                        // Logique de sauvegarde ici
                    }
                });

                _backupThreads.Add(jobName, thread);
                _cancellationTokens.Add(jobName, cancellationTokenSource);
                _pauseEvents.Add(jobName, pauseEvent);
                thread.Start();
            }
        }
    }
    
    // Checks if a job with the specified name exists
    public bool JobExists(string jobName)
    {
        return _backupJobs.Any(job => job.Name.Equals(jobName, StringComparison.OrdinalIgnoreCase));
    }
}