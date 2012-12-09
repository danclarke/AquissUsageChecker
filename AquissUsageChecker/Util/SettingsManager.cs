using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace AquissUsageChecker.Util
{
    public static class SettingsManager
    {
        public const string KeyHashCode = "hashcode";
        public const string KeyAllowance = "allowance";

        private const string ApplicationName = "AquissUsageChecker";
        private const string ConfigFilename = "settings.xml";

        private static readonly string _filename;

        private static readonly object _xmlDocLock = new object();
        private static readonly XDocument _xmlDoc;

        static SettingsManager()
        {
            lock (_xmlDocLock)
            {
                var documentsDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var appFolder = Path.Combine(documentsDir, ApplicationName);
                _filename = Path.Combine(appFolder, ConfigFilename);

                if (!Directory.Exists(appFolder))
                    Directory.CreateDirectory(appFolder);

                if (File.Exists(_filename))
                    _xmlDoc = XDocument.Load(_filename);
                else
                    _xmlDoc = new XDocument(new XElement("settings"));
            }
        }

        public static void SetSetting(string key, string value)
        {
            lock (_xmlDocLock)
            {
                var element = _xmlDoc.Element("settings").Element(key);

                if (element != null)
                    element.Value = value;
                else
                {
                    element = new XElement(key, value);
                    _xmlDoc.Element("settings").Add(element);
                }

                _xmlDoc.Save(_filename);
            }
        }

		/// <summary>
		/// Gets the setting.
		/// </summary>
		/// <returns>
		/// The setting, string.Empty if not present
		/// </returns>
		/// <param name='key'>
		/// Key.
		/// </param>
        public static string GetSetting(string key)
        {
            lock (_xmlDocLock)
            {
                var element = _xmlDoc.Element("settings").Element(key);

                if (element != null)
                    return element.Value;
                else
                    return string.Empty;
            }
        }
    }
}

