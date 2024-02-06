using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace EasySaveConsole
{
    public class ConfigModel
    {
        private const string BackupConfigFilePath = "Resources/backupConfig.json";
        private const string LocaleConfigFilePath = "Resources/localesConfig.json";
        private const int MaxBackupJobs = 5;
        public string Locale { get; set; }

        public List<BackupJobConfig> LoadBackupJobs()
        {
            if (!File.Exists(BackupConfigFilePath))
            {
                return new List<BackupJobConfig>();
            }

            string json = File.ReadAllText(BackupConfigFilePath);
            return JsonSerializer.Deserialize<List<BackupJobConfig>>(json) ?? new List<BackupJobConfig>();
        }

        public void LoadCurrentLocale()
        {
            try
            {
                if (!File.Exists(LocaleConfigFilePath))
                {
                    Console.WriteLine($"Warning: Locale file not found at '{LocaleConfigFilePath}'. Falling back to default language.");
                    Locale = "en-US";
                    return;
                }

                string json = File.ReadAllText(LocaleConfigFilePath);
                var localesConfig = JsonSerializer.Deserialize<LocaleConfig>(json);

                if (localesConfig == null || string.IsNullOrEmpty(localesConfig.CurrentLocale))
                {
                    throw new InvalidOperationException("Error while loading or invalid current locale setting.");
                }

                Locale = localesConfig.CurrentLocale;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading language settings: {ex.Message}");
            }
        }
        
        public void SetLocale(string newLocale)
        {
            var languageConfig = new LocaleConfig { CurrentLocale = newLocale };
            string json = JsonSerializer.Serialize(languageConfig, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(LocaleConfigFilePath, json);
            Locale = newLocale;
        }

        public void AddBackupJob(BackupJobConfig jobConfig)
        {
            var backupJobs = LoadBackupJobs();

            if (backupJobs.Count >= MaxBackupJobs)
            {
                throw new InvalidOperationException("Maximum number of backup jobs reached.");
            }

            backupJobs.Add(jobConfig);
            string json = JsonSerializer.Serialize(backupJobs, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(BackupConfigFilePath, json);
        }
    }

    public class BackupJobConfig
    {
        public string Name { get; set; }
        public string SourceDir { get; set; }
        public string DestinationDir { get; set; }
        public string Type { get; set; }
    }

    public class LocaleConfig
    {
        public string CurrentLocale { get; set; }
    }
}