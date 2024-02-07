using System;
using System.Configuration;
using System.Globalization;

namespace EasySaveConsole
{
    public class LocaleModel
    {
        public string Locale { get; set; }

        public void LoadCurrentLocale()
        {
            try
            {
                var config = ConfigurationManager.GetSection("localeConfig") as LocaleConfigSection;
                if (config != null)
                {
                    Locale = config.CurrentLocale;
                    CultureInfo culture = new CultureInfo(Locale);
                    CultureInfo.CurrentUICulture = culture;
                    Console.WriteLine($"Locale set to: {CultureInfo.CurrentUICulture}");
                }
                else
                {
                    Console.WriteLine("Locale config not found. Falling back to default language.");
                    Locale = "en-US";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading language settings: {ex}");
            }
        }

        public void SetLocale(string newLocale)
        {
            Console.WriteLine($"Setting new locale to: {newLocale}");
            // Update the App.config file here as necessary
            Locale = newLocale;
        }
    }
}