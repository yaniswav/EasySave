using System;
using System.Collections.Generic;

namespace EasySave.ViewModels
{
    public class BackupJobControlVM
    {
        private EasySave.BackupManager _backupManager;

        public BackupJobControlVM(EasySave.BackupManager backupManager)
        {
            _backupManager = backupManager;
        }

        public void PauseJobs(List<string> jobNames)
        {
            foreach (var jobName in jobNames)
            {
                _backupManager.PauseJob(jobName);
            }
        }

        public void ResumeJobs(List<string> jobNames)
        {
            foreach (var jobName in jobNames)
            {
                _backupManager.ResumeJob(jobName);
            }
        }

        public void StopJobs(List<string> jobNames)
        {
            foreach (var jobName in jobNames)
            {
                _backupManager.StopJob(jobName);
            }
        }

        public void StartJobs(List<string> jobNames)
        {
            _backupManager.ExecuteJobs(jobNames.ToArray());
        }
    }
}