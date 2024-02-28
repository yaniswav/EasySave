using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EasySave
{
    public abstract class BackupJob
    {
        public string Name { get; }
        public string SourceDir { get; }
        public string DestinationDir { get; }
        public string Type { get; protected set; }

        public string State { get; private set; }

        public long TotalFilesSize;
        public int TotalFilesToCopy;
        public int NbFilesLeftToDo;
        public double Progression { get; protected set; }
        private static LoggingModel logger;

        protected ConfigModel config = ConfigModel.Instance;

        protected BackupJob(string name, string sourceDir, string destinationDir, string type)
        {
            Name = name;
            SourceDir = sourceDir;
            DestinationDir = destinationDir;
            Type = type;
            InitializeLogger();
        }

        private void InitializeLogger()
        {
            // Suppose ConfigModel.OutputFormat retourne "XML" ou "JSON"
            string outputFormat = config.OutputFormat;
            logger = outputFormat == "XML" ? new XmlLogger() : new JsonLogger();
        }

        protected void InitializeTrackingProperties()
        {
            var allFiles = Directory.GetFiles(SourceDir, "*", SearchOption.AllDirectories);
            TotalFilesSize = allFiles.Sum(file => new FileInfo(file).Length);
            TotalFilesToCopy = allFiles.Length;
            NbFilesLeftToDo = TotalFilesToCopy;
        }

        public virtual void Start(CancellationToken cancellationToken, ManualResetEvent pauseEvent)
        {
            InitializeTrackingProperties();

            try
            {
                UpdateState("ACTIVE");
                PerformBackup(SourceDir, DestinationDir, cancellationToken, pauseEvent);
                UpdateState("END");
            }
            catch (OperationCanceledException)
            {
                UpdateState("CANCELLED");
            }
            catch (Exception)
            {
                UpdateState("ERROR");
            }
        }

        protected abstract void PerformBackup(string sourceDir, string destinationDir,
            CancellationToken cancellationToken, ManualResetEvent pauseEvent);


        protected abstract void CopyDirectory(string sourceDir, string destinationDir,
            CancellationToken cancellationToken,
            ManualResetEvent pauseEvent);


        protected int MaxBufferSize = 1024 * 1024;

        // Copy a file from source to destination with a buffer to optimize performance
        protected void CopyFileWithBuffer(string sourceFile, string destinationFile)
        {
            long fileSize = new FileInfo(sourceFile).Length;
            int bufferSize = DetermineBufferSize(fileSize);
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                if (!config.ExtToEncrypt.Contains(Path.GetExtension(sourceFile).ToLower()))
                {
                    using (FileStream sourceStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read))
                    {
                        using (FileStream destStream =
                               new FileStream(destinationFile, FileMode.Create, FileAccess.Write))
                        {
                            byte[] buffer = new byte[bufferSize];
                            int bytesRead;
                            while ((bytesRead = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                destStream.Write(buffer, 0, bytesRead);
                            }
                        }
                    }
                }

                else
                {
                    EncryptFile(sourceFile); // Cryptez le fichier source sans le copier
                }

                stopwatch.Stop();
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                Console.WriteLine($"Error during the file copying : {ex.Message}");
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


        protected void UpdateProgress(string fileCopied)
        {
            NbFilesLeftToDo--;
            Progression = TotalFilesToCopy > 0 ? 100.0 * (TotalFilesToCopy - NbFilesLeftToDo) / TotalFilesToCopy : 100;
        }

        public bool HasPriorityFile(string file)
        {
            return config.ExtPrio.Contains(Path.GetExtension(file).ToLower().TrimStart('.'));
        }


        private void EncryptFile(string filePath)
        {
        }

        protected bool ShouldCopyFile(string sourceFile, string destinationFile)
        {
            if (!File.Exists(destinationFile)) return true;
            var sourceFileInfo = new FileInfo(sourceFile);
            var destFileInfo = new FileInfo(destinationFile);
            return sourceFileInfo.LastWriteTime > destFileInfo.LastWriteTime ||
                   sourceFileInfo.Length != destFileInfo.Length;
        }

        protected void UpdateState(string state)
        {
            State = state;
            StateModel.UpdateBackupState(this);
        }
    }
}