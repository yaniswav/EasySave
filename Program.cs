using System;

namespace EasySaveConsole
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ConfigModel configModel = new ConfigModel();

            View view = new View(configModel);

            view.DisplayMenu();
        }
    }
}