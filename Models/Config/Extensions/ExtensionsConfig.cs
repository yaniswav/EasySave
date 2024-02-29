using System;
using System.Configuration;

namespace EasySave;

public partial class ConfigModel
{
    public string CryptoSoftPath
    {
        get => ConfigurationManager.AppSettings["CryptoSoftPath"];
        set => UpdateAppSettings("CryptoSoftPath", value);
    }
    
    public string EncryptionKey
    {
        get => ConfigurationManager.AppSettings["EncryptionKey"];
        set => UpdateAppSettings("EncryptionKey", value);
    }

    public List<string> ExtToEncrypt
    {
        get => (ConfigurationManager.AppSettings["ExtToEncrypt"] ?? string.Empty)
            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
            .ToList();
        set => UpdateAppSettings("ExtToEncrypt", String.Join(",", value));
    }

    public List<string> ExtPrio
    {
        get => (ConfigurationManager.AppSettings["ExtPrio"] ?? string.Empty)
            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
            .ToList();
        set => UpdateAppSettings("ExtPrio", String.Join(",", value));
    }
}