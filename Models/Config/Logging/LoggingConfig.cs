using System;
using System.Configuration;

namespace EasySave;

public partial class ConfigModel
{
    private const string OutputFormatKey = "OutputFormat";

    public string OutputFormat
    {
        get => ConfigurationManager.AppSettings[OutputFormatKey];
        set => UpdateAppSettings(OutputFormatKey, value);
    }
}