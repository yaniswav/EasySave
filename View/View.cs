using System;
using EasySave;
using System.Globalization;
using System.Resources;
using System.Configuration;
using System.Collections;
using System.Collections.Specialized;

// This class represents the user interface logic for the application
public class View
{
    // Dependencies for managing configurations, displaying messages, and accessing resource files
    private ViewModel _viewModel;
    private Utilities _messageDisplay;
    private ResourceManager _resourceManager;
    protected ConfigModel config = ConfigModel.Instance;

    // Constructor initializes the view with necessary models and utilities
    public View(ViewModel viewModel, Utilities messageDisplay)
    {
        _viewModel = viewModel;
        // Initialize ResourceManager for accessing localized strings
        var resourceManager = new ResourceManager("EasySave.Resources.Languages.Messages", typeof(Program).Assembly);
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
                    DisplayBackupProgress(_viewModel.BackupManager);
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

    private void ChangeLocale()
    {
        Console.WriteLine();
        Console.WriteLine("Choose your new default language / Choisissez votre nouvelle langue par défaut (en/fr):");
        string newLocale = Console.ReadLine();
        CultureInfo newCulture = newLocale == "fr" ? new CultureInfo("fr-FR") : new CultureInfo("en-US");

        // Mise à jour de la locale via la propriété CurrentLocale
        config.CurrentLocale = newCulture.Name;

        // Inutile de mettre à jour CultureInfo.CurrentUICulture ici car SetCulture est appelé dans CurrentLocale

        _resourceManager = new ResourceManager("EasySave.Resources.Messages", typeof(Program).Assembly);
        _messageDisplay.DisplayMessage("ChangedLanguage");
        Console.WriteLine($"{newCulture.DisplayName}");
    }


    // Displays a list of configured backup jobs
    private void ListBackups()
    {
        Console.WriteLine();
        var backupJobs = config.LoadBackupJobs();
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
                    $"Name: {job.Name}, Source: {job.SourceDir}, Destination: {job.DestinationDir}, Type: {job.Type}");
            }
        }
    }


    // Interface for creating a new backup job
    public void CreateBackupInterface()
    {
        Console.WriteLine();
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
            Console.WriteLine(errorMessage);
        }
    }

    // Interface for editing an existing backup job
    public void EditBackupInterface()
    {
        Console.WriteLine();
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
            _messageDisplay.DisplayMessage("errorMessage");
        }
    }

// Interface for deleting an existing backup job
    public void DeleteBackupInterface()
    {
        Console.WriteLine();
        _messageDisplay.DisplayMessage("DeleteBackupName");
        string jobName = Console.ReadLine();

        if (_viewModel.TryDeleteBackup(jobName))
        {
            _messageDisplay.DisplayMessage("DeleteSuccess");
        }
    }

    public void DisplayBackupProgress(BackupManager backupManager)
    {
        while (!backupManager.AllJobsCompleted)
        {
            var progress = backupManager.GetBackupProgress();
            foreach (var kvp in progress)
            {
                Console.WriteLine("Progression qui progresse");
                Console.WriteLine($"Progression de {kvp.Key}: {kvp.Value}%");
                Thread.Sleep(1000);
            }
        }
    }
}