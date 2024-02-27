using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace EasySave.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private string _backupName;

    [ObservableProperty]
    private string _sourceDirectory;

    [ObservableProperty]
    private string _targetDirectory;

    [ObservableProperty]
    private bool _isBackupInProgress;

    // Utilisez [RelayCommand] pour générer la commande et son exécution automatiquement
    // Supprimez la déclaration explicite de la propriété StartBackupCommand dans le constructeur

    public MainWindowViewModel()
    {
    }

    // La méthode est transformée en commande grâce à l'attribut [RelayCommand]
    // La propriété StartBackupCommand est automatiquement générée avec son getter (et pas de setter nécessaire)
    [RelayCommand(CanExecute = nameof(CanStartBackup))]
    private void StartBackup()
    {
        // Implémentez la logique pour démarrer une sauvegarde ici
        IsBackupInProgress = true;
    }

    private bool CanStartBackup()
    {
        return !string.IsNullOrWhiteSpace(SourceDirectory) && 
               !string.IsNullOrWhiteSpace(TargetDirectory) && 
               !IsBackupInProgress;
    }

    // Le reste de votre code reste inchangé
}