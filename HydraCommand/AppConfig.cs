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
using System.IO;
using System.Configuration;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;


//using YamlDotNet.Serialization;
//using YamlDotNet.RepresentationModel;

namespace HydraCommand
{
    public class Sections
    {
        public static List<string> defaultSections =
            new List<string> { "bot", "cfg" };
    }

    public sealed class DefaultConfig : Sections
    {
        // Hold Default Config Settings
        public static Dictionary<string, NameValueCollection> configDefaults =
            new Dictionary<string, NameValueCollection>();

        public static void ParseDefaults()
        {
            foreach (string section in defaultSections)
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
                        results += kvp + "= " + configDefaults[kvps.Key][kvp];
                    }
                    results += "\n\n";
                }
                return results;
            }
            else if (defaultSections.Contains(section.ToLower()))
            {
                foreach(string kvp in configDefaults[section])
                {
                    results += kvp + "= " + configDefaults[section][kvp];
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

    public class CustomConfig : Sections
    {
        private IniFile iniFile;

        public CustomConfig(string filename)
        {
            iniFile = new IniFile(filename);
        }

        public static Dictionary<string, string> customConfig = new Dictionary<string, string>();

        //TODO: Must populate customConfig dictionary
        public static void ParseCustom()
        {
            foreach (string section in defaultSections)
            {
                customConfig.Add(section, _GetSettings(section));
            }
        }

        private static string _GetSettings(string section)
        {
            //TODO: iterate thru custom config ini
            return "";
        }
        //TODO: This will be unecessary once ParseCustome is done
        public string LoadConfig(string filename)
        {
            string[] all = iniFile.GetAllValues("bot", "prompt");

            foreach (string val in all)
            {
                Console.WriteLine(val);
            }

            if (iniFile.GetValue("bot", "prompt") != "default")
            {
                return iniFile.GetValue("bot", "prompt");
            }

            return DefaultConfig.GetSettings("bot", "prompt");
        }
        //TODO: Complete this
        //public static dynamic GetSettings(string section)
        //{
        //    string results = "";
        //    if (section.ToLower() == "all")
        //    {
        //        foreach (KeyValuePair<string, string> kvps in )
        //        {
        //            results += "[" + kvps.Key + "]" + "\n";
        //            foreach (string kvp in kvps.Value)
        //            {
        //                results += kvp + "= " + configDefaults[kvps.Key][kvp];
        //            }
        //            Console.Write("\n\n");
        //        }
        //        return results;
        //    }
        //    else if (defaultSections.Contains(section.ToLower()))
        //    {
        //        foreach (string kvp in configDefaults[section])
        //        {
        //            results += kvp + "= " + configDefaults[section][kvp];
        //        }
        //        return results;
        //    }
        //    return string.Format("Unrecognized argument: {0}", section);
        //}
    }
}   
    
