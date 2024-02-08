using System;
using System.Globalization;
using System.Resources;
using System.Configuration;
using System.Collections;
using System.Collections.Specialized;


namespace EasySaveConsole
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ConfigModel configModel = new ConfigModel();

            // Création du ResourceManager pour les messages
            var resourceManager = new ResourceManager("easySave_console.Resources.Messages", typeof(Program).Assembly);

            // Initialisation de Utilities avec le ResourceManager
            Utilities messageDisplay = new Utilities(resourceManager);

            // Création de l'instance ViewModel avec ConfigModel et Utilities
            ViewModel viewModel = new ViewModel(configModel, messageDisplay);

            // Création de l'instance View avec ConfigModel, ViewModel et Utilities
            View view = new View(configModel, viewModel, messageDisplay);

            // Affichage du menu
            view.DisplayMenu();
        }
    }
}