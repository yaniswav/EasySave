using System;
using EasySaveConsole; // Assurez-vous que cet espace de noms correspond à celui de votre projet.
using System.Globalization;
using System.Resources;

public class View
{
    private ConfigModel _configModel;
    private ResourceManager _resourceManager;

    public View(ConfigModel configModel)
    {
        _configModel = configModel;
        SelectLanguage();
    }

    private void SelectLanguage()
    {
        Console.WriteLine("Choose your language (en/fr):");
        string language = Console.ReadLine();
        CultureInfo culture = language == "fr" ? new CultureInfo("fr-FR") : new CultureInfo("en-US");
        CultureInfo.CurrentUICulture = culture;
        _resourceManager = new ResourceManager("easySave_console.Resources.Messages", typeof(Program).Assembly);

    }
    public void DisplayMenu()
    {
        bool exit = false;
        while (!exit)
        {
            Console.WriteLine(_resourceManager.GetString("WelcomeMessage"));
            Console.WriteLine(_resourceManager.GetString("ChooseOption"));
            Console.WriteLine(_resourceManager.GetString("BackupList"));
            Console.WriteLine(_resourceManager.GetString("CreateBackup"));
            Console.WriteLine(_resourceManager.GetString("EditBackup"));
            Console.WriteLine(_resourceManager.GetString("DeleteBackup"));
            Console.WriteLine(_resourceManager.GetString("ExitMessage"));

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
                    Console.WriteLine("La modification n'est pas supportée dans la version actuelle.");
                    break;
                case "4":
                    Console.WriteLine("La suppression n'est pas supportée dans la version actuelle.");
                    break;
                case "5":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Option invalide, veuillez réessayer.");
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
                Console.WriteLine($"Nom: {job.Name}, Source: {job.SourceDir}, Destination: {job.DestinationDir}, Type: {job.Type}");
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

        BackupJobConfig newJob = new BackupJobConfig { Name = name, SourceDir = sourceDir, DestinationDir = destinationDir, Type = type };
        _configModel.AddBackupJob(newJob);
        Console.WriteLine("Sauvegarde ajoutée avec succès.");
    }
}
