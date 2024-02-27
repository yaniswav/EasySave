using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace EasySave
{
    public class DifferentialBackup : BackupJob
    {
        public DifferentialBackup(string name, string sourceDir, string destinationDir)
            : base(name, sourceDir, destinationDir, "Differential")
        {
        }

        protected override void PerformBackup(string sourceDir, string destinationDir,
            CancellationToken cancellationToken,
            ManualResetEvent pauseEvent)
        {
            CopyDirectory(sourceDir, destinationDir, cancellationToken, pauseEvent);
        }

        protected override void CopyDirectory(string sourceDir, string destinationDir,
            CancellationToken cancellationToken, ManualResetEvent pauseEvent)
        {
            if (!Directory.Exists(destinationDir))
            {
                Directory.CreateDirectory(destinationDir);
            }

            // Filtre les fichiers en fonction de leur priorité et de la taille
            var sourceFiles = Directory.GetFiles(sourceDir);
            var priorityFiles = sourceFiles.Where(file => config.ExtPrio.Contains(Path.GetExtension(file).ToLower()))
                .ToList();
            var nonPriorityFiles = sourceFiles
                .Where(file => !config.ExtPrio.Contains(Path.GetExtension(file).ToLower())).ToList();

            // Copie d'abord les fichiers prioritaires
            foreach (var sourceFile in priorityFiles)
            {
                cancellationToken.ThrowIfCancellationRequested();
                pauseEvent.WaitOne();

                var destFile = Path.Combine(destinationDir, Path.GetFileName(sourceFile));
                if (ShouldCopyFile(sourceFile, destFile))
                {
                    CopyFileWithBuffer(sourceFile, destFile);
                    UpdateProgress(sourceFile);
                }
            }

            // Copie ensuite les fichiers non prioritaires, en respectant la restriction de taille
            foreach (var sourceFile in nonPriorityFiles)
            {
                cancellationToken.ThrowIfCancellationRequested();
                pauseEvent.WaitOne();

                long fileSize = new FileInfo(sourceFile).Length;
                if (fileSize <= config.MaxBackupFileSize * 1024)
                {
                    var destFile = Path.Combine(destinationDir, Path.GetFileName(sourceFile));
                    if (ShouldCopyFile(sourceFile, destFile))
                    {
                        CopyFileWithBuffer(sourceFile, destFile);
                        UpdateProgress(sourceFile);
                    }
                }
            }

            // Récursion pour les sous-dossiers
            foreach (var dir in Directory.GetDirectories(sourceDir))
            {
                cancellationToken.ThrowIfCancellationRequested();
                pauseEvent.WaitOne();

                var destDir = Path.Combine(destinationDir, Path.GetFileName(dir));
                CopyDirectory(dir, destDir, cancellationToken, pauseEvent);
            }
        }
    }
}