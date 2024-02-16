using System;
using System.Diagnostics;
using System.IO;

//namespace EasySave;

// Verify is cryptosoft is installed or not
public class CryptosoftChecker
{
    public bool IsCryptosoftInstalled()
    {
        try
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "cryptosoft.exe", // Name of executable file
            };

            using (Process process = Process.Start(startInfo))
            {
                process.WaitForExit();

                if (process.ExitCode == 0)
                {
                    return true;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de la vérification de cryptosoft: {ex.Message}");
        }

        return false; // Return false because cryptosoft wasn't find or not installed
    }
}