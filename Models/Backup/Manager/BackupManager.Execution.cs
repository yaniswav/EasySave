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

        }

        private void StartNewBackupJob(string jobName)
        {

            var jobToExecute = _backupJobs.First(job => job.Name.Equals(jobName, StringComparison.OrdinalIgnoreCase));
            var cancellationTokenSource = new CancellationTokenSource();
            var pauseEvent = new ManualResetEvent(true);

            var thread = InitializeJobThread(jobName, jobToExecute, cancellationTokenSource, pauseEvent);


            _backupThreads.Add(jobName, thread);
            _cancellationTokens.Add(jobName, cancellationTokenSource);
            _pauseEvents.Add(jobName, pauseEvent);

            thread.Start();
        }

        private Thread InitializeJobThread(string jobName, BackupJob jobToExecute,
            CancellationTokenSource cancellationTokenSource, ManualResetEvent pauseEvent)
        {
            return new Thread(() =>
            {
                try
                {
                    jobToExecute.Start(cancellationTokenSource.Token, pauseEvent);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception occurred in backup job '{jobName}': {ex.Message}");
                }
                finally
                {
                    ThreadCleanup(jobName);
                }
            });
        }
    }
}