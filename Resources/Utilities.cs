using System;
using System.Globalization;
using System.Resources;
using EasySave.ViewModels;

public class Utilities
{
    public ResourceManager ResourceManager { get; private set; }

    public Utilities(ResourceManager resourceManager)
    {
        ResourceManager = resourceManager;
    }
    
    public string GetMessage(string resourceKey)
    {
        return ResourceManager.GetString(resourceKey, CultureInfo.CurrentUICulture);
    }

    public void DisplayMessage(string resourceKey)
    {
        Console.WriteLine(ResourceManager.GetString(resourceKey, CultureInfo.CurrentUICulture));
    }

    public void ChangeCulture(string culture)
    {
        CultureInfo newCultureInfo = new CultureInfo(culture);
        CultureInfo.CurrentUICulture = newCultureInfo;
        CultureInfo.CurrentCulture = newCultureInfo;
    }
}