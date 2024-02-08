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
            CultureInfo cultureInfo = new CultureInfo(Locale);
            CultureInfo.CurrentCulture = cultureInfo;
            CultureInfo.CurrentUICulture = cultureInfo; // Ajoutez cette ligne
            Console.WriteLine($"Locale set to: {CultureInfo.CurrentCulture}");
        }


        public void SetLocale(string newLocale)
        {
            Console.WriteLine($"Setting new locale to: {newLocale}");
            UpdateAppSettings(CurrentLocaleKey, newLocale);
            LoadCurrentLocale(); // Reload locale to update CultureInfo
        }

        public List<BackupJobConfig> LoadBackupJobs()
        {
            Console.WriteLine("Loading backup jobs...");
            string jobsData = ConfigurationManager.AppSettings[BackupJobsKey];
            if (string.IsNullOrEmpty(jobsData))
            {
                Console.WriteLine("No backup jobs found.");
                return new List<BackupJobConfig>();
            }

            return jobsData.Split(';')
                .Select(jobStr => BackupJobConfig.FromString(jobStr))
                .ToList();
        }

        public void AddBackupJob(BackupJobConfig jobConfig)
        {
            Console.WriteLine($"Adding backup job: {jobConfig.Name}");
            var backupJobs = LoadBackupJobs();

            if (backupJobs.Count >= MaxBackupJobs)
            {
                Console.WriteLine("Reached maximum backup job limit.");
                throw new InvalidOperationException("Maximum number of backup jobs reached.");
            }

            backupJobs.Add(jobConfig);
            string jobsData = string.Join(";", backupJobs.Select(job => job.ToString()));
            UpdateAppSettings(BackupJobsKey, jobsData);
            Console.WriteLine($"Backup job {jobConfig.Name} added.");
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