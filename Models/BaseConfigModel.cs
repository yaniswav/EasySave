using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace EasySaveConsole
{
    public class LocaleConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("CurrentLocale")]
        public string CurrentLocale
        {
            get { return (string)this["CurrentLocale"]; }
            set { this["CurrentLocale"] = value; }
        }
    }

    public class BackupJobsConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("", IsDefaultCollection = true)]
        public BackupJobConfigCollection Jobs
        {
            get { return (BackupJobConfigCollection)this[""]; }
        }
    }

    public class BackupJobConfigCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new BackupJobConfigElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((BackupJobConfigElement)element).Name;
        }
    }

    public class BackupJobConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("key", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string)this["key"]; }
            set { this["key"] = value; }
        }

        [ConfigurationProperty("sourceDir", IsRequired = true)]
        public string SourceDir
        {
            get { return (string)this["sourceDir"]; }
            set { this["sourceDir"] = value; }
        }

        [ConfigurationProperty("destinationDir", IsRequired = true)]
        public string DestinationDir
        {
            get { return (string)this["destinationDir"]; }
            set { this["destinationDir"] = value; }
        }

        [ConfigurationProperty("type", IsRequired = true)]
        public string Type
        {
            get { return (string)this["type"]; }
            set { this["type"] = value; }
        }
    }
}