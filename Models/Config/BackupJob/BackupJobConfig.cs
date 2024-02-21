using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;

namespace EasySave;

// Class to represent the configuration for a single backup job

public class BackupJobConfig
{
    public string Name { get; set; }
    public string SourceDir { get; set; }
    public string DestinationDir { get; set; }
    public string Type { get; set; }

    // Converts the backup job configuration to a string for storage
    public override string ToString()
    {
        return $"{Name},{SourceDir},{DestinationDir},{Type}";
    }

    // Creates a backup job configuration from a string
    public static BackupJobConfig FromString(string data)
    {
        var parts = data.Split(',');
        if (parts.Length != 4)
        {
            throw new FormatException("Invalid backup job data format.");
        }

        return new BackupJobConfig
        {
            Name = parts[0],
            SourceDir = parts[1],
            DestinationDir = parts[2],
            Type = parts[3]
        };
    }
}

public partial class ConfigModel
{
    private const string BackupJobsKey = "BackupJobs";

    public List<BackupJobConfig> LoadBackupJobs()
    {
        return GetBackupJobs();
    }

    public void AddBackupJob(BackupJobConfig jobConfig)
    {
        if (jobConfig == null)
        {
            throw new ArgumentNullException(nameof(jobConfig), "Backup job configuration cannot be null");
        }

        var backupJobs = GetBackupJobs();


        backupJobs.Add(jobConfig);
        SaveBackupJobs(backupJobs);
        Console.WriteLine($"Backup job {jobConfig.Name} added.");
    }

    public bool DeleteBackupJob(string jobName)
    {
        var backupJobs = GetBackupJobs();
        var jobToDelete =
            backupJobs.FirstOrDefault(job => job.Name.Equals(jobName, StringComparison.OrdinalIgnoreCase));

        if (jobToDelete != null)
        {
            backupJobs.Remove(jobToDelete);
            SaveBackupJobs(backupJobs);
            Console.WriteLine($"Backup job {jobName} deleted.");
            return true;
        }
        else
        {
            Console.WriteLine($"Backup job {jobName} not found.");
            return false;
        }
    }

    public void ModifyBackupJob(string jobName, BackupJobConfig modifiedJob)
    {
        var backupJobs = GetBackupJobs();
        var jobToModify =
            backupJobs.FirstOrDefault(job => job.Name.Equals(jobName, StringComparison.OrdinalIgnoreCase));

        if (jobToModify != null)
        {
            backupJobs.Remove(jobToModify);
            if (backupJobs.Any(job => job.Name.Equals(modifiedJob.Name, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine($"A backup job with the name '{modifiedJob.Name}' already exists.");
                return;
            }

            backupJobs.Add(modifiedJob);
            SaveBackupJobs(backupJobs);
            Console.WriteLine($"Backup job {jobName} modified.");
        }
        else
        {
            Console.WriteLine($"Backup job {jobName} not found.");
        }
    }

    private List<BackupJobConfig> GetBackupJobs()
    {
        try
        {
            string jobsData = ConfigurationManager.AppSettings[BackupJobsKey];
            if (string.IsNullOrEmpty(jobsData))
            {
                return new List<BackupJobConfig>();
            }

            var jobsList = new List<BackupJobConfig>();
            foreach (var jobStr in jobsData.Split(';'))
            {
                try
                {
                    jobsList.Add(BackupJobConfig.FromString(jobStr));
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error loading job config '{jobStr}': {e.Message}");
                }
            }

            return jobsList;
        }
        catch (ConfigurationErrorsException e)
        {
            Console.WriteLine($"Error retrieving backup jobs: {e.Message}");
            return new List<BackupJobConfig>();
        }
    }

    private void SaveBackupJobs(List<BackupJobConfig> backupJobs)
    {
        try
        {
            string jobsData = string.Join(";", backupJobs.Select(job => job.ToString()));
            UpdateAppSettings(BackupJobsKey, jobsData);
        }
        catch (ConfigurationErrorsException e)
        {
            Console.WriteLine($"Error saving backup jobs: {e.Message}");
        }
    }
}