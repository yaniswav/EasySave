using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using System.Globalization;
using Avalonia.VisualTree;
using System.Collections.Generic;
using System.Linq;



namespace EasySave.Views;

public partial class SettingsView : UserControl
{
    
    private static int _lastSelectedIndexLanguage = 1;
    private static int _lastSelectedIndexFormat = 0;
    private static string _lastFileSize = "10000"; 
    private static string _lastBusinessSoftware = "notepad";
    private static string _lastLogDirectory = "Default/Log/Path";
    private static string _lastStateDirectory = "Default/State/Path";
    
    private static Dictionary<string, bool> _checkBoxStates = new Dictionary<string, bool>
    {
        {"PrioritizedCheckExe", false},
        {"PrioritizedCheckDocx", false},
        {"PrioritizedCheckPdf", false},
        {"PrioritizedCheckTxt", false},
        {"EncryptedCheckExe", false},
        {"EncryptedCheckDocx", false},
        {"EncryptedCheckPdf", false},
        {"EncryptedCheckTxt", false},
    };
    
    public SettingsView()
    {
        InitializeComponent();
        ApplyCheckBoxStates();
        Language.SelectedIndex = _lastSelectedIndexLanguage;
        Format.SelectedIndex = _lastSelectedIndexFormat;
        FileSize.Text = _lastFileSize;
        BusinessSoftware.Text = _lastBusinessSoftware;
        LogDirectory.Text = _lastLogDirectory;
        StateDirectory.Text = _lastStateDirectory;
    }

    private void ApplyCheckBoxStates()
    {
        PrioritizedCheckExe.IsChecked = _checkBoxStates["PrioritizedCheckExe"];
        PrioritizedCheckDocx.IsChecked = _checkBoxStates["PrioritizedCheckDocx"];
        PrioritizedCheckPdf.IsChecked = _checkBoxStates["PrioritizedCheckPdf"];
        PrioritizedCheckTxt.IsChecked = _checkBoxStates["PrioritizedCheckTxt"];
        EncryptedCheckExe.IsChecked = _checkBoxStates["EncryptedCheckExe"];
        EncryptedCheckDocx.IsChecked = _checkBoxStates["EncryptedCheckDocx"];
        EncryptedCheckPdf.IsChecked = _checkBoxStates["EncryptedCheckPdf"];
        EncryptedCheckTxt.IsChecked = _checkBoxStates["EncryptedCheckTxt"];
    }
    
    private void SaveCheckBoxStates()
    {
        _checkBoxStates["PrioritizedCheckExe"] = PrioritizedCheckExe.IsChecked ?? false;
        _checkBoxStates["PrioritizedCheckDocx"] = PrioritizedCheckDocx.IsChecked ?? false;
        _checkBoxStates["PrioritizedCheckPdf"] = PrioritizedCheckPdf.IsChecked ?? false;
        _checkBoxStates["PrioritizedCheckTxt"] = PrioritizedCheckTxt.IsChecked ?? false;
        _checkBoxStates["EncryptedCheckExe"] = EncryptedCheckExe.IsChecked ?? false;
        _checkBoxStates["EncryptedCheckDocx"] = EncryptedCheckDocx.IsChecked ?? false;
        _checkBoxStates["EncryptedCheckPdf"] = EncryptedCheckPdf.IsChecked ?? false;
        _checkBoxStates["EncryptedCheckTxt"] = EncryptedCheckTxt.IsChecked ?? false;
    }
    
    private void ResetCheckBoxStates()
    {
        // Set all states to default (e.g., false)
        _checkBoxStates.Keys.ToList().ForEach(key => _checkBoxStates[key] = false);
        // Specific logic for resetting to true if needed
    }
    
    private void OnResetButtonClick(object sender, RoutedEventArgs e)
    {
        // Reset settings to default values
        _lastSelectedIndexLanguage = 1;
        _lastSelectedIndexFormat = 0;
        _lastFileSize = "10000";
        _lastBusinessSoftware = "notepad";
        _lastLogDirectory = "Default/Log/Path";
        _lastStateDirectory = "Default/State/Path";
        Assets.Resources.Culture = new CultureInfo("en");
        ResetCheckBoxStates();
        ApplyCheckBoxStates();
    
        // Apply the reset settings
        Language.SelectedIndex = _lastSelectedIndexLanguage;
        Format.SelectedIndex = _lastSelectedIndexFormat;
        FileSize.Text = _lastFileSize;
        BusinessSoftware.Text = _lastBusinessSoftware;
        LogDirectory.Text = _lastLogDirectory;
        StateDirectory.Text = _lastStateDirectory;

        // Refresh MainWindow content
        var mainWindow = (MainWindow)this.GetVisualRoot();
        if (mainWindow != null)
        {
            mainWindow.RefreshButtonContent();
            mainWindow.MainContent.Content = new SettingsView(); 
            mainWindow.UpdateButtonColor(activeButton: "Settings");
        }
    }
    
    private void OnApplyButtonClick(object sender, RoutedEventArgs e)
    {
        SaveCheckBoxStates();
        ApplyCheckBoxStates();
        var newIndexLanguage = Language.SelectedIndex;
        var newIndexFormat = Format.SelectedIndex;
        var newFileSize = FileSize.Text; // Get the current value from the TextBox
        Assets.Resources.Culture = new CultureInfo(newIndexLanguage == 0 ? "fr" : "en");
        _lastSelectedIndexLanguage = Language.SelectedIndex;
        _lastSelectedIndexFormat = Format.SelectedIndex;
        _lastFileSize = FileSize.Text;
        _lastBusinessSoftware = BusinessSoftware.Text;
        _lastLogDirectory = LogDirectory.Text;
        _lastStateDirectory = StateDirectory.Text;
        Language.SelectedIndex = _lastSelectedIndexLanguage; 
        Format.SelectedIndex = _lastSelectedIndexFormat;
        FileSize.Text = _lastFileSize;
        BusinessSoftware.Text = _lastBusinessSoftware;
        LogDirectory.Text = _lastLogDirectory;
        StateDirectory.Text = _lastStateDirectory;

        var mainWindow = (MainWindow)this.GetVisualRoot();
        if (mainWindow != null)
        {
            mainWindow.RefreshButtonContent();
            mainWindow.MainContent.Content = new SettingsView(); 
            mainWindow.UpdateButtonColor(activeButton: "Settings");
        }
    }


}