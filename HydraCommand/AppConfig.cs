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
using IniParser;
using IniParser.Model;

namespace HydraCommand
{
    public static class Sections
    {
        // Predefine default config sections
        public static List<string> defaultSections =
            new List<string> { "bot", "cfg" , "proxy", "reactor", "messenger"};

        // Predefine user config sections
        public static List<string> customSections =
            new List<string> { "bot", "proxy", "reactor", "messenger" };
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
        private static IniData iniData;
        private static FileIniDataParser iniParser;
        private static string iniFilename;

        public static void LoadConfig(string filename)
        {
            iniFilename = filename;
            iniParser = new FileIniDataParser();
            iniData = iniParser.ReadFile(filename);
        }

        public static string GetSettings(string section, string key)
        {
            if (iniData[section][key] == "default") return DefaultConfig.GetSettings(section, key);
            else return iniData[section][key];
        }

        public static string GetSettings(string section)
        {
            string results = "";
            if (section.ToLower() == "all")
            {
                foreach (SectionData _section in iniData.Sections)
                {
                    results += "[" + _section.SectionName + "]" + "\n";

                    foreach (KeyData _field in _section.Keys)
                    {
                        results += _field.KeyName + "= " + _field.Value + "\n";
                    }
                    results += "\n";
                }
                return results;
            }
            else if (iniData.Sections.ContainsSection(section.ToLower()))
            {
                foreach (KeyData _field in iniData[section])
                {
                    results += _field.KeyName + "= " + _field.Value + "\n";
                }
                return results;
            }
            return string.Format("Unrecognized argument: {0}", section);
        }

        public static bool SetSetting(string section, string key, string value)
        {
            try
            {
                iniData[section][key] = value;
                iniParser.WriteFile(iniFilename, iniData);
                return true;
            }
            catch
            {
                return false;
            }  
        }
    }
}   
    
