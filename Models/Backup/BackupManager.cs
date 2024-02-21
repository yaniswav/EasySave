using System;
using System.Collections.Generic;
using System.Linq;

namespace EasySave;

// Manages backup jobs, loading configurations, and executing specified jobs
public class BackupManager
{
    private List<BackupJob> _backupJobs = new List<BackupJob>();
    private ConfigModel _configModel = new ConfigModel();

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

    // Checks if a job with the specified name exists
    public bool JobExists(string jobName)
    {
        return _backupJobs.Any(job => job.Name.Equals(jobName, StringComparison.OrdinalIgnoreCase));
    }

    // Executes the specified backup jobs by name
    public void ExecuteJobs(string[] jobNames)
    {
        foreach (string jobName in jobNames)
        {
            BackupJob jobToExecute =
                _backupJobs.FirstOrDefault(job => job.Name.Equals(jobName, StringComparison.OrdinalIgnoreCase));
            if (jobToExecute != null)
            {
                jobToExecute.Start();
            }
            else
            {
                Console.WriteLine($"Backup job '{jobName}' not found.");
            }
        }
    }
}