using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;

namespace EasySaveConsole
{
    public class ConfigModel
    {
        private const string BackupJobsKey = "BackupJobs";
        private const string CurrentLocaleKey = "CurrentLocale";
        private const int MaxBackupJobs = 5;

        public string Locale { get; private set; }

        public ConfigModel()
        {
            LoadCurrentLocale();
        }

        public void LoadCurrentLocale()
        {
            Console.WriteLine("Loading current locale...");
            Locale = ConfigurationManager.AppSettings[CurrentLocaleKey] ?? "en-US";
            SetCulture(Locale);
        }

        public void SetLocale(string newLocale)
        {
            Console.WriteLine($"Setting new locale to: {newLocale}");
            UpdateAppSettings(CurrentLocaleKey, newLocale);
            SetCulture(newLocale);
        }

        private void SetCulture(string locale)
        {
            CultureInfo cultureInfo = new CultureInfo(locale);
            CultureInfo.CurrentCulture = cultureInfo;
            CultureInfo.CurrentUICulture = cultureInfo;
            Console.WriteLine($"Locale set to: {CultureInfo.CurrentCulture}");
        }

        public List<BackupJobConfig> LoadBackupJobs()
        {
            Console.WriteLine("Loading backup jobs...");
            return GetBackupJobs();
        }

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

        public void DeleteBackupJob(string jobName)
        {
            var backupJobs = GetBackupJobs();
            var jobToDelete =
                backupJobs.FirstOrDefault(job => job.Name.Equals(jobName, StringComparison.OrdinalIgnoreCase));

            if (jobToDelete != null)
            {
                backupJobs.Remove(jobToDelete);
                SaveBackupJobs(backupJobs);
                Console.WriteLine($"Backup job {jobName} deleted.");
            }
            else
            {
                Console.WriteLine($"Backup job {jobName} not found.");
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
            string jobsData = ConfigurationManager.AppSettings[BackupJobsKey];
            if (string.IsNullOrEmpty(jobsData))
            {
                return new List<BackupJobConfig>();
            }

            return jobsData.Split(';')
                .Select(jobStr => BackupJobConfig.FromString(jobStr))
                .ToList();
        }

        private void SaveBackupJobs(List<BackupJobConfig> backupJobs)
        {
            string jobsData = string.Join(";", backupJobs.Select(job => job.ToString()));
            UpdateAppSettings(BackupJobsKey, jobsData);
        }

        private void ValidateBackupJobCount(List<BackupJobConfig> backupJobs)
        {
            if (backupJobs.Count >= MaxBackupJobs)
            {
                throw new InvalidOperationException("Maximum number of backup jobs reached.");
            }
        }

        public bool BackupJobExists(string jobName)
        {
            var backupJobs = GetBackupJobs();
            return backupJobs.Any(job => job.Name.Equals(jobName, StringComparison.OrdinalIgnoreCase));
        }

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

    public class BackupJobConfig
    {
        public string Name { get; set; }
        public string SourceDir { get; set; }
        public string DestinationDir { get; set; }
        public string Type { get; set; }

        public override string ToString()
        {
            return $"{Name},{SourceDir},{DestinationDir},{Type}";
        }

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

    public class LocaleConfig
    {
        public string CurrentLocale { get; set; }
    }
}