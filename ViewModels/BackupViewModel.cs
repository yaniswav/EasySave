using System.Collections.ObjectModel;
using ReactiveUI;

namespace EasySave.ViewModels
{
    public class BackupViewModel : ReactiveObject
    {
        public ObservableCollection<BackupJob> Jobs { get; }

        public BackupViewModel()
        {
            Jobs = new ObservableCollection<BackupJob>
            {
                new BackupJob { Name = "Sauvegarde 1", Source = "/path/source1", Destination = "/path/destination1", Type = "Complète", State = "Prêt", Progress = "0%" },
                new BackupJob { Name = "Sauvegarde 2", Source = "/path/source2", Destination = "/path/destination2", Type = "Différentielle", State = "En cours", Progress = "50%" }
                // Ajoutez ici d'autres sauvegardes fictives selon votre besoin
            };
        }
    }

    public class BackupJob : ReactiveObject
    {
        private string name;
        private string source;
        private string destination;
        private string type;
        private string state;
        private string progress;

        public string Name { get => name; set => this.RaiseAndSetIfChanged(ref name, value); }
        public string Source { get => source; set => this.RaiseAndSetIfChanged(ref source, value); }
        public string Destination { get => destination; set => this.RaiseAndSetIfChanged(ref destination, value); }
        public string Type { get => type; set => this.RaiseAndSetIfChanged(ref type, value); }
        public string State { get => state; set => this.RaiseAndSetIfChanged(ref state, value); }
        public string Progress { get => progress; set => this.RaiseAndSetIfChanged(ref progress, value); }
    }
}