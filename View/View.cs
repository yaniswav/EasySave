using System;

namespace EasySaveConsole

{
    public class View
    {

        public void StartInterface()
        {
            bool continueRunning = true;
            while (continueRunning)
            {
                DisplayMenu();
                string command = Console.ReadLine();
                ProcessCommand(command);
            }
        }

        private void DisplayMenu()
        {
            Console.WriteLine("EasySave - Backup Menu");
            Console.WriteLine("Enter your backup command (e.g., 1-3, 1 ;3) or 'exit' to quit:");
        }

        private void ProcessCommand(string command)
        {
            if (command.Equals("exit", StringComparison.OrdinalIgnoreCase))
            {
                Environment.Exit(0);
            }
            else
            {
                // viewModel.executeTask(command);
                // Display additional information if necessary
            }
        }

        public void DisplayResult(string message)
        {
            Console.WriteLine(message);
        }

        public void DisplayError(string error)
        {
            Console.WriteLine("Error : " + error);
        }
    }
}