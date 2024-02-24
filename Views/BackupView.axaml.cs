using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Resources;
using System.Reflection;
using EasySave.ViewModels;

namespace EasySave.Views;

public partial class BackupView : UserControl
{
    public BackupView()
    {
        InitializeComponent();
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
}