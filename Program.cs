using System;
using System.Globalization;
using System.Resources;
using System.Configuration;
using System.Collections;
using System.Collections.Specialized;
using System.Net;

namespace EasySave
{
    // Class for entry point of the EasySaveConsole application
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Initialize the configuration model to manage application settings
            var configModel = ConfigModel.Instance;

            // Create ResourceManager for message
            var resourceManager =
                new ResourceManager("EasySave.Resources.Languages.Messages", typeof(Program).Assembly);

            //Initialize Utilities with ResourceManager to display message to the user
            Utilities messageDisplay = new Utilities(resourceManager);

            //Create ViewModel instance to provide access to business logic and interaction with the model
            ViewModel viewModel = new ViewModel(configModel, messageDisplay);

            // Create View instance, responsible for user interface logic and user interactions
            View view = new View(viewModel, messageDisplay);
            
            Server server = new Server(8080);
            Task serverTask = Task.Run(() => server.Start());
            
            
            // Menu display    
            view.DisplayMenu();
            
            await serverTask;
        }
    }
}