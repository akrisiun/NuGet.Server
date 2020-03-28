// #if VS15 // NET452

using System.Collections.Specialized;
using ConfigurationManager = global::System.Configuration.ConfigurationManager;
using ConnectionStringSettingsCollection = global::System.Configuration.ConnectionStringSettingsCollection;
using ConnectionStringSettings = global::System.Configuration.ConnectionStringSettings;

namespace System
{
    public class Config
    {
        public static class Manager
        {
            //     Gets the System.Configuration.AppSettingsSection data for the current application's
            public static NameValueCollection AppSettings { get => ConfigurationManager.AppSettings; }

            //     Gets the System.Configuration.ConnectionStringsSection data for the current application's configuration.
            public static ConnectionStringSettingsCollection ConnectionStrings { get => ConfigurationManager.ConnectionStrings; }

            static NameValueCollection _sqlconfig;
            public static NameValueCollection SqlSettings {
                get {
                    if (_sqlconfig == null)
                    {
                        _sqlconfig = new NameValueCollection();
                        foreach (ConnectionStringSettings item in ConnectionStrings)
                        {
                            _sqlconfig.Add(item.Name, item.ConnectionString);
                        }
                    }
                    return _sqlconfig;
                }
            }

        }
    }
}