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

    public void DisplayMessage(string resourceKey)
    {
        Console.WriteLine(_resourceManager.GetString(resourceKey, CultureInfo.CurrentUICulture));
    }

    public void DisplayMenu()
    {
        bool exit = false;
        DisplayMessage("WelcomeMessage");
        Console.WriteLine();
        while (!exit)
        {
            DisplayMessage("ChooseOption");
            Console.WriteLine();
            DisplayMessage("BackupList");
            DisplayMessage("CreateBackup");
            DisplayMessage("EditBackup");
            DisplayMessage("DeleteBackup");
            DisplayMessage("ExecuteBackup");
            DisplayMessage("ChangeLanguage");
            DisplayMessage("ExitMessage");
            Console.WriteLine();
            
            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    ListBackups();
                    Console.WriteLine();
                    break;
                case "2":
                    CreateBackup();
                    Console.WriteLine();
                    break;
                case "3":
                    EditBackup();
                    Console.WriteLine();
                    break;
                case "4":
                    DeleteBackup();
                    Console.WriteLine();
                    break;
                case "5":
                    ViewModel.ExecuteBackups();
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
                    DisplayMessage("IncorrectMessage");
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
            DisplayMessage("NoBackup");
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
        DisplayMessage("CreateNewBackup"); 
        DisplayMessage("BackupName"); 
        string name = Console.ReadLine();
        if (_configModel.BackupJobExists(name))
        {
            DisplayMessage("JobAlredyExists"); 
            return;
        }

        DisplayMessage("SourceDirectory"); 
        string sourceDir = Console.ReadLine();
        if (!IsValidPath(sourceDir))
        {
            DisplayMessage("InvalidPath"); 
            return;
        }

        DisplayMessage("TargetDirectory"); 
        string destinationDir = Console.ReadLine();
        if (!IsValidPath(destinationDir))
        {
            DisplayMessage("InvalidPath"); 
            return;
        }

        Console.Write("Type (Complete/Differential) : ");
        string type = Console.ReadLine();
        if (!IsValidBackupType(type))
        {
            DisplayMessage("InvalidType"); 
            return;
        }

        BackupJobConfig newJob = new BackupJobConfig
            { Name = name, SourceDir = sourceDir, DestinationDir = destinationDir, Type = type };
        _configModel.AddBackupJob(newJob);
        DisplayMessage("BackupSuccess"); 
    }

    private void EditBackup()
    {
        DisplayMessage("EditBackupName"); 
        string jobName = Console.ReadLine();

        // Obtenez les nouvelles informations de sauvegarde
        DisplayMessage("SourceDirectory"); 
        string newSourceDir = Console.ReadLine();


        if (!IsValidPath(newSourceDir))
        {
            DisplayMessage("InvalidPath"); 
            return;
        }


        DisplayMessage("TargetDirectory"); 

        string newDestinationDir = Console.ReadLine();

        if (!IsValidPath(newDestinationDir))
        {
            DisplayMessage("InvalidPath"); 
            return;
        }

        Console.Write("Type (Complete/Differential) : ");
        string newType = Console.ReadLine();


        if (!IsValidBackupType(newType))
        {
            DisplayMessage("InvalidType"); 
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
        DisplayMessage("EditSuccess"); 
    }

    private void DeleteBackup()
    {
        DisplayMessage("DeleteBackupName"); 
        string jobName = Console.ReadLine();

        _configModel.DeleteBackupJob(jobName);
        DisplayMessage("DeleteSuccess"); 
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