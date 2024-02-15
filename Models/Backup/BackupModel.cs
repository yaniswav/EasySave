using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Configuration;

namespace EasySave
{
    public class BackupModel
    {
        public void StartBackup(BackupJob job)
        {
            switch (job.Type)
            {
                case "Complete":
                    // Créer et démarrer un backup complet
                    var completeBackup = new CompleteBackup(job.Name, job.SourceDir, job.DestinationDir);
                    completeBackup.Start();
                    break;
                case "Differential":
                    // Créer et démarrer un backup différentiel
                    var differentialBackup = new DifferentialBackup(job.Name, job.SourceDir, job.DestinationDir);
                    differentialBackup.Start();
                    break;
                default:
                    throw new InvalidOperationException("Type de sauvegarde non supporté.");
            }
        }
    }
}