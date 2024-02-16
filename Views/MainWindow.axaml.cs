using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Resources;
using System.Reflection;

namespace EasySave.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // Cache initialement les panels pour créer/modifier et supprimer/exécuter
            ShowCreateEditFields(false);
            ShowDeleteExecuteFields(false);
        }

        private void OnCreateBackupClick(object sender, RoutedEventArgs e)
        {
            SetActionTitle("Créer une sauvegarde");
            ShowCreateEditFields(true);
            ShowDeleteExecuteFields(false);
            // Cache le message de confirmation pour les autres actions
            HideConfirmationMessages();
            ActionCreateEditButton.Content = "Créer";
        }

        private void OnEditBackupClick(object sender, RoutedEventArgs e)
        {
            SetActionTitle("Modifier une sauvegarde");
            ShowCreateEditFields(true);
            ShowDeleteExecuteFields(false);
            // Cache le message de confirmation pour les autres actions
            HideConfirmationMessages();
            ActionCreateEditButton.Content = "Modifier";
        }

        private void OnDeleteBackupClick(object sender, RoutedEventArgs e)
        {
            SetActionTitle("Supprimer une sauvegarde");
            ShowCreateEditFields(false);
            ShowDeleteExecuteFields(true);
            // Cache le message de confirmation pour les autres actions
            HideConfirmationMessages();
            ActionDeleteExecuteButton.Content = "Supprimer";
        }

        private void OnExecuteBackupClick(object sender, RoutedEventArgs e)
        {
            SetActionTitle("Executer une sauvegarde");
            ShowCreateEditFields(false);
            ShowDeleteExecuteFields(true);
            // Cache le message de confirmation pour les autres actions
            HideConfirmationMessages();
            ActionDeleteExecuteButton.Content = "Executer";
        }
        
        private void HideConfirmationMessages()
        {
            CreateEditConfirmationTextBlock.IsVisible = false;
            DeleteExecuteConfirmationTextBlock.IsVisible = false;
        }

        private void SetActionTitle(string title)
        {
            ActionTitleTextBlock.Text = title;
            ActionTitlePanel.IsVisible = true;
        }

        private void ShowCreateEditFields(bool isVisible)
        {
            CreateEditInputFieldsPanel.IsVisible = isVisible;
            if (!isVisible)
            {
                // Clear fields if we are hiding them
                BackupNameTextBox.Text = "";
                SourceDirectoryTextBox.Text = "";
                TargetDirectoryTextBox.Text = "";
                BackupTypeComboBox.SelectedIndex = -1;
            }
        }

        private void ShowDeleteExecuteFields(bool isVisible)
        {
            DeleteExecuteInputFieldsPanel.IsVisible = isVisible;
            if (!isVisible)
            {
                // Clear the field if we are hiding it
                SingleBackupNameTextBox.Text = "";
            }
        }

        private void OnQuitClick(object sender, RoutedEventArgs e)
        {
            this.Close(); // Ferme l'application
        }

        private void OnListBackupsClick(object sender, RoutedEventArgs e)
        {
            SetActionTitle("Lister les sauvegardes");
            ShowCreateEditFields(false);
            ShowDeleteExecuteFields(false);
        }
        
        private void OnActionCreateEditButtonClick(object sender, RoutedEventArgs e)
        {
            // Affiche le message "OK" sous le bouton Créer/Modifier
            CreateEditConfirmationTextBlock.IsVisible = true;
            // Cache potentiellement les autres messages de confirmation
            DeleteExecuteConfirmationTextBlock.IsVisible = false;
        }

        private void OnActionDeleteExecuteButtonClick(object sender, RoutedEventArgs e)
        {
            // Affiche le message "OK" sous le bouton Supprimer/Executer
            DeleteExecuteConfirmationTextBlock.IsVisible = true;
            // Cache potentiellement les autres messages de confirmation
            CreateEditConfirmationTextBlock.IsVisible = false;
        }
        
        private async void OnSelectSourceDirectoryClick(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFolderDialog();
            var result = await dialog.ShowAsync(this);
            if (!string.IsNullOrEmpty(result))
            {
                SourceDirectoryTextBox.Text = result;
            }
        }

        private async void OnSelectTargetDirectoryClick(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFolderDialog();
            var result = await dialog.ShowAsync(this);
            if (!string.IsNullOrEmpty(result))
            {
                TargetDirectoryTextBox.Text = result;
            }
        }


    }
}