using System;
using System.Globalization;
using System.Resources;
using System.Configuration;
using System.Collections;
using System.Collections.Specialized;


namespace EasySave
{
    // Class for entry point of the EasySaveConsole application
    public class Program
    {
        public static void Main(string[] args)
        {
            // Initialize the configuration model to manage application settings
            ConfigModel configModel = new ConfigModel();

            // Create ResourceManager for message
            var resourceManager = new ResourceManager("EasySave.Resources.Languages.Messages", typeof(Program).Assembly);

            //Initialize Utilities with ResourceManager to display message to the user
            Utilities messageDisplay = new Utilities(resourceManager);

            //Create ViewModel instance to provide access to business logic and interaction with the model
            BackupVM backupVm = new BackupVM(configModel, messageDisplay);

            // Create View instance, responsible for user interface logic and user interactions
            View view = new View(configModel, backupVm, messageDisplay);

            // Menu display
            view.DisplayMenu();
        }
    }

    public class View
    {
        public View(ConfigModel configModel, BackupVM backupVm, Utilities messageDisplay)
        {
            // Example of setting class members
            this.ConfigModel = configModel;
            this.BackupVm = backupVm;
            this.MessageDisplay = messageDisplay;

            // Initialize your view components here
        }

        public Utilities MessageDisplay { get; set; }

        public BackupVM BackupVm { get; set; }

        public ConfigModel ConfigModel { get; set; }

        public void DisplayMenu()
        {
            throw new NotImplementedException();
        }
    }
}