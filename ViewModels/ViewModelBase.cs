using System.ComponentModel;
using ReactiveUI;

namespace EasySave.ViewModels
{
    // ViewModelBase serves as a base class for all view models, providing support for property change notification.
    public class ViewModelBase : ReactiveObject, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // Method to notify listeners that a property value has changed.
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}