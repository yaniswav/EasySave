using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using System.ComponentModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;


namespace EasySave.ViewModels
{
    public class CommunicationVM : ViewModelBase
    {
        private string _notificationMessage;

        public CommunicationVM()
        {
            // Initialize commands
            ShowNotificationCommand = new RelayCommand(ShowNotification);
        }

        // Property to bind with the UI for displaying messages
        public string NotificationMessage
        {
            get => _notificationMessage;
            set => SetProperty(ref _notificationMessage, value);
        }

        // Command for showing notifications
        public ICommand ShowNotificationCommand { get; }

        // Method to invoke displaying a notification
        private void ShowNotification(object message)
        {
            if (message is string msg)
            {
                NotificationMessage = msg;
            }
        }
    }
}