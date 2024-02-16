using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ReactiveUI;
using System.ComponentModel.DataAnnotations;

namespace EasySave.ViewModels
{
    // ViewModelBase serves as a base class for all view models, providing support for property change notification.
    public class ViewModelBase : ReactiveObject, INotifyPropertyChanged, INotifyDataErrorInfo
    {
        private Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

        public event PropertyChangedEventHandler PropertyChanged;

        // INotifyDataErrorInfo members
        public bool HasErrors => _errors.Any();

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        // Method to notify listeners that a property value has changed.
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            ValidateProperty(propertyName);
        }

        // Method to validate a property
        protected void ValidateProperty(string propertyName)
        {
            var propertyValue = GetType().GetProperty(propertyName)?.GetValue(this, null);
            var validationContext = new ValidationContext(this) { MemberName = propertyName };
            var validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(propertyValue, validationContext, validationResults);

            if (validationResults.Any())
            {
                _errors[propertyName] = validationResults.Select(r => r.ErrorMessage).ToList();
            }
            else
            {
                _errors.Remove(propertyName);
            }

            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        // Method to get errors by property name
        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName) || !_errors.ContainsKey(propertyName))
            {
                return null;
            }

            return _errors[propertyName];
        }
    }
}
