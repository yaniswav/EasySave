using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace EasySave
{
    public class CompleteBackup : BackupJob
    {
        public CompleteBackup(string name, string sourceDir, string destinationDir)
            : base(name, sourceDir, destinationDir, "Complete")
        {
        }

        protected override void PerformBackup(string sourceDir, string destinationDir,
            CancellationToken cancellationToken, ManualResetEvent pauseEvent)
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

            var allFiles = Directory.GetFiles(sourceDir);
            var priorityFiles = allFiles.Where(file => config.ExtPrio.Contains(Path.GetExtension(file).ToLower()))
                .ToList();
            var nonPriorityFiles = allFiles.Where(file => !config.ExtPrio.Contains(Path.GetExtension(file).ToLower()))
                .ToList();

            // Copie des fichiers prioritaires en premier
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

            // Copie des fichiers non prioritaires en respectant la restriction de taille
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

            // Gestion récursive des sous-dossiers
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