﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Configuration;
using System.Threading;

namespace EasySave
{
    // Derived class for performing a complete backup
    public class CompleteBackup : BackupJob
    {
        public CompleteBackup(string name, string sourceDir, string destinationDir)
            : base(name, sourceDir, destinationDir, "Complete")
        {
        }

        // Overrides the Start method to perform a complete backup
        public override void Start(CancellationToken cancellationToken, ManualResetEvent pauseEvent)
        {
            try
            {
                InitializeTrackingProperties();
                UpdateState("ACTIVE");
                Console.WriteLine($"Starting complete backup: {Name}");
                CopyDirectory(SourceDir, DestinationDir, cancellationToken, pauseEvent);
                UpdateState("END");
                Console.WriteLine($"Complete backup '{Name}' finished successfully.");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine($"Backup '{Name}' cancelled.");
                UpdateState("CANCELLED");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during complete backup '{Name}': {ex.Message}");
                UpdateState("ERROR");
            }
        }

        // Initializes properties to track progress and size of the backup
        private void InitializeTrackingProperties()
        {
            // Console.WriteLine($"Initializing tracking properties for complete backup '{Name}'...");
            var allFiles = Directory.GetFiles(SourceDir, "*", SearchOption.AllDirectories);
            TotalFilesToCopy = allFiles.Length;
            TotalFilesSize = allFiles.Sum(file => new FileInfo(file).Length);
            NbFilesLeftToDo = TotalFilesToCopy;
            // Console.WriteLine($"Tracking properties initialized for complete backup '{Name}'.");
        }

        // Updates the progress of the backup process
        private void UpdateProgress(string fileCopied)
        {
            if (NbFilesLeftToDo > 0)
            {
                NbFilesLeftToDo--;
            }

            Progression = TotalFilesToCopy > 0 ? 100.0 * (TotalFilesToCopy - NbFilesLeftToDo) / TotalFilesToCopy : 100;
            UpdateState("ACTIVE");
        }


        // Recursively copies directories and files from source to destination
        private void CopyDirectory(string sourceDir, string destinationDir, CancellationToken cancellationToken,
            ManualResetEvent pauseEvent)
        {
            if (!Directory.Exists(destinationDir))
            {
                Directory.CreateDirectory(destinationDir);
                // Console.WriteLine($"Directory created: {destinationDir}");
            }

            foreach (var file in Directory.GetFiles(sourceDir))
            {
                cancellationToken.ThrowIfCancellationRequested();
                pauseEvent.WaitOne();

                var destFile = Path.Combine(destinationDir, Path.GetFileName(file));
                // Console.WriteLine($"Copying file: {file} to {destFile}");
                CopyFileWithBuffer(file, destFile);
                UpdateProgress(file);
            }

            foreach (var directory in Directory.GetDirectories(sourceDir))
            {
                var destDir = Path.Combine(destinationDir, Path.GetFileName(directory));
                // Console.WriteLine($"Copying directory: {directory} to {destDir}");
                CopyDirectory(directory, destDir, cancellationToken, pauseEvent);
            }
        }
    }
}