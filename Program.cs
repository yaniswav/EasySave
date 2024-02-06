using System;

namespace EasySaveConsole
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Instanciation du ConfigModel qui contient la logique pour gérer les sauvegardes.
            ConfigModel configModel = new ConfigModel();

            // Création de la vue avec le modèle de configuration.
            View view = new View(configModel);

            // Démarrage de l'interface utilisateur.
            view.DisplayMenu();
        }
    }
}