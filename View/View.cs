using System;
using EasySaveConsole;
using System.Globalization;
using System.Resources;
using System.Configuration;
using System.Collections;
using System.Collections.Specialized;

// This class represents the user interface logic for the application
public class View
{
    // Dependencies for managing configurations, displaying messages, and accessing resource files
    private ConfigModel _configModel;
    private ViewModel _viewModel;
    private Utilities _messageDisplay;
    private ResourceManager _resourceManager;

    // Constructor initializes the view with necessary models and utilities
    public View(ConfigModel configModel, ViewModel viewModel, Utilities messageDisplay)
    {
        _configModel = configModel;
        _viewModel = viewModel;
        _configModel.LoadCurrentLocale(); // Load the current locale based on application settings
        // Initialize ResourceManager for accessing localized strings
        var resourceManager = new ResourceManager("easySave_console.Resources.Messages", typeof(Program).Assembly);
        _messageDisplay = new Utilities(resourceManager); // Utilities for displaying messages to the user
    }


    // Displays the main menu and handles user interactions
    public void DisplayMenu()
    {
        bool exit = false; // Flag to control the menu loop
        _messageDisplay.DisplayMessage("WelcomeMessage");
        Console.WriteLine();
        while (!exit)
        {
            // Display menu options
            _messageDisplay.DisplayMessage("ChooseOption");
            Console.WriteLine();
            _messageDisplay.DisplayMessage("BackupList");
            _messageDisplay.DisplayMessage("CreateBackup");
            _messageDisplay.DisplayMessage("EditBackup");
            _messageDisplay.DisplayMessage("DeleteBackup");
            _messageDisplay.DisplayMessage("ExecuteBackup");
            _messageDisplay.DisplayMessage("ChangeLanguage");
            _messageDisplay.DisplayMessage("ExitMessage");
            Console.WriteLine();

            string choice = Console.ReadLine(); // User input for menu selection
            switch (choice) // Handle menu selection
            {
                case "1":
                    ListBackups();
                    Console.WriteLine();
                    break;
                case "2":
                    CreateBackupInterface();
                    Console.WriteLine();
                    break;
                case "3":
                    EditBackupInterface();
                    Console.WriteLine();
                    break;
                case "4":
                    DeleteBackupInterface();
                    Console.WriteLine();
                    break;
                case "5":
                    _viewModel.ExecuteBackups();
                    Console.WriteLine();
                    break;
                case "6":
                    ChangeLocale();
                    Console.WriteLine();
                    break;
                case "7":
                    exit = true;
                    break;
                default:
                    _messageDisplay.DisplayMessage("IncorrectMessage");
                    Console.WriteLine();
                    break;
            }
        }
    }

    // Allows the user to change the application's language/locale
    private void ChangeLocale()
    {
        Console.WriteLine("Choose your new default language / Choisissez votre nouvelle langue par défaut (en/fr):");
        string newLocale = Console.ReadLine();
        CultureInfo newCulture = newLocale == "fr" ? new CultureInfo("fr-FR") : new CultureInfo("en-US");
        _configModel.SetLocale(newCulture.Name);
        CultureInfo.CurrentUICulture = newCulture;
        _resourceManager = new ResourceManager("easySave_console.Resources.Messages", typeof(Program).Assembly);
        Console.WriteLine($"Language changed to / Langue changée en : {newCulture.DisplayName}");
    }

    // Displays a list of configured backup jobs
    private void ListBackups()
    {
        var backupJobs = _configModel.LoadBackupJobs();
        if (backupJobs.Count == 0)
        {
            _messageDisplay.DisplayMessage("NoBackup");
        }
        else
        {
            // Display details of each configured backup job
            foreach (var job in backupJobs)
            {
                Console.WriteLine(
                    $"Nom: {job.Name}, Source: {job.SourceDir}, Destination: {job.DestinationDir}, Type: {job.Type}");
            }
        }
    }


    // Interface for creating a new backup job
    public void CreateBackupInterface()
    {
        // Collect input from user for new backup job details
        _messageDisplay.DisplayMessage("CreateNewBackup");
        _messageDisplay.DisplayMessage("BackupName");
        string name = Console.ReadLine();

        _messageDisplay.DisplayMessage("SourceDirectory");
        string sourceDir = Console.ReadLine();

        _messageDisplay.DisplayMessage("TargetDirectory");
        string destinationDir = Console.ReadLine();

        Console.Write("Type (Complete/Differential) : ");
        string type = Console.ReadLine();

        // Attempt to create backup job with provided details
        if (_viewModel.TryCreateBackup(name, sourceDir, destinationDir, type, out string errorMessage))
        {
            _messageDisplay.DisplayMessage("BackupSuccess");
        }
        else
        {
            _messageDisplay.DisplayMessage(errorMessage);
        }
    }

    // Interface for editing an existing backup job
    public void EditBackupInterface()
    {
        // Collect new details for the backup job to be edited
        _messageDisplay.DisplayMessage("EditBackupName");
        string jobName = Console.ReadLine();

        _messageDisplay.DisplayMessage("SourceDirectory");
        string newSourceDir = Console.ReadLine();

        _messageDisplay.DisplayMessage("TargetDirectory");
        string newDestinationDir = Console.ReadLine();

        Console.Write("Type (Complete/Differential) : ");
        string newType = Console.ReadLine();

        // Attempt to edit backup job with new details
        if (_viewModel.TryEditBackup(jobName, newSourceDir, newDestinationDir, newType, out string errorMessage))
        {
            _messageDisplay.DisplayMessage("EditSuccess");
        }
        else
        {
            _messageDisplay.DisplayMessage(errorMessage);
        }
    }

    // Interface for deleting an existing backup job
    public void DeleteBackupInterface()
    {
        _messageDisplay.DisplayMessage("DeleteBackupName");
        string jobName = Console.ReadLine();

        if (_viewModel.TryDeleteBackup(jobName))
        {
            _messageDisplay.DisplayMessage("DeleteSuccess");
        }
    }
}