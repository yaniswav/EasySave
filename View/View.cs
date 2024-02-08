using System;
using EasySaveConsole;
using System.Globalization;
using System.Resources;
using System.Configuration;
using System.Collections;
using System.Collections.Specialized;

public class View
{
    private ConfigModel _configModel;
    private ViewModel _viewModel;
    private Utilities _messageDisplay;
    private ResourceManager _resourceManager;

    public View(ConfigModel configModel, ViewModel viewModel, Utilities messageDisplay)
    {
        _configModel = configModel;
        _viewModel = viewModel;
        _configModel.LoadCurrentLocale();
        var resourceManager = new ResourceManager("easySave_console.Resources.Messages", typeof(Program).Assembly);
        _messageDisplay = new Utilities(resourceManager);
    }


    public void DisplayMenu()
    {
        bool exit = false;
        _messageDisplay.DisplayMessage("WelcomeMessage");
        Console.WriteLine();
        while (!exit)
        {
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

            string choice = Console.ReadLine();
            switch (choice)
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

    private void ListBackups()
    {
        var backupJobs = _configModel.LoadBackupJobs();
        if (backupJobs.Count == 0)
        {
            _messageDisplay.DisplayMessage("NoBackup");
        }
        else
        {
            foreach (var job in backupJobs)
            {
                Console.WriteLine(
                    $"Nom: {job.Name}, Source: {job.SourceDir}, Destination: {job.DestinationDir}, Type: {job.Type}");
            }
        }
    }


    public void CreateBackupInterface()
    {
        _messageDisplay.DisplayMessage("CreateNewBackup");
        _messageDisplay.DisplayMessage("BackupName");
        string name = Console.ReadLine();

        _messageDisplay.DisplayMessage("SourceDirectory");
        string sourceDir = Console.ReadLine();

        _messageDisplay.DisplayMessage("TargetDirectory");
        string destinationDir = Console.ReadLine();

        Console.Write("Type (Complete/Differential) : ");
        string type = Console.ReadLine();

        if (_viewModel.TryCreateBackup(name, sourceDir, destinationDir, type, out string errorMessage))
        {
            _messageDisplay.DisplayMessage("BackupSuccess");
        }
        else
        {
            _messageDisplay.DisplayMessage(errorMessage);
        }
    }

    public void EditBackupInterface()
    {
        _messageDisplay.DisplayMessage("EditBackupName");
        string jobName = Console.ReadLine();

        _messageDisplay.DisplayMessage("SourceDirectory");
        string newSourceDir = Console.ReadLine();

        _messageDisplay.DisplayMessage("TargetDirectory");
        string newDestinationDir = Console.ReadLine();

        Console.Write("Type (Complete/Differential) : ");
        string newType = Console.ReadLine();

        if (_viewModel.TryEditBackup(jobName, newSourceDir, newDestinationDir, newType, out string errorMessage))
        {
            _messageDisplay.DisplayMessage("EditSuccess");
        }
        else
        {
            _messageDisplay.DisplayMessage(errorMessage);
        }
    }

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