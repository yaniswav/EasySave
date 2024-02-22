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
        }

        private void BackupButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            MainContent.Content = new BackupView();
        }

        private void SettingsButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            MainContent.Content = new SettingsView();
        }
    }
}