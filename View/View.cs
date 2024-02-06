using System;
using EasySaveConsole; // Assurez-vous d'ajuster cet espace de noms en fonction de votre projet

public class View
{
    private ConfigModel.BackupManager _backupManager;

    public View(ConfigModel.BackupManager backupManager)
    {
        _backupManager = backupManager;
    }

    public void DisplayMenu()
    {
        bool exit = false;
        while (!exit)
        {
            Console.WriteLine("Choisissez une option :");
            Console.WriteLine("1. Lister les sauvegardes");
            Console.WriteLine("2. Créer une sauvegarde");
            Console.WriteLine("3. Modifier une sauvegarde");
            Console.WriteLine("4. Supprimer une sauvegarde");
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
                    EditBackup();
                    break;
                case "4":
                    DeleteBackup();
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
        Console.WriteLine("Liste des sauvegardes :");
        // Ici, utiliser _backupManager pour lister les sauvegardes
    }

    private void CreateBackup()
    {
        Console.WriteLine("Création d'une nouvelle sauvegarde. Veuillez fournir les détails.");
        // Demander à l'utilisateur les détails nécessaires et utiliser _backupManager pour créer une sauvegarde
    }

    private void EditBackup()
    {
        Console.WriteLine("Modification d'une sauvegarde. Veuillez fournir les détails.");
        // Demander à l'utilisateur les détails nécessaires et utiliser _backupManager pour modifier une sauvegarde
    }

    private void DeleteBackup()
    {
        Console.WriteLine("Suppression d'une sauvegarde. Veuillez fournir le nom de la sauvegarde.");
        // Demander à l'utilisateur le nom de la sauvegarde à supprimer et utiliser _backupManager pour la supprimer
    }
}
