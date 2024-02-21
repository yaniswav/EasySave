using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;

namespace EasySave.ViewModels
{
    public class PriorityFilesViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<string> _priorityExtensions;
        private string _newExtension;
        public event PropertyChangedEventHandler PropertyChanged;

        public PriorityFilesViewModel()
        {
            _priorityExtensions = new ObservableCollection<string>();
        }

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
                NewExtension = string.Empty; // Reset after adding
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

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}