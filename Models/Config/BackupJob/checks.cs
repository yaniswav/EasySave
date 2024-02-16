using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;

namespace EasySave;

public partial class ConfigModel
{
    public bool BackupJobExists(string jobName)
    {
        if (string.IsNullOrWhiteSpace(jobName))
        {
            throw new ArgumentException("Job name cannot be null or whitespace", nameof(jobName));
        }

        var backupJobs = GetBackupJobs();
        return backupJobs.Any(job => job.Name.Equals(jobName, StringComparison.OrdinalIgnoreCase));
    }

    public bool IsValid(BackupJobConfig job)
    {
        try
        {
            ValidateString(job.Name, "Name");
            ValidateString(job.SourceDir, "SourceDir");
            ValidateString(job.DestinationDir, "DestinationDir");

            return !string.IsNullOrEmpty(job.Name) &&
                   !string.IsNullOrEmpty(job.SourceDir) &&
                   !string.IsNullOrEmpty(job.DestinationDir) &&
                   !job.SourceDir.Equals(job.DestinationDir, StringComparison.OrdinalIgnoreCase);
        }
        catch (ArgumentException)
        {
            return false;
        }
    }

    private static void ValidateString(string value, string propertyName)
    {
        if (value.Any(c => c < 32 || c > 126 || (c >= 58 && c <= 63 && !IsAllowedColon(c, value))))
        {
            var invalidChars = value
                .Where(c => c < 32 || c > 126 || (c >= 58 && c <= 63 && !IsAllowedColon(c, value))).ToArray();
            var invalidCharString = new string(invalidChars);
            throw new ArgumentException($"{propertyName} contains invalid characters: {invalidCharString}",
                propertyName);
        }
    }

    private static bool IsAllowedColon(char c, string value)
    {
        // Allow colons that follow a single letter (drive letter)
        int index = value.IndexOf(c);
        return c == 58 && index == 1 && char.IsLetter(value[0]);
    }
}