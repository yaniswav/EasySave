using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace EasySave;

// Derived class for performing a differential backup
public class DifferentialBackup : BackupJob
{
    public DifferentialBackup(string name, string sourceDir, string destinationDir)
        : base(name, sourceDir, destinationDir, "Differential")
    {
    }

    // Overrides the Start method to perform a differential backup
    public override void Start(CancellationToken cancellationToken, ManualResetEvent pauseEvent)
    {
        InitializeTrackingProperties();
        UpdateState("ACTIVE");

        try
        {
            PerformDifferentialBackup(SourceDir, DestinationDir, cancellationToken, pauseEvent);
            UpdateProgress(null);
            UpdateState("END");
        }

        catch (OperationCanceledException)
        {
            Console.WriteLine("Backup cancelled.");
            UpdateState("CANCELLED");
        }

        catch (Exception ex)
        {
            Console.WriteLine($"Error during backup: {ex.Message}");
            UpdateState("ERROR");
        }
    }


    // Initializes properties for tracking which files need to be copied
    private void InitializeTrackingProperties()
    {
        var allFiles = Directory.GetFiles(SourceDir, "*", SearchOption.AllDirectories);
        TotalFilesSize = allFiles.Sum(file => new FileInfo(file).Length);

        TotalFilesToCopy = allFiles.Count(file =>
        {
            string destFile = Path.Combine(DestinationDir, file.Substring(SourceDir.Length + 1));
            return ShouldCopyFile(file, destFile);
        });

        NbFilesLeftToDo = TotalFilesToCopy;
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


    // Performs the differential backup by copying only modified or new files
    private void PerformDifferentialBackup(string sourceDir, string destinationDir, CancellationToken cancellationToken,
        ManualResetEvent pauseEvent)
    {
        if (!Directory.Exists(destinationDir))
            Directory.CreateDirectory(destinationDir);

        Parallel.ForEach(Directory.GetFiles(sourceDir), sourceFile =>
        {
            var destFile = Path.Combine(destinationDir, Path.GetFileName(sourceFile));
            if (ShouldCopyFile(sourceFile, destFile))
            {
                CopyFileWithBuffer(sourceFile, destFile);
            }
            else
            {
                Console.WriteLine($"Unchanged: {sourceFile}");
            }
        });

        // Dans la méthode PerformDifferentialBackup
        Parallel.ForEach(Directory.GetDirectories(sourceDir), directory =>
        {
            var destDir = Path.Combine(destinationDir, Path.GetFileName(directory));
            PerformDifferentialBackup(directory, destDir, cancellationToken,
                pauseEvent); // Utiliser directement les paramètres existants
        });
    }

    // Determines if a file should be copied based on modification date and size
    private bool ShouldCopyFile(string sourceFile, string destinationFile)
    {
        if (!File.Exists(destinationFile))
        {
            Console.WriteLine($"New file: {sourceFile}");
            return true;
        }

        var sourceFileInfo = new FileInfo(sourceFile);
        var destFileInfo = new FileInfo(destinationFile);

        bool isModified = sourceFileInfo.LastWriteTime > destFileInfo.LastWriteTime ||
                          sourceFileInfo.Length != destFileInfo.Length;

        if (isModified)
        {
            Console.WriteLine($"Replacing: {sourceFile}");
            Console.WriteLine(
                $"  Source Last Modified: {sourceFileInfo.LastWriteTime}, Size: {sourceFileInfo.Length} bytes");
            Console.WriteLine(
                $"  Dest. Last Modified: {destFileInfo.LastWriteTime}, Size: {destFileInfo.Length} bytes");
        }

        return isModified;
    }
}