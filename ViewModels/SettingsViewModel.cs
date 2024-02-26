using System.Collections.ObjectModel;
using ReactiveUI;
using EasySave; // Make sure this is the correct namespace

public class SettingsViewModel : ReactiveObject
{
    private string _selectedLanguage;
    private ConfigModel _localeConfig = new ConfigModel(); // Assuming ConfigModel is correct

    public SettingsViewModel()
    {
        Languages = new ObservableCollection<string> { "English", "Français" };
        SelectedLanguage = "English"; // Default language
    }

    public ObservableCollection<string> Languages { get; }

    public string SelectedLanguage
    {
        get => _selectedLanguage;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedLanguage, value);
            ChangeLanguage(value);
        }
    }

    private void ChangeLanguage(string language)
    {
        var locale = language == "Français" ? "fr-FR" : "en-US";
        _localeConfig.SetLocale(locale); // Use ConfigModel's method
    }
}