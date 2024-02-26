using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Resources;
using System.Reflection;
using EasySave.ViewModels;

namespace EasySave.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            UpdateButtonColor();
        }

        private void BackupButton_Click(object sender, RoutedEventArgs e)
        {
            // Logique pour charger BackupView
            MainContent.Content = new BackupView();
            UpdateButtonColor(activeButton: "Backup");
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            // Logique pour charger SettingsView
            MainContent.Content = new SettingsView();
            UpdateButtonColor(activeButton: "Settings");
        }

        private void UpdateButtonColor(string activeButton = "")
        {
            var activeColor = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#857DFF"));
            var inactiveColor = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#B4BAFF"));

            BackupButton.Background = activeButton == "Backup" ? activeColor : inactiveColor;
            SettingsButton.Background = activeButton == "Settings" ? activeColor : inactiveColor;
        }
        
    }
}