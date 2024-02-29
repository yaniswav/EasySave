using Avalonia.Controls;
using Avalonia.Interactivity;
using EasySave.ViewModels;
using System.Threading.Tasks;

namespace EasySave.Views;

public partial class BackupView : UserControl
{
    public BackupView()
    {
        InitializeComponent();
        this.DataContext = new BackupVM();
    }

    private async void OnSelectSourceDirectoryClick(object sender, RoutedEventArgs e)
    {
        var selectionWindow = new FileTypeView();
        var mainWindow = this.VisualRoot as Window;
        await selectionWindow.ShowDialog(mainWindow); 
        if (!string.IsNullOrEmpty(selectionWindow.SelectedPath))
        {
            SourceDirectoryTextBox.Text = selectionWindow.SelectedPath;
        }
    }

    private async void OnSelectTargetDirectoryClick(object sender, RoutedEventArgs e)
    {
        var selectionWindow = new FileTypeView();
        var mainWindow = this.VisualRoot as Window;
        await selectionWindow.ShowDialog(mainWindow);
        if (!string.IsNullOrEmpty(selectionWindow.SelectedPath))
        {
            TargetDirectoryTextBox.Text = selectionWindow.SelectedPath;
        }
    }
    
    private void OnCreateButtonClick(object sender, RoutedEventArgs e)
    {
        var viewModel = this.DataContext as BackupVM;
        if (viewModel == null) return;

        var name = NameTextBox.Text;
        var sourceDir = SourceDirectoryTextBox.Text;
        var destinationDir = TargetDirectoryTextBox.Text;
        var typeItem = TypeChoice.SelectedItem as ComboBoxItem;
        var typeContent = typeItem?.Content.ToString();
    
        if (!viewModel.TryCreateBackup(name, sourceDir, destinationDir, typeContent)) {
            if (viewModel._configModel.BackupJobExists(name))
            {
                ShowErrorMessage(NameError, $"Une sauvegarde nommée '{name}' existe déjà.");
            }
            if (!viewModel.IsValidPath(sourceDir))
            {
                ShowErrorMessage(SourceError, "Chemin incrorrect.");
            }
            if (!viewModel.IsValidPath(destinationDir)) {
                ShowErrorMessage(DestinationError, "Chemin incrorrect.");
            }
            if (!viewModel.IsValidBackupType(typeContent)) {
                ShowErrorMessage(TypeError, "Erreur, choisir un type de sauvegarde");
            }
        }
        else
        {
            ShowSuccessMessage("Sauvegarde créée");
            ResetFields();
        }
    }
    
    private void OnEditButtonClick(object sender, RoutedEventArgs e)
    {
        var viewModel = this.DataContext as BackupVM; 
        if (viewModel == null) return; 
        
        var jobName = NameTextBox.Text; 
        var newSourceDir = SourceDirectoryTextBox.Text; 
        var newDestinationDir = TargetDirectoryTextBox.Text; 
        var typeItem = TypeChoice.SelectedItem as ComboBoxItem;
        var newType = typeItem?.Content.ToString();
        
        if (!viewModel.TryEditBackup(jobName, newSourceDir, newDestinationDir, newType)) {
            if (!viewModel._configModel.BackupJobExists(jobName))
            {
                ShowErrorMessage(NameError, $"Aucune sauvegarde nommée '{jobName}'.");
            }
            if (!viewModel.IsValidPath(newSourceDir))
            {
                ShowErrorMessage(SourceError, "Chemin incrorrect.");
            }
            if (!viewModel.IsValidPath(newDestinationDir)) {
                ShowErrorMessage(DestinationError, "Chemin incrorrect.");
            }
            if (!viewModel.IsValidBackupType(newType)) {
                ShowErrorMessage(TypeError, "Erreur, choisir un type de sauvegarde");
            }
        }
        else
        {
            ShowSuccessMessage("Sauvegarde modifiée");
            ResetFields();
        }
    }
    
    private void OnDeleteButtonClick(object sender, RoutedEventArgs e)
    {
        var viewModel = this.DataContext as BackupVM; 
        if (viewModel == null) return; 

        var jobName = NameTextBox.Text; 
        
        if (!viewModel.TryDeleteBackup(jobName))
        {
            if (!viewModel._configModel.BackupJobExists(jobName))
            {
                ShowErrorMessage(NameError, $"Aucune sauvegarde nommée '{jobName}'.");
            }
        }
        else
        {
            ShowSuccessMessage("Sauvegarde supprimée");
            ResetFields();
        }
    }

    
    private void OnExecuteButtonClick(object sender, RoutedEventArgs e)
    {
        // TODO: Add logic for deleting a backup
    }
    
    private void OnPauseButtonClick(object sender, RoutedEventArgs e)
    {
        // TODO: Add logic for deleting a backup
    }
    
    private void OnPlayButtonClick(object sender, RoutedEventArgs e)
    {
        // TODO: Add logic for deleting a backup
    }
    
    private void OnStopButtonClick(object sender, RoutedEventArgs e)
    {
        // TODO: Add logic for deleting a backup
    }
    
    private async void ShowErrorMessage(TextBlock errorTextBlock, string message)
    {
        errorTextBlock.Text = message;
        errorTextBlock.IsVisible = true;

        // Attendez 3 secondes
        await Task.Delay(3000);
        errorTextBlock.IsVisible = false;
    }
    private async void ShowSuccessMessage(string message)
    {
        SuccessMessage.Text = message;
        SuccessMessage.IsVisible = true;

        // Attendez 3 secondes
        await Task.Delay(3000);
        SuccessMessage.IsVisible = false;
    }

    private void ResetFields()
    {
        NameTextBox.Text = string.Empty;
        SourceDirectoryTextBox.Text = string.Empty;
        TargetDirectoryTextBox.Text = string.Empty;
        TypeChoice.SelectedIndex = -1;
    }
}
