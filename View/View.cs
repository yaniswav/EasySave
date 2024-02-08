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
    private ResourceManager _resourceManager;

    public View(ConfigModel configModel)
    {
        _configModel = configModel;
        _configModel.LoadCurrentLocale();
        _resourceManager = new ResourceManager("easySave_console.Resources.Messages", typeof(Program).Assembly);
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


            Console.WriteLine("6. Change language / Changer la langue");
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
                    ModifyBackup();
                    break;
                case "4":
                    DeleteBackup();
                    break;
                case "5":
                    ChangeLocale();
                    break;
                case "6":
                    ViewModel.ExecuteBackups();
                    break;
                case "7":
                    exit = true;
                    break;
                default:
                    DisplayMessage("IncorrectMessage");
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
        CultureInfo.CurrentUICulture = newCulture; // Cette ligne est correcte
        _resourceManager = new ResourceManager("easySave_console.Resources.Messages", typeof(Program).Assembly);
        Console.WriteLine($"Language changed to / Langue changée en : {newCulture.DisplayName}");
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
        if (_configModel.BackupJobExists(name))
        {
            Console.WriteLine("Un job de sauvegarde avec ce nom existe déjà.");
            return;
        }

        Console.Write("Répertoire source : ");
        string sourceDir = Console.ReadLine();
        if (!IsValidPath(sourceDir))
        {
            Console.WriteLine("Le chemin source est invalide.");
            return;
        }

        Console.Write("Répertoire destination : ");
        string destinationDir = Console.ReadLine();
        if (!IsValidPath(destinationDir))
        {
            Console.WriteLine("Le chemin de destination est invalide.");
            return;
        }

        Console.Write("Type (Complet/Différentiel) : ");
        string type = Console.ReadLine();
        if (!IsValidBackupType(type))
        {
            Console.WriteLine("Le type de sauvegarde est invalide.");
            return;
        }

        BackupJobConfig newJob = new BackupJobConfig
            { Name = name, SourceDir = sourceDir, DestinationDir = destinationDir, Type = type };
        _configModel.AddBackupJob(newJob);
        Console.WriteLine("Sauvegarde ajoutée avec succès.");
    }

    private void ModifyBackup()
    {
        Console.WriteLine("Entrez le nom du job de sauvegarde à modifier : ");
        string jobName = Console.ReadLine();

        // Obtenez les nouvelles informations de sauvegarde
        Console.Write("Nouveau répertoire source : ");
        string newSourceDir = Console.ReadLine();


        if (!IsValidPath(newSourceDir))
        {
            Console.WriteLine("Le chemin source est invalide.");
            return;
        }


        Console.Write("Nouveau répertoire destination : ");

        string newDestinationDir = Console.ReadLine();

        if (!IsValidPath(newDestinationDir))
        {
            Console.WriteLine("Le chemin source est invalide.");
            return;
        }

        Console.Write("Nouveau type (Complete/Differential) : ");
        string newType = Console.ReadLine();


        if (!IsValidBackupType(newType))
        {
            Console.WriteLine("Le type de sauvegarde est invalide.");
            return;
        }


        // Créez une nouvelle configuration de job
        BackupJobConfig modifiedJob = new BackupJobConfig
        {
            Name = jobName,
            SourceDir = newSourceDir,
            DestinationDir = newDestinationDir,
            Type = newType
        };

        _configModel.ModifyBackupJob(jobName, modifiedJob);
        Console.WriteLine("Sauvegarde modifiée avec succès.");
    }

    private void DeleteBackup()
    {
        Console.WriteLine("Entrez le nom du job de sauvegarde à supprimer : ");
        string jobName = Console.ReadLine();

        _configModel.DeleteBackupJob(jobName);
        Console.WriteLine("Sauvegarde supprimée avec succès.");
    }

    private bool IsValidBackupType(string type)
    {
        var validTypes = new[] { "Complete", "Differential" };
        return validTypes.Contains(type);
    }

    private bool IsValidPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return false;
        }

        var validPathPrefixes = new List<string>
            { "C:\\", "D:\\", "E:\\", "F:\\", "G:\\", "H:\\" };

        return validPathPrefixes.Any(prefix => path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
    }
}