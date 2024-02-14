using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;

namespace EasySaveConsole
{
    // Manages application configuration, including locales and backup job settings
    public class ConfigModel
    {
        // Constants to identify configuration keys and limits
        private const string BackupJobsKey = "BackupJobs";
        private const string CurrentLocaleKey = "CurrentLocale";
        private const int MaxBackupJobs = 5;

        public string Locale { get; private set; }

        // Constructor initializes the current locale from configuration
        public ConfigModel()
        {
            LoadCurrentLocale();
        }

        // Loads the current locale from the application settings, defaults to "en-US" if not set
        public void LoadCurrentLocale()
        {
            Locale = ConfigurationManager.AppSettings[CurrentLocaleKey] ?? "en-US";
            SetCulture(Locale);
        }

        // Sets a new locale and updates application settings
        public void SetLocale(string newLocale)
        {
            UpdateAppSettings(CurrentLocaleKey, newLocale);
            SetCulture(newLocale);
        }

        // Applies the specified culture settings to the application
        private void SetCulture(string locale)
        {
            CultureInfo cultureInfo = new CultureInfo(locale);
            CultureInfo.CurrentCulture = cultureInfo;
            CultureInfo.CurrentUICulture = cultureInfo;
        }

        // Loads backup job configurations from application settings
        public List<BackupJobConfig> LoadBackupJobs()
        {
            return GetBackupJobs();
        }

        // Adds a new backup job configuration, ensuring the job name is unique and the max count is not exceeded
        public void AddBackupJob(BackupJobConfig jobConfig)
        {
            var backupJobs = GetBackupJobs();
            ValidateBackupJobCount(backupJobs);

            if (backupJobs.Any(job => job.Name.Equals(jobConfig.Name, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine($"A backup job with the name '{jobConfig.Name}' already exists.");
                return;
            }


            backupJobs.Add(jobConfig);
            SaveBackupJobs(backupJobs);
            Console.WriteLine($"Backup job {jobConfig.Name} added.");
        }

        // Deletes a backup job configuration by name
        public bool DeleteBackupJob(string jobName)
        {
            var backupJobs = GetBackupJobs();
            var jobToDelete = backupJobs.FirstOrDefault(job => job.Name.Equals(jobName, StringComparison.OrdinalIgnoreCase));

            if (jobToDelete != null)
            {
                backupJobs.Remove(jobToDelete);
                SaveBackupJobs(backupJobs);
                Console.WriteLine($"Backup job {jobName} deleted."); // Cette ligne peut être supprimée si vous gérez les messages dans la vue.
                return true;
            }
            else
            {
                Console.WriteLine($"Backup job {jobName} not found."); // Cette ligne peut être supprimée si vous gérez les messages dans la vue.
                return false;
            }
        }


        // Modifies an existing backup job configuration
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


        // Retrieves backup job configurations from application settings
        private List<BackupJobConfig> GetBackupJobs()
        {
            string jobsData = ConfigurationManager.AppSettings[BackupJobsKey];
            if (string.IsNullOrEmpty(jobsData))
            {
                return new List<BackupJobConfig>();
            }

            return jobsData.Split(';')
                .Select(jobStr => BackupJobConfig.FromString(jobStr))
                .ToList();
        }

        // Saves updated backup job configurations to application settings
        private void SaveBackupJobs(List<BackupJobConfig> backupJobs)
        {
            string jobsData = string.Join(";", backupJobs.Select(job => job.ToString()));
            UpdateAppSettings(BackupJobsKey, jobsData);
        }

        // Ensures the total number of backup jobs does not exceed the maximum allowed
        private void ValidateBackupJobCount(List<BackupJobConfig> backupJobs)
        {
            if (backupJobs.Count >= MaxBackupJobs)
            {
                throw new InvalidOperationException("Maximum number of backup jobs reached.");
            }
        }

        // Checks if a backup job with the specified name exists
        public bool BackupJobExists(string jobName)
        {
            var backupJobs = GetBackupJobs();
            return backupJobs.Any(job => job.Name.Equals(jobName, StringComparison.OrdinalIgnoreCase));
        }

        // Updates a single application setting by key
        private static void UpdateAppSettings(string key, string value)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (config.AppSettings.Settings[key] != null)
            {
                config.AppSettings.Settings[key].Value = value;
            }
            else
            {
                config.AppSettings.Settings.Add(key, value);
            }

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
    }

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
            return new BackupJobConfig
            {
                Name = parts[0],
                SourceDir = parts[1],
                DestinationDir = parts[2],
                Type = parts[3]
            };
        }
    }

    // Used to store the current locale configuration
    public class LocaleConfig
    {
        public string CurrentLocale { get; set; }
    }
}