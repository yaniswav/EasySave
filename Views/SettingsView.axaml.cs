using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using System.Globalization;


namespace EasySave.Views;

public partial class SettingsView : UserControl
{
    public SettingsView()
    {
        InitializeComponent();
    }
    private void OnResetButtonClick(object sender, RoutedEventArgs e)
    {
        // TODO: Add logic for resetting settings
    }
    
    private void OnApplyButtonClick(object sender, RoutedEventArgs e)
    {
        Assets.Resources.Culture = new CultureInfo("fr");
        
    }
}