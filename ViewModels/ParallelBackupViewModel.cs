using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace EasySave.ViewModels
{
    public class ParallelBackupViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        private int _bandwidthLimit;
        private List<string> _priorityExtensions;

        public int BandwidthLimit
        {
            get => _bandwidthLimit;
            set
            {
                if (value != _bandwidthLimit)
                {
                    _bandwidthLimit = value;
                    OnPropertyChanged();
                }
            }
        }

        public List<string> PriorityExtensions
        {
            get => _priorityExtensions;
            set
            {
                if (value != _priorityExtensions)
                {
                    _priorityExtensions = value;
                    OnPropertyChanged();
                }
            }
        }

        public ParallelBackupViewModel()
        {
            _priorityExtensions = new List<string>();
            _bandwidthLimit = 1024; // Default value in Kilobytes
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Error => null;

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case nameof(BandwidthLimit):
                        if (BandwidthLimit <= 0)
                            return "Bandwidth limit must be greater than 0.";
                        break;
                    case nameof(PriorityExtensions):
                        if (PriorityExtensions.Any(ext => !Regex.IsMatch(ext, @"^\.[a-zA-Z0-9]+$")))
                            return "Invalid file extension format. Must start with a dot followed by alphanumeric characters.";
                        break;
                }
                return null;
            }
        }
    }
}
