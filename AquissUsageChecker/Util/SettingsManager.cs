/*
    AquissUsageChecker - Realtime display of broadband usage on Aquiss
    Copyright (C) 2013  Dan Clarke

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace AquissUsageChecker.Util
{
    /// <summary>
    /// Persistent settings storage
    /// </summary>
    /// <remarks>Saves to an XML file</remarks>
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
            // Open up the settings XML doc, and create if neccesary
            lock (_xmlDocLock)
            {
                // TODO: Use a better folder, this one just isn't new-Mac
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

        /// <summary>
        /// Save a new setting, or update an existing one
        /// </summary>
        /// <param name="key">Setting name</param>
        /// <param name="value">Value</param>
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
		/// Gets an existing setting
		/// </summary>
		/// <returns>
		/// The setting, string.Empty if not present
		/// </returns>
		/// <param name='key'>
		/// Setting name
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

