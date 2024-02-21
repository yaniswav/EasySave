using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EasySave.ViewModels
{
    public class RealTimeStatusViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<BackupJobStatus> _backupJobsStatus;

        public ObservableCollection<BackupJobStatus> BackupJobsStatus
        {
            get => _backupJobsStatus;
            set
            {
                _backupJobsStatus = value;
                OnPropertyChanged();
            }
        }

        public RealTimeStatusViewModel()
        {
            BackupJobsStatus = new ObservableCollection<BackupJobStatus>();
            // Ici, vous pourriez initialiser BackupJobsStatus avec des données réelles, possiblement en souscrivant à un service ou un manager qui met à jour les statuts des sauvegardes
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class BackupJobStatus : INotifyPropertyChanged
    {
        private string _name;
        private string _state;
        private double _progression;
        // Ajoutez d'autres propriétés nécessaires pour afficher l'état en temps réel

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public string State
        {
            get => _state;
            s