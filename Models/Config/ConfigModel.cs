using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;

namespace EasySave
{
    public class ConfigModel
    {
        private const string BackupJobsKey = "BackupJobs";
        private const string CurrentLocaleKey = "CurrentLocale";

        public string Locale { get; private set; }

        public ConfigModel()
        {
            LoadCurrentLocale();
        }

        public void LoadCurrentLocale()
        {
            try
            {
                Locale = ConfigurationManager.AppSettings[CurrentLocaleKey] ?? "en-US";
            }
            catch (ConfigurationErrorsException e)
            {
                Console.WriteLine($"Error loading locale: {e.Message}");
                Locale = "en-US"; // Default to "en-US" on error
            }

            SetCulture(Locale);
        }

        public void SetLocale(string newLocale)
        {
            if (string.IsNullOrWhiteSpace(newLocale))
            {
                throw new ArgumentException("Locale cannot be null or whitespace", nameof(newLocale));
            }

            try
            {
                UpdateAppSettings(CurrentLocaleKey, newLocale);
            }
            catch (ConfigurationErrorsException e)
            {
                Console.WriteLine($"Error setting locale: {e.Message}");
                return;
            }

            SetCulture(newLocale);
        }

        private void SetCulture(string locale)
        {
            CultureInfo cultureInfo = new CultureInfo(locale);
            CultureInfo.CurrentCulture = cultureInfo;
            CultureInfo.CurrentUICulture = cultureInfo;
        }

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

            if (backupJobs.Any(job => job.Name.Equals(jobConfig.Name, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine($"A backup job with the name '{jobConfig.Name}' already exists.");
                return;
            }

            backupJobs.Add(jobConfig);
            SaveBackupJobs(backupJobs);
            Console.WriteLine($"Backup job {jobConfig.Name} added.");
        }

        public bool DeleteBackupJob(string jobName)
        {
            if (string.IsNullOrWhiteSpace(jobName))
            {
                throw new ArgumentException("Job name cannot be null or whitespace", nameof(jobName));
            }

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
            if (string.IsNullOrWhiteSpace(jobName))
            {
                throw new ArgumentException("Job name cannot be null or whitespace", nameof(jobName));
            }

            if (modifiedJob == null)
            {
                throw new ArgumentNullException(nameof(modifiedJob), "Modified job configuration cannot be null");
            }

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

        public bool BackupJobExists(string jobName)
        {
            if (string.IsNullOrWhiteSpace(jobName))
            {
                throw new ArgumentException("Job name cannot be null or whitespace", nameof(jobName));
            }

            var backupJobs = GetBackupJobs();
            return backupJobs.Any(job => job.Name.Equals(jobName, StringComparison.OrdinalIgnoreCase));
        }

        private static void UpdateAppSettings(string key, string value)
        {
            try
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
            catch (ConfigurationErrorsException e)
            {
                Console.WriteLine($"Error updating app settings: {e.Message}");
            }
        }
    }
}