using System.Collections.ObjectModel;
using System.ComponentModel;
using EasySave; // Make sure this namespace is correct
using System;
using System.Linq;
using System.Threading.Tasks;
﻿using CommunityToolkit.Mvvm.ComponentModel;

namespace EasySave.ViewModels
{
    public class BackupVM 
    {
        private ObservableCollection<BackupJob> _backupJobs;
        

        public BackupVM()
        {
            _backupJobs = new ObservableCollection<BackupJob>();
            // Initialize with existing backup jobs if necessary
        }

        public ObservableCollection<BackupJob> BackupJobs
        {
            get => _backupJobs;
            set
            {
                _backupJobs = value;
                
            }
        }

        public void AddBackupJob(string name, string sourceDir, string destinationDir, string type)
        {
            BackupJob backupJob;
            if (type == "Complete")
            {
                backupJob = new CompleteBackup(name, sourceDir, destinationDir);
            }
            else if (type == "Differential")
            {
                backupJob = new DifferentialBackup(name, sourceDir, destinationDir);
            }
            else
            {
                throw new ArgumentException("Unsupported backup job type.");
            }

            _backupJobs.Add(backupJob);
           
        }

        public void DeleteBackupJob(string name)
        {
            var jobToDelete = _backupJobs.FirstOrDefault(job => job.Name == name);
            if (jobToDelete != null)
            {
                _backupJobs.Remove(jobToDelete);
                
            }
        }

        // Additional methods to manage backup jobs (e.g., start, stop) could be added here


    }
}
