using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;


namespace EasySave.ViewModels
{
    public class NavigationVM : ViewModelBase
    {
        private ViewModelBase _currentViewModel;

        public NavigationVM()
        {
            // Commands to switch views
            ShowCreateBackupViewCommand = new RelayCommand(o => CurrentViewModel = new CreateBackupVM());
            ShowModifyBackupViewCommand = new RelayCommand(o => CurrentViewModel = new ModifyBackupVM());
            // Additional commands for other views as needed
        }

        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set => SetProperty(ref _currentViewModel, value);
        }

        public ICommand ShowCreateBackupViewCommand { get; private set; }
        public ICommand ShowModifyBackupViewCommand { get; private set; }
        // Additional ICommand properties for other views as needed
    }
}