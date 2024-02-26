using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Configuration;
using System.Threading;

namespace EasySave;

// Class representing a backup job with properties

public class BackupJob
{
    public string Name { get; set; }
    public string SourceDir { get; set; }
    public string DestinationDir { get; set; }
    public string Type { get; set; }

    public string State { get; private set; }

    public int TotalFilesToCopy = 0;
    public long TotalFilesSize = 0;
    public int NbFilesLeftToDo = 0;
    public double Progression = 0;

    private static readonly LoggingModel logger = CreateLogger();

    // Constructeur statique pour initialiser le logger basé sur la configuration
    private static LoggingModel CreateLogger()
    {
        string loggerType = ConfigModel.OutputFormat;
        Console.WriteLine($"loggerType : {loggerType}");
        return loggerType == "XML" ? (LoggingModel)new XmlLogger() : new JsonLogger();
    }

    // Constructor to initialize a new BackupJob with basic details
    public BackupJob(string name, string sourceDir, string destinationDir, string type)
    {
        Name = name;
        SourceDir = sourceDir;
        DestinationDir = destinationDir;
        Type = type;
    }

    public List<string> ExtensionsToEncrypt { get; set; }
    private List<string> FilesToEncrypt;

    public BackupJob(string name, string sourceDir, string destinationDir, string type,
        List<string> extensionsToEncrypt)
        : this(name, sourceDir, destinationDir, type) // Appel du constructeur existant
    {
        ExtensionsToEncrypt = extensionsToEncrypt ?? new List<string>();
        FilesToEncrypt = new List<string>();
    }

    protected void PrepareEncryptionList()
    {
        FilesToEncrypt = Directory.GetFiles(SourceDir, "*.*", SearchOption.AllDirectories)
            .Where(file => ExtensionsToEncrypt.Contains(Path.GetExtension(file).ToLower()))
            .ToList();
    }


    // Starts the backup process
    public virtual void Start(CancellationToken cancellationToken, ManualResetEvent pauseEvent)
    {
        PrepareEncryptionList();
        Console.WriteLine($"Starting backup: {Name}");
    }

    protected const int MaxBufferSize = 1024 * 1024; // 1 MB

    // Copy a file from source to destination with a buffer to optimize performance
    protected void CopyFileWithBuffer(string sourceFile, string destinationFile)
    {
        
        Console.WriteLine($"Logger : {logger}");

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


            var logEntry = new XmlLogger
            {
                Name = this.Name,
                FileSource = sourceFile,
                FileTarget = destinationFile,
                FileSize = fileSize,
                FileTransferTime = stopwatch.ElapsedMilliseconds,
                Time = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                Error = false
            };


            LoggingModel.EnqueueLog(logEntry);
            // Console.WriteLine(
            // $"Transfert log for {sourceFile} done. Transert time : {stopwatch.ElapsedMilliseconds} ms.");
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            Console.WriteLine($"Error during the file copying : {ex.Message}");

            // Configurer et écrire le log en cas d'erreur

            var logEntry = new XmlLogger
            {
                Name = this.Name,
                FileSource = sourceFile,
                FileTarget = destinationFile,
                FileSize = fileSize,
                FileTransferTime = stopwatch.ElapsedMilliseconds,
                Time = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                Error = false
            };


            LoggingModel.EnqueueLog(logEntry);
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
        State = state;
        StateModel.UpdateBackupState(this);
    }
}