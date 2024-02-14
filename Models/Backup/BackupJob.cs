using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Configuration;

namespace EasySave;

// Class representing a backup job with properties

public class BackupJob
{
    public string Name { get; set; }
    public string SourceDir { get; set; }
    public string DestinationDir { get; set; }
    public string Type { get; set; }

    protected int TotalFilesToCopy = 0;
    protected long TotalFilesSize = 0;
    protected int NbFilesLeftToDo = 0;
    protected double Progression = 0;

    // Constructor to initialize a new BackupJob with basic details
    public BackupJob(string name, string sourceDir, string destinationDir, string type)
    {
        Name = name;
        SourceDir = sourceDir;
        DestinationDir = destinationDir;
        Type = type;
    }


    // Starts the backup process
    public virtual void Start()
    {
        Console.WriteLine($"Starting backup: {Name}");
    }

    protected const int MaxBufferSize = 1024 * 1024; // 1 MB

    // Copies a file from source to destination with a buffer to optimize performance
    protected void CopyFileWithBuffer(string sourceFile, string destinationFile)
    {
        long fileSize = new FileInfo(sourceFile).Length;

        int bufferSize = DetermineBufferSize(fileSize);

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            using (FileStream sourceStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read))
            {
                using (FileStream destStream = new FileStream(destinationFile, FileMode.Create, FileAccess.Write))
                {
                    byte[] buffer = new byte[bufferSize];
                    int bytesRead;
                    while ((bytesRead = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        destStream.Write(buffer, 0, bytesRead);
                    }
                }
            }

            stopwatch.Stop();

            // Log in log.json file
            LoggingModel.LogFileTransfer(
                this.Name, // Supposed you have an attribute Name in your class to identify backup
                sourceFile,
                destinationFile,
                fileSize,
                stopwatch.ElapsedMilliseconds
            );
            Console.WriteLine(
                $"Transfert log for {sourceFile} done. Transert time : {stopwatch.ElapsedMilliseconds} ms.");
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            Console.WriteLine($"Error during the file copying : {ex.Message}");
            // Be sure to have error management in your LoggingModel
            LoggingModel.LogFileTransfer(
                this.Name, // Supposed you have an attribute Name in your class to identify your backup
                sourceFile,
                destinationFile,
                fileSize,
                stopwatch.ElapsedMilliseconds,
                true // Return an error 
            );
        }
    }

    // Determines the buffer size based on the file size
    protected int DetermineBufferSize(long fileSize)
    {
        if (fileSize <= MaxBufferSize)
            return (int)fileSize;
        else
            return MaxBufferSize;
    }


    // Updates the state of the backup job with progress and other details
    protected void UpdateState(string state)
    {
        StateModel.UpdateBackupState(this, state, TotalFilesToCopy, TotalFilesSize, NbFilesLeftToDo, Progression,
            "state.json");
    }
}