namespace EasySaveConsole;

public class ConfigModel
{
    // BackupManager class (inherits from BackupJob)
    public class BackupManager
    {
        private List<BackupJob> backupJobs;

        public BackupManager()
        {
            backupJobs = new List<BackupJob>();
            LoadJobsFromJson(); // Load BackupJobs from JSON file
        }

        public void Create(BackupJob job)
        {
            backupJobs.Add(job);
            SaveJobsToJson();
        }

        public void Edit(string jobName, BackupJob updatedJob)
        {
            var job = backupJobs.Find(j => j.Name == jobName);
            if (job != null)
            {
                backupJobs.Remove(job);
                backupJobs.Add(updatedJob);
                SaveJobsToJson();
            }
            // TODO (handle error)
        }

        public void Delete(string jobName)
        {
            var job = backupJobs.Find(j => j.Name == jobName);
            if (job != null)
            {
                backupJobs.Remove(job);
                SaveJobsToJson();
            }
            // TODO (handle error)
        }

        private void SaveJobsToJson()
        {
            // TODO
        }

        private void LoadJobsFromJson()
        {
            // TODO
        }
    }
}