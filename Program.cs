using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;


namespace EasySaveConsole
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IConfigurationManager configurationManager = new ConfigurationManagerImplementation();
            // Instanciation du ConfigModel qui contient la logique pour gérer les sauvegardes.
            ConfigModel configModel = new ConfigModel(configurationManager);

            // Création de la vue avec le modèle de configuration.
            View view = new View(configModel);

            // Démarrage de l'interface utilisateur.
            view.DisplayMenu();
        }
    }
}