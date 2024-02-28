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

            var allFiles = Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories);
            var priorityFiles = allFiles.Where(file => HasPriorityFile(file)).ToArray();
            var nonPriorityFiles = allFiles.Except(priorityFiles).ToArray();
            
            Console.WriteLine($"MaxBackupFileSize : {config.MaxBackupFileSize}");
            foreach (var sourceFile in priorityFiles)
            {
                cancellationToken.ThrowIfCancellationRequested();
                pauseEvent.WaitOne();

                long fileSize = new FileInfo(sourceFile).Length;
                Console.WriteLine($"fileSize: {fileSize}");

                var destinationFile = Path.Combine(destinationDir, sourceFile.Substring(SourceDir.Length + 1));
                CopyFileWithBuffer(sourceFile, destinationFile);
                UpdateProgress(sourceFile);
            }

            foreach (var sourceFile in nonPriorityFiles)
            {
                cancellationToken.ThrowIfCancellationRequested();
                pauseEvent.WaitOne();
                long fileSize = 10;
                // long fileSize = new FileInfo(sourceFile).Length;
                if (fileSize <= config.MaxBackupFileSize)
                {
                    var destinationFile = Path.Combine(destinationDir, sourceFile.Substring(SourceDir.Length + 1));
                    CopyFileWithBuffer(sourceFile, destinationFile);
                    UpdateProgress(sourceFile);
                }
            }

            foreach (var dir in Directory.GetDirectories(sourceDir))
            {
                cancellationToken.ThrowIfCancellationRequested();
                pauseEvent.WaitOne();

                var destDir = Path.Combine(destinationDir, dir.Substring(SourceDir.Length + 1));
                CopyDirectory(dir, destDir, cancellationToken, pauseEvent);
            }
        }
    }
}