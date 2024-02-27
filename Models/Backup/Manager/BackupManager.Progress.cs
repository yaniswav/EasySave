using System.Collections.Generic;
using System.Linq;
using System;

namespace EasySave
{
    public partial class BackupManager
    {
        public Dictionary<string, double> GetBackupProgress()
        {
            return _backupJobs
                .GroupBy(job => job.Name)
                .ToDictionary(group => group.Key, group => group.Average(job => job.Progression));
        }
        
        public void ListActiveThreads()
        {
            Console.WriteLine("Listing Active Threads:");
            foreach (var entry in _backupThreads)
            {
                Console.WriteLine($"entry key: {entry.Key} entry value: {entry.Value}");
                string threadStatus = entry.Value.IsAlive ? "Active" : "Not Active";
                Console.WriteLine($"Job: {entry.Key}, Thread Status: {threadStatus}");
            }
        }


        
    }
}