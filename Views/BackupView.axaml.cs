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

    private async void OnExecuteButtonClick(object sender, RoutedEventArgs e)
    {
        var selectedBackupJobs = SampleDataGrid.SelectedItems
            .Cast<BackupJobConfig>()
            .Select(jobConfig => jobConfig.Name)
            .ToList();

        if (selectedBackupJobs.Any())
        {
            var viewModel = this.DataContext as BackupVM;
            if (viewModel != null)
            {
                // Construction du message de succès avec les noms des tâches sélectionnées
                string successMessage = $"{String.Join(", ", selectedBackupJobs)} en cours d'exécution...";
                ExecutionSuccessMessage.Text = successMessage;
                ExecutionSuccessMessage.IsVisible = true; // Afficher le message

                viewModel.StartJobs(selectedBackupJobs);

                // Attendre 3 secondes avant de masquer le message
                await Task.Delay(3000);
                ExecutionSuccessMessage.IsVisible = false; // Masquer le message après 3 secondes
            }
        }
        else
        {
            // Construction du message de succès avec les noms des tâches sélectionnées
            string errorMessage = "Sélectionner au moins un travail";
            ExecutionErrorMessage.Text = errorMessage;
            ExecutionErrorMessage.IsVisible = true; // Afficher le message

            // Attendre 3 secondes avant de masquer le message
            await Task.Delay(3000);
            ExecutionErrorMessage.IsVisible = false; // Masquer le message après 3 secondes
        }
    }
    
    private async void OnPauseButtonClick(object sender, RoutedEventArgs e)
    {
        var selectedBackupJobs = SampleDataGrid.SelectedItems
            .Cast<BackupJobConfig>()
            .Select(jobConfig => jobConfig.Name)
            .ToList();

        if (selectedBackupJobs.Any())
        {
            var viewModel = this.DataContext as BackupVM; // Assurez-vous d'utiliser le bon ViewModel
            if (viewModel != null)
            {
                // Construction du message de pause
                string pauseMessage = $"{String.Join(", ", selectedBackupJobs)} en pause...";
                PauseSuccessMessage.Text = pauseMessage;
                PauseSuccessMessage.IsVisible = true; // Afficher le message

                viewModel.PauseJobs(selectedBackupJobs); // Appeler la méthode PauseJobs

                // Attendre 3 secondes avant de masquer le message
                await Task.Delay(3000);
                PauseSuccessMessage.IsVisible = false; // Masquer le message après 3 secondes
            }
        }
        else
        {
            // Optionnel : Gérer le cas où aucune tâche n'est sélectionnée
            Console.WriteLine("Please select at least one backup job to pause.");
        }
    }

    
    private async void OnPlayButtonClick(object sender, RoutedEventArgs e)
    {
        var selectedBackupJobs = SampleDataGrid.SelectedItems
            .Cast<BackupJobConfig>()
            .Select(jobConfig => jobConfig.Name)
            .ToList();

        if (selectedBackupJobs.Any())
        {
            var viewModel = this.DataContext as BackupVM; // Assurez-vous d'utiliser le bon ViewModel
            if (viewModel != null)
            {
                // Construction du message de reprise
                string resumeMessage = $"{String.Join(", ", selectedBackupJobs)} en reprise...";
                ResumeSuccessMessage.Text = resumeMessage;
                ResumeSuccessMessage.IsVisible = true; // Afficher le message

                viewModel.ResumeJobs(selectedBackupJobs); // Appeler la méthode ResumeJobs

                // Attendre 3 secondes avant de masquer le message
                await Task.Delay(3000);
                ResumeSuccessMessage.IsVisible = false; // Masquer le message après 3 secondes
            }
        }
        else
        {
            // Optionnel : Gérer le cas où aucune tâche n'est sélectionnée
            Console.WriteLine("Please select at least one backup job to resume.");
        }
    }

    
    private async void OnStopButtonClick(object sender, RoutedEventArgs e)
    {
        var selectedBackupJobs = SampleDataGrid.SelectedItems
            .Cast<BackupJobConfig>()
            .Select(jobConfig => jobConfig.Name)
            .ToList();

        if (selectedBackupJobs.Any())
        {
            var viewModel = this.DataContext as BackupVM; // Assurez-vous que c'est le bon ViewModel
            if (viewModel != null)
            {
                // Construction du message d'arrêt
                string stopMessage = $"{String.Join(", ", selectedBackupJobs)} arrêtée...";
                StopSuccessMessage.Text = stopMessage;
                StopSuccessMessage.IsVisible = true; // Afficher le message

                viewModel.StopJobs(selectedBackupJobs); // Appeler la méthode StopJobs

                // Attendre 3 secondes avant de masquer le message
                await Task.Delay(3000);
                StopSuccessMessage.IsVisible = false; // Masquer le message après 3 secondes
            }
        }
        else
        {
            // Optionnel : Gérer le cas où aucune tâche n'est sélectionnée
            Console.WriteLine("Please select at least one backup job to stop.");
        }
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

