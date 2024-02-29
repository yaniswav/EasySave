using Avalonia.Controls;
using Avalonia.Interactivity;


namespace EasySave.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainContent.Content = new BackupView();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            UpdateButtonColor(activeButton: "Backup");
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

        public void UpdateButtonColor(string activeButton = "")
        {
            var activeColor = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#857DFF"));
            var inactiveColor = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#B4BAFF"));

            BackupButton.Background = activeButton == "Backup" ? activeColor : inactiveColor;
            SettingsButton.Background = activeButton == "Settings" ? activeColor : inactiveColor;
        }
        
        public void RefreshButtonContent()
        {
            BackupButton.Content = Assets.Resources.Backup;
            SettingsButton.Content = Assets.Resources.Settings;
        }

        
    }
}