using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;

namespace EasySave;

// Used to store the current locale configuration
public class LocaleConfig
{
    public string CurrentLocale { get; set; }
}