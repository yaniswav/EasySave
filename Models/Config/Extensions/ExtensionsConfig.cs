using System;
using System.Configuration;

namespace EasySave;

public partial class ConfigModel
{
    public static string ExtToEncrypt
    {
        get => ConfigurationManager.AppSettings["ExtToEncrypt"];
        set => UpdateAppSettings("ExtToEncrypt", value);
    }

    public static string CryptoSoftPath
    {
        get => ConfigurationManager.AppSettings["CryptoSoftPath"];
        set => UpdateAppSettings("CryptoSoftPath", value);
    }

    public static string ExtPrio
    {
        get => ConfigurationManager.AppSettings["ExtPrio"];
        set => UpdateAppSettings("ExtPrio", value);
    }
}