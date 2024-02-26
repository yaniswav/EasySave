using System;
using System.Configuration;

namespace EasySave;

public partial class ConfigModel
{
    public static string BusinessSoftware
    {
        get => ConfigurationManager.AppSettings["BusinessSoftwareKey"];
        set => UpdateAppSettings("BusinessSoftwareKey", value);
    }
}