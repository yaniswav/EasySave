using System;
using System.Configuration;
using System.Globalization;

namespace EasySave;

public partial class ConfigModel
{
    private const string CurrentLocaleKey = "CurrentLocale";

    public static string CurrentLocale
    {
        get => ConfigurationManager.AppSettings[CurrentLocaleKey] ?? "en-US";
        set
        {
            UpdateAppSettings(CurrentLocaleKey, value);
            SetCulture(value);
        }
    }

    private static void SetCulture(string locale)
    {
        CultureInfo.CurrentCulture = new CultureInfo(locale);
        CultureInfo.CurrentUICulture = new CultureInfo(locale);
    }
}