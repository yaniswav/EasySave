using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Globalization;

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
            Console.WriteLine("Loading backup jobs...");

            if (!File.Exists(BackupConfigFilePath))
            {
                Console.WriteLine("Backup config file not found.");
                return new List<BackupJobConfig>();
            }

            string json = File.ReadAllText(BackupConfigFilePath);
            Console.WriteLine($"Backup config loaded: {json}");
            var backupJobs = JsonSerializer.Deserialize<List<BackupJobConfig>>(json);
            if (backupJobs == null)
            {
                Console.WriteLine("Deserialization of backup jobs returned null.");
                return new List<BackupJobConfig>();
            }

            Console.WriteLine("Backup jobs deserialized successfully.");
            return backupJobs;
        }

        public void LoadCurrentLocale()
        {
            Console.WriteLine("Loading current locale...");
            try
            {
                if (!File.Exists(LocaleConfigFilePath))
                {
                    Console.WriteLine(
                        $"Warning: Locale file not found at '{LocaleConfigFilePath}'. Falling back to default language.");
                    Locale = "en-US";
                    return;
                }

                string json = File.ReadAllText(LocaleConfigFilePath);
                Console.WriteLine($"Locale config loaded: {json}");

                var localesConfig = JsonSerializer.Deserialize<LocaleConfig>(json);
                if (localesConfig == null || string.IsNullOrEmpty(localesConfig.CurrentLocale))
                {
                    throw new InvalidOperationException("Error while loading or invalid current locale setting.");
                }

                Locale = localesConfig.CurrentLocale;
                
                Console.WriteLine($"Locale we got : {Locale}");
                
                CultureInfo culture = Locale == "fr" ? new CultureInfo("fr-FR") : new CultureInfo("en-US");
                CultureInfo.CurrentUICulture = culture;
                Console.WriteLine($"Locale set to: {Locale}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading language settings: {ex.ToString()}");
            }
        }

        public void SetLocale(string newLocale)
        {
            Console.WriteLine($"Setting new locale to: {newLocale}");
            var languageConfig = new LocaleConfig { CurrentLocale = newLocale };
            string json = JsonSerializer.Serialize(languageConfig, new JsonSerializerOptions { WriteIndented = true });
            Console.WriteLine($"Locale config to be written: {json}");
            File.WriteAllText(LocaleConfigFilePath, json);
            Locale = newLocale;
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
            Console.WriteLine($"Backup job {jobConfig.Name} added.");
            string json = JsonSerializer.Serialize(backupJobs, new JsonSerializerOptions { WriteIndented = true });
            Console.WriteLine($"Updated backup job list: {json}");
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