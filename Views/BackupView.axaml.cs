using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using EasySave.ViewModels;
using System.Threading.Tasks;
using Avalonia.VisualTree;
using System.Linq;

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
                ShowErrorMessage(NameErrorCreate, $"'{name}' '{NameErrorCreate.Text} ");
            }
            if (!viewModel.IsValidPath(sourceDir))
            {
                ShowErrorMessage(SourceError, SourceError.Text);
            }
            if (!viewModel.IsValidPath(destinationDir)) {
                ShowErrorMessage(DestinationError, DestinationError.Text);
            }
            if (!viewModel.IsValidBackupType(typeContent)) {
                ShowErrorMessage(TypeError, TypeError.Text);
            }
        }
        else
        {
            ShowCreateSuccessMessage(SuccessMessageCreate.Text);
            ResetFields();
            var mainWindow = (MainWindow)this.GetVisualRoot();
            if (mainWindow != null)
            {
                mainWindow.RefreshButtonContent();
                mainWindow.MainContent.Content = new BackupView(); 
                mainWindow.UpdateButtonColor(activeButton: "Backup");
            }
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
                ShowErrorMessage(NameError, $"{NameError.Text} {jobName}");
            }
            if (!viewModel.IsValidPath(newSourceDir))
            {
                ShowErrorMessage(SourceError, SourceError.Text);
            }
            if (!viewModel.IsValidPath(newDestinationDir)) {
                ShowErrorMessage(DestinationError, DestinationError.Text);
            }
            if (!viewModel.IsValidBackupType(newType)) {
                ShowErrorMessage(TypeError, TypeError.Text);
            }
        }
        else
        {
            ShowEditSuccessMessage(SuccessMessageEdit.Text);
            ResetFields();
            var mainWindow = (MainWindow)this.GetVisualRoot();
            if (mainWindow != null)
            {
                mainWindow.RefreshButtonContent();
                mainWindow.MainContent.Content = new BackupView(); 
                mainWindow.UpdateButtonColor(activeButton: "Backup");
            }
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
                ShowErrorMessage(NameError, $"{NameError.Text} {jobName}");
            }
        }
        else
        {
            ShowDeleteSuccessMessage(SuccessMessageDelete.Text);
            ResetFields();
            var mainWindow = (MainWindow)this.GetVisualRoot();
            if (mainWindow != null)
            {
                mainWindow.RefreshButtonContent();
                mainWindow.MainContent.Content = new BackupView(); 
                mainWindow.UpdateButtonColor(activeButton: "Backup");
            }
        }
    }

    
    // Dans BackupView.axaml.cs

    private void OnExecuteButtonClick(object sender, RoutedEventArgs e)
    {
        Console.WriteLine("OnExecuteButtonClick started"); // Pour vérifier que la méthode est appelée

        var selectedBackupJobs = SampleDataGrid.SelectedItems
            .Cast<BackupJobConfig>() // Assurez-vous que c'est le type correct pour vos éléments dans le DataGrid
            .Select(jobConfig => jobConfig.Name) // Extraction des noms des tâches de sauvegarde
            .ToList();

        Console.WriteLine($"Selected jobs count: {selectedBackupJobs.Count}"); // Pour afficher le nombre de tâches sélectionnées

        if (selectedBackupJobs.Any())
        {
            Console.WriteLine("There are selected jobs"); // Confirmer que des tâches sont sélectionnées
            var viewModel = this.DataContext as BackupVM; // Utilisation de BackupVM ici
            if (viewModel != null)
            {
                Console.WriteLine("ViewModel is not null, attempting to start jobs"); // Confirmer que le ViewModel est récupéré
                viewModel.StartJobs(selectedBackupJobs); // Appeler StartJobs sur BackupVM
                Console.WriteLine("StartJobs called on ViewModel"); // Confirmer l'appel à StartJobs
            }
            else
            {
                Console.WriteLine("ViewModel is null"); // Si le ViewModel n'est pas récupéré
            }
        }
        else
        {
            // Gérer le cas où aucune tâche n'est sélectionnée
            Console.WriteLine("Please select at least one backup job to execute."); // Si aucune tâche n'est sélectionnée
        }
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
    private async void ShowCreateSuccessMessage(string message)
    {
        SuccessMessageCreate.Text = message;
        SuccessMessageCreate.IsVisible = true;

        // Attendez 3 secondes
        await Task.Delay(3000);
        SuccessMessageCreate.IsVisible = false;
    }
    
    private async void ShowEditSuccessMessage(string message)
    {
        SuccessMessageEdit.Text = message;
        SuccessMessageEdit.IsVisible = true;

        // Attendez 3 secondes
        await Task.Delay(3000);
        SuccessMessageEdit.IsVisible = false;
    }
    
    private async void ShowDeleteSuccessMessage(string message)
    {
        SuccessMessageDelete.Text = message;
        SuccessMessageDelete.IsVisible = true;

        // Attendez 3 secondes
        await Task.Delay(3000);
        SuccessMessageDelete.IsVisible = false;
    }

    private void ResetFields()
    {
        NameTextBox.Text = string.Empty;
        SourceDirectoryTextBox.Text = string.Empty;
        TargetDirectoryTextBox.Text = string.Empty;
        TypeChoice.SelectedIndex = -1;
    }
    
    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var grid = sender as DataGrid;
        if (grid.SelectedItem is BackupJobConfig selectedBackup)
        {
            NameTextBox.Text = selectedBackup.Name;
            SourceDirectoryTextBox.Text = selectedBackup.SourceDir;
            TargetDirectoryTextBox.Text = selectedBackup.DestinationDir;
            TypeChoice.SelectedItem = TypeChoice.Items.Cast<ComboBoxItem>().FirstOrDefault(item => item.Content.ToString() == selectedBackup.Type);
        }
        
    }

}

