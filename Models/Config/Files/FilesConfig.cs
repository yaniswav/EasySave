using System;
using System.Configuration;

namespace EasySave;

public partial class ConfigModel
{
    private const string MaxBackupFileSizeKey = "MaxBackupFileSize";

    public int MaxBackupFileSize
    {
        get
        {
            int.TryParse(ConfigurationManager.AppSettings[MaxBackupFileSizeKey], out int maxFileSize);
            return maxFileSize;
        }
        set { UpdateAppSettings(MaxBackupFileSizeKey, value.ToString()); }
    }
}