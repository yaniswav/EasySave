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

            var allFiles = Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories);
            var priorityFiles = allFiles.Where(file => HasPriorityFile(file)).ToArray();
            var nonPriorityFiles = allFiles.Except(priorityFiles).ToArray();

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

            foreach (var sourceFile in nonPriorityFiles)
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