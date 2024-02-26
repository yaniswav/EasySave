using System;
using System.Collections.Generic;
using System.Linq;

namespace EasySave
{
    public partial class BackupManager
    {
        private List<BackupJob> _backupJobs = new List<BackupJob>();

        public BackupManager()
        {
            // Construct
        }

        public async Task LoadBackupJobs()
        {
            var jobConfigs = await Task.Run(() => ConfigModel.Instance.LoadBackupJobs());
            foreach (var jobConfig in jobConfigs)
            {
                await AddBackupJobBasedOnType(jobConfig);
            }
        }

        public bool JobExists(string jobName)
        {
            return _backupJobs.Any(job => job.Name.Equals(jobName, StringComparison.OrdinalIgnoreCase));
        }

        private async Task AddBackupJobBasedOnType(dynamic job)
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

        // Propriété publique pour vérifier si tous les travaux de sauvegarde sont terminés
        public bool AllJobsCompleted
        {
            get { return _backupThreads.Values.All(thread => !thread.IsAlive); }
        }
    }
}