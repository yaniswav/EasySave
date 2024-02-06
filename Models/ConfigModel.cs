using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace EasySaveConsole
{
    public class ConfigModel
    {
        private const string BackupConfigFilePath = "Resources/config.json";
        private const string LanguageConfigFilePath = "Resources/language.json";
        private const int MaxBackupJobs = 5;
        public string Language { get; private set; }

        public List<BackupJobConfig> LoadBackupJobs()
        {
            if (!File.Exists(BackupConfigFilePath))
            {
                return new List<BackupJobConfig>();
            }

            string json = File.ReadAllText(BackupConfigFilePath);
            return JsonSerializer.Deserialize<List<BackupJobConfig>>(json) ?? new List<BackupJobConfig>();
        }

        public void LoadCurrentLanguage()
        {
            if (!File.Exists(LanguageConfigFilePath))
            {
                throw new FileNotFoundException("Language file not found.");
            }

            string json = File.ReadAllText(LanguageConfigFilePath);
            var languageConfig = JsonSerializer.Deserialize<LanguageConfig>(json);

            if (languageConfig == null || string.IsNullOrEmpty(languageConfig.Language))
            {
                throw new InvalidOperationException("Error while loading or invalid language setting.");
            }

            Language = languageConfig.Language;
        }
        
        public void SetLanguage(string newLanguage)
        {
            var languageConfig = new LanguageConfig { Language = newLanguage };
            string json = JsonSerializer.Serialize(languageConfig, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(LanguageConfigFilePath, json);
            Language = newLanguage;
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

    public class LanguageConfig
    {
        public string Language { get; set; }
    }
}