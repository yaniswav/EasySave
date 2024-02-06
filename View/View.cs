using System;
using EasySaveConsole; // Assurez-vous que cet espace de noms correspond à celui de votre projet.

public class View
{
    private ConfigModel _configModel;

    public View(ConfigModel configModel)
    {
        _configModel = configModel;
    }

    public void DisplayMenu()
    {
        bool exit = false;
        while (!exit)
        {
            Console.WriteLine("Choisissez une option :");
            Console.WriteLine("1. Lister les sauvegardes");
            Console.WriteLine("2. Créer une sauvegarde");
            Console.WriteLine("3. Modifier une sauvegarde (non implémenté dans ConfigModel)");
            Console.WriteLine("4. Supprimer une sauvegarde (non implémenté dans ConfigModel)");
            Console.WriteLine("5. Quitter");

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
