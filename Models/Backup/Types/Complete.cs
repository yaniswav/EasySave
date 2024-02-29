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

            Dictionary<string, string> encryptionMappings = new Dictionary<string, string>();

            // Encrypt and then save priority files
            foreach (var file in priorityFiles)
            {
                cancellationToken.ThrowIfCancellationRequested();
                pauseEvent.WaitOne();

                var destinationFile = Path.Combine(destinationDir, file.Substring(SourceDir.Length + 1));

                if (config.ExtToEncrypt.Contains(Path.GetExtension(file).ToLower().TrimStart('.')))
                {
                    encryptionMappings.Add(file, destinationFile);
                }
                else
                {
                    CopyFileWithBuffer(file, destinationFile);
                    UpdateProgress(file);
                }
            }

            EncryptFiles(encryptionMappings, config.EncryptionKey);
            encryptionMappings.Clear(); // Clear mappings after encryption

            // Encrypt and then save non-priority files
            foreach (var file in nonPriorityFiles)
            {
                cancellationToken.ThrowIfCancellationRequested();
                pauseEvent.WaitOne();

                var destinationFile = Path.Combine(destinationDir, file.Substring(SourceDir.Length + 1));

                if (config.ExtToEncrypt.Contains(Path.GetExtension(file).ToLower().TrimStart('.')))
                {
                    encryptionMappings.Add(file, destinationFile);
                }
                else
                {
                    CopyFileWithBuffer(file, destinationFile);
                    UpdateProgress(file);
                }
            }

            EncryptFiles(encryptionMappings, config.EncryptionKey);

            // Process subdirectories
            foreach (var dir in Directory.GetDirectories(sourceDir))
            {
                cancellationToken.ThrowIfCancellationRequested();
                pauseEvent.WaitOne();

                var nextDestDir = Path.Combine(destinationDir, dir.Substring(SourceDir.Length + 1));
                CopyDirectory(dir, nextDestDir, cancellationToken, pauseEvent);
            }
        }
    }
}