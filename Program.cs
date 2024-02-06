using System;

namespace EasySaveConsole
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Supposons que BackupManager est une classe ou un composant dans ConfigModel qui gère les sauvegardes.
            // Vous devez remplacer cette partie par la création réelle de votre gestionnaire de sauvegarde.
            ConfigModel.BackupManager backupManager = new ConfigModel.BackupManager();

            // Création de la vue avec le gestionnaire de sauvegarde.
            View view = new View(backupManager);

            // Démarrage de l'interface utilisateur.
            view.DisplayMenu();
        }
    }
}