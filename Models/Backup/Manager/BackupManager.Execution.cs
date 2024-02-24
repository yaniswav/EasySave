using System;
using System.Linq;
using System.Threading;

namespace EasySave
{
    public partial class BackupManager
    {
        private Dictionary<string, Thread> _backupThreads = new Dictionary<string, Thread>();

        public void ExecuteJobs(string[] jobNames)
        {
            foreach (string jobName in jobNames)
            {
                if (JobExists(jobName))
                {

                    if (!_backupThreads.ContainsKey(jobName) || !_backupThreads[jobName].IsAlive)
                    {
                        StartNewBackupJob(jobName);
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

        private void StartNewBackupJob(string jobName)
        {
            LogCurrentBackupThreads("Avant la création du thread");
            Console.WriteLine($"Starting execution of backup job '{jobName}'...");

            var jobToExecute = _backupJobs.First(job => job.Name.Equals(jobName, StringComparison.OrdinalIgnoreCase));
            var cancellationTokenSource = new CancellationTokenSource();
            var pauseEvent = new ManualResetEvent(true);

            var thread = InitializeJobThread(jobName, jobToExecute, cancellationTokenSource, pauseEvent);

            Console.WriteLine(
                $"Adding backup job '{jobName}' to the list of running jobs... with thread {thread.ManagedThreadId}.");

            _backupThreads.Add(jobName, thread);
            _cancellationTokens.Add(jobName, cancellationTokenSource);
            _pauseEvents.Add(jobName, pauseEvent);

            thread.Start();
            ListActiveThreads();
            Console.WriteLine($"[{DateTime.Now}] Starting execution of backup job '{jobName}'...");
        }

        private Thread InitializeJobThread(string jobName, BackupJob jobToExecute,
            CancellationTokenSource cancellationTokenSource, ManualResetEvent pauseEvent)
        {
            return new Thread(() =>
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
                    ThreadCleanup(jobName);
                    ListActiveThreads();

                }
            });
        }
    }
}