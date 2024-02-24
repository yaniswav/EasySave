using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Resources;
using System.Reflection;
using EasySave.ViewModels;
using System.Collections.Generic;


namespace EasySave.Views;

public partial class BackupView : UserControl
{
    public BackupView()
    {
        InitializeComponent();

        //Ne fonctionne pas 
        var sampleData = new List<BackupJob>
        {
            new BackupJob
            {
                Name = "Job 1", SourceDirectory = "C:\\Source1", DestinationDirectory = "D:\\Destination1",
                Type = "Complète", State = "Ready", Progress = "0%"
            },
            new BackupJob
            {
                Name = "Job 2", SourceDirectory = "C:\\Source2", DestinationDirectory = "D:\\Destination2",
                Type = "Différentielle", State = "In Progress", Progress = "50%"
            }
            // Add more sample data as needed
        };
        SampleDataGrid.ItemsSource = sampleData;
    }

    //Ne fonctionne pas 
    public class BackupJob
    {
        public bool Selected { get; set; }
        public string Name { get; set; }
        public string SourceDirectory { get; set; }
        public string DestinationDirectory { get; set; }
        public string Type { get; set; }
        public string State { get; set; }
        public string Progress { get; set; }
    }

    private async void OnSelectSourceDirectoryClick(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFolderDialog();
        var result = await dialog.ShowAsync(new Window());
        if (!string.IsNullOrEmpty(result))
        {
            SourceDirectoryTextBox.Text = result;
        }
    }

    private async void OnSelectTargetDirectoryClick(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFolderDialog();
        var result = await dialog.ShowAsync(new Window());
        if (!string.IsNullOrEmpty(result))
        {
            TargetDirectoryTextBox.Text = result;
        }
    }
    
    private void OnCreateButtonClick(object sender, RoutedEventArgs e)
    {
        // TODO: Add logic for creating a backup
    }

    private void OnEditButtonClick(object sender, RoutedEventArgs e)
    {
        // TODO: Add logic for editing a backup
    }

    private void OnDeleteButtonClick(object sender, RoutedEventArgs e)
    {
        // TODO: Add logic for deleting a backup
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

}

