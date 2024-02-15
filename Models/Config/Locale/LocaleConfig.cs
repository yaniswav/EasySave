using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;

namespace EasySave;

// Used to store the current locale configuration
public partial class ConfigModel

{
    private const string CurrentLocaleKey = "CurrentLocale";


    public string CurrentLocale { get; set; }

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
}