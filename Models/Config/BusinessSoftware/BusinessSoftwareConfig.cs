using System;
using System.Configuration;

namespace EasySave;

public partial class ConfigModel
{
    public string BusinessSoftware
    {
        get => ConfigurationManager.AppSettings["BusinessSoftwareKey"];
        set => UpdateAppSettings("BusinessSoftwareKey", value);
    }
}