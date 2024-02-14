using System;
using System.Globalization;
using System.Resources;

public class Utilities
{
    public ResourceManager _resourceManager;

    public Utilities(ResourceManager resourceManager)
    {
        _resourceManager = resourceManager;
    }

    public void DisplayMessage(string resourceKey)
    {
        Console.WriteLine(_resourceManager.GetString(resourceKey, CultureInfo.CurrentUICulture));
    }
    
    
}