using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;

namespace EasySave;

// Class to represent the configuration for a single backup job

public class BackupJobConfig
{
    public string Name { get; set; }
    public string SourceDir { get; set; }
    public string DestinationDir { get; set; }
    public string Type { get; set; }

    // Converts the backup job configuration to a string for storage
    public override string ToString()
    {
        return $"{Name},{SourceDir},{DestinationDir},{Type}";
    }

    // Creates a backup job configuration from a string
    public static BackupJobConfig FromString(string data)
    {
        var parts = data.Split(',');
        if (parts.Length != 4)
        {
            throw new FormatException("Invalid backup job data format.");
        }

        return new BackupJobConfig
        {
            Name = parts[0],
            SourceDir = parts[1],
            DestinationDir = parts[2],
            Type = parts[3]
        };
    }

    public bool IsValid()
    {
        try
        {
            ValidateString(Name, "Name");
            ValidateString(SourceDir, "SourceDir");
            ValidateString(DestinationDir, "DestinationDir");

            return !string.IsNullOrEmpty(Name) &&
                   !string.IsNullOrEmpty(SourceDir) &&
                   !string.IsNullOrEmpty(DestinationDir) &&
                   !SourceDir.Equals(DestinationDir, StringComparison.OrdinalIgnoreCase);
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