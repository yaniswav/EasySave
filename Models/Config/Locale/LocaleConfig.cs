using System;
using System.Configuration;
using System.Globalization;

namespace EasySave;

public partial class ConfigModel
{
    private const string CurrentLocaleKey = "CurrentLocale";

    public string CurrentLocale
    {
        get => ConfigurationManager.AppSettings[CurrentLocaleKey] ?? "en-US";
        set
        {
            UpdateAppSettings(CurrentLocaleKey, value);
            SetCulture(value);
        }
    }

    public string? Locale { get; set; }

    private static void SetCulture(string locale)
    {
        CultureInfo.CurrentCulture = new CultureInfo(locale);
        CultureInfo.CurrentUICulture = new CultureInfo(locale);
    }

    public void SetLocale(string selectedLocale)
    {
        throw new NotImplementedException();
    }
}