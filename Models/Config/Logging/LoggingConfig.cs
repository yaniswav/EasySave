using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;

namespace EasySave;

public static class ConfigurationHelper
{
    public static string GetOutputFormat()
    {
        return ConfigurationManager.AppSettings["OutputFormat"];
    }

    public static void SetOutputFormat(string format)
    {
        // Get the current configuration file
        Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        // Modify the OutputFormat setting
        config.AppSettings.Settings["OutputFormat"].Value = format;

        // Save the configuration file
        config.Save(ConfigurationSaveMode.Modified);

        // Refresh the appSettings section to reflect updated settings
        ConfigurationManager.RefreshSection("appSettings");
    }
}