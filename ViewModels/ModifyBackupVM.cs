using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EasySave.ViewModels
{
    public class ModifyBackupVm : INotifyPropertyChanged
    {
        private ObservableCollection<string> _priorityExtensions = new ObservableCollection<string>();
        private string _newExtension = string.Empty; // Initialisé avec une chaîne vide pour éviter l'affectation de null

        // Implémentez l'interface avec un événement nullable
        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<string> PriorityExtensions
        {
            get => _priorityExtensions;
            set
            {
                _priorityExtensions = value;
                OnPropertyChanged();
            }
        }

        public string NewExtension
        {
            get => _newExtension;
            set
            {
                if (value != _newExtension)
                {
                    _newExtension = value;
                    OnPropertyChanged();
                }
            }
        }

        public void AddExtension()
        {
            if (!string.IsNullOrWhiteSpace(NewExtension) && !_priorityExtensions.Contains(NewExtension))
            {
                _priorityExtensions.Add(NewExtension);
                OnPropertyChanged(nameof(PriorityExtensions));
                NewExtension = string.Empty; // Utilisez une chaîne vide au lieu de null
            }
        }

        public void RemoveExtension(string extension)
        {
            if (_priorityExtensions.Contains(extension))
            {
                _priorityExtensions.Remove(extension);
                OnPropertyChanged(nameof(PriorityExtensions));
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) // Marquez propertyName comme nullable
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}