using System;
using EasySaveConsole;
using System.Globalization;
using System.Resources;

public class View
{
    private ConfigModel _configModel;
    private ResourceManager _resourceManager;

    public View(ConfigModel configModel)
    {
        _configModel = configModel;
        _configModel.LoadCurrentLanguage();
        _resourceManager = new ResourceManager("easySave_console.Resources.Messages", typeof(Program).Assembly);
        SelectLanguage();
    }

    private void SelectLanguage()
    {
        string currentLanguage = _configModel.Language;
        CultureInfo culture = currentLanguage == "fr" ? new CultureInfo("fr-FR") : new CultureInfo("en-US");
        CultureInfo.CurrentUICulture = culture;
        DisplayMessage("SelectLanguage");
    }

    private void DisplayMessage(string resourceKey)
    {
        Console.WriteLine(_resourceManager.GetString(resourceKey, CultureInfo.CurrentUICulture));
    }

    public void DisplayMenu()
    {
        bool exit = false;
        while (!exit)
        {
            DisplayMessage("WelcomeMessage");
            DisplayMessage("ChooseOption");
            DisplayMessage("BackupList");
            DisplayMessage("CreateBackup");
            DisplayMessage("EditBackup");
            DisplayMessage("DeleteBackup");
            DisplayMessage("ExitMessage");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    ListBackups();
                    break;
                case "2":
                    CreateBackup();
                    break;
                case "3":
                    DisplayMessage("EditNotSupported");
                    break;
                case "4":
                    DisplayMessage("DeleteNotSupported");
                    break;
                case "5":
                    exit = true;
                    break;
                default:
                    DisplayMessage("InvalidOption");
                    break;
            }
        }
    }

    private void ListBackups()
    {
        var backupJobs = _configModel.LoadBackupJobs();
        if (backupJobs.Count == 0)
        {
            Console.WriteLine("Aucune sauvegarde configurée.");
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

    private void CreateBackup()
    {
        Console.WriteLine("Création d'une nouvelle sauvegarde.");
        Console.Write("Nom de la sauvegarde : ");
        string name = Console.ReadLine();
        Console.Write("Répertoire source : ");
        string sourceDir = Console.ReadLine();
        Console.Write("Répertoire destination : ");
        string destinationDir = Console.ReadLine();
        Console.Write("Type (Complet/Différentiel) : ");
        string type = Console.ReadLine();

        BackupJobConfig newJob = new BackupJobConfig
            { Name = name, SourceDir = sourceDir, DestinationDir = destinationDir, Type = type };
        _configModel.AddBackupJob(newJob);
        Console.WriteLine("Sauvegarde ajoutée avec succès.");
    }
}