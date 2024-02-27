using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace EasySave.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _selectedLanguage;

    [ObservableProperty]
    private bool _isDarkThemeEnabled;

    // La commande est générée automatiquement à partir de la méthode marquée par l'attribut [RelayCommand]
    [RelayCommand]
    private void ApplySettings()
    {
        // Logique pour appliquer les paramètres ici
        // Par exemple, mettre à jour la configuration de l'application avec les nouvelles valeurs
    }
}