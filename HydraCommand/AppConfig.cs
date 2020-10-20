//
//  AppConfig.cs
//
//  Author:
//       Christian Lachapelle <lachapellec@gmail.com>
//
//  Copyright (c) 2020 Devworks8
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Configuration;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace HydraCommand
{
    public static class Sections
    {
        public static List<string> defaultSections =
            new List<string> { "bot", "cfg" };
    }

    public static class DefaultConfig
    {
        // Hold Default Config Settings
        public static Dictionary<string, NameValueCollection> configDefaults =
            new Dictionary<string, NameValueCollection>();

        public static void ParseDefaults()
        {
            foreach (string section in Sections.defaultSections)
            {
                configDefaults.Add(section, _GetSettings(section));
            }
        }

        public static NameValueCollection _GetSettings(string section)
        {
            return (NameValueCollection)ConfigurationManager.GetSection(section);
        }

        public static dynamic GetSettings(string section)
        {
            string results = "";
            if (section.ToLower() == "all")
            {
                foreach(KeyValuePair<string, NameValueCollection> kvps in configDefaults)
                {
                    // Add the section headers
                    results += "["+kvps.Key+"]" + "\n";
                    foreach(string kvp in kvps.Value)
                    {
                        // Add the key=value
                        results += kvp + "= " + configDefaults[kvps.Key][kvp]+"\n";
                    }
                    results += "\n";
                }
                return results;
            }
            else if (Sections.defaultSections.Contains(section.ToLower()))
            {
                foreach(string kvp in configDefaults[section])
                {
                    results += kvp + "= " + configDefaults[section][kvp]+"\n";
                }
                return results;
            }
            return string.Format("Unrecognized argument: {0}", section);
        }

        public static dynamic GetSettings(string section, string key)
        {
            return configDefaults[section][key];
        }
    }

    public static class CustomConfig
    {
        private static IniFile iniFile;

        public static Dictionary<string, Dictionary<string, string>> customConfig = new Dictionary<string, Dictionary<string, string>>();

        public static void LoadConfig(string filename)
        {
            iniFile = new IniFile(filename);
        }

        public static void ParseCustom()
        {
            foreach (string section in Sections.defaultSections)
            {
                customConfig.Add(section, _GetSettings(section));
            }
        }

        private static Dictionary<string, string> _GetSettings(string section)
        {
            return iniFile.GetValue(section);
        }

        public static string GetSettings(string section, string key)
        {
            if (section.ToLower() == "bot" && key.ToLower() == "prompt")
            {
                if (iniFile.GetValue("bot", "prompt") != "default")
                {
                    return iniFile.GetValue("bot", "prompt");
                }

                return DefaultConfig.GetSettings("bot", "prompt");
            }

            return customConfig[section][key];
        }

        public static string GetSettings(string section)
        {
            string results = "";
            if (section.ToLower() == "all")
            {
                foreach (KeyValuePair<string, Dictionary<string, string>> _section in customConfig)
                {
                    results += "[" + _section.Key + "]" + "\n";
                    foreach (string _field in customConfig[_section.Key].Keys)
                    {
                        results += _field + "= " + customConfig[_section.Key][_field]+"\n";
                    }
                    results += "\n";
                }
                return results;
            }
            else if (Sections.defaultSections.Contains(section.ToLower()))
            {
                foreach (KeyValuePair<string, string> kvp in customConfig[section])
                {
                    results += kvp.Key + "= " + kvp.Value+"\n";
                }
                return results;
            }
            return string.Format("Unrecognized argument: {0}", section);
        }
    }
}   
    
