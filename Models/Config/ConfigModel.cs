using System;
using System.Configuration;
using System.Linq;

namespace EasySave;

public partial class ConfigModel
{
    private static ConfigModel _instance;

    private ConfigModel()
    {
        // Constructor is private to prevent instantiation
    }

    public static ConfigModel Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ConfigModel();
                // Load initial configuration settings
                _instance.LoadConfiguration();
            }

            return _instance;
        }
    }

    public void LoadConfiguration()
    {
        // Chargement de la configuration locale
        CurrentLocale = ConfigurationManager.AppSettings["CurrentLocale"] ?? "en-US";

        // Chargement du format de sortie pour la journalisation
        OutputFormat = ConfigurationManager.AppSettings["OutputFormat"] ?? "default";

        // Chargement de la taille maximale de fichier de sauvegarde
        MaxBackupFileSize = int.TryParse(ConfigurationManager.AppSettings["MaxBackupFileSize"], out int maxFileSize)
            ? maxFileSize
            : 0;

        // Chargement et conversion de ExtToEncrypt en liste
        var extToEncryptValue = ConfigurationManager.AppSettings["ExtToEncrypt"] ?? string.Empty;
        ExtToEncrypt = extToEncryptValue.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

        // Chargement et conversion de ExtPrio en liste
        var extPrioValue = ConfigurationManager.AppSettings["ExtPrio"] ?? string.Empty;
        ExtPrio = extPrioValue.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

        CryptoSoftPath = ConfigurationManager.AppSettings["CryptoSoftPath"] ?? string.Empty;

        // Chargement des paramètres pour le logiciel d'entreprise
        BusinessSoftware = ConfigurationManager.AppSettings["BusinessSoftwareKey"] ?? string.Empty;
    }

    public void UpdateAppSettings(string key, string value)
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