﻿//
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
        private static DefaultConfig _instance = null;
        private static readonly object _lock = new object();

        DefaultConfig() { }

        public static DefaultConfig SingleInstance
        {
            get
            {
                /*
                 * The lock will allow only one thread at a time to access 
                 * the block of code inside it.
                 * If there is already a thread accessing the block of code
                 * inside the lock, the other threads will hold at this point
                 * until the first thread finishes her business inside that block.
                 */
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new DefaultConfig();
                    }
                    return _instance;
                }
            }
        }

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

        public static dynamic GetSettings(string section, string key)
        {
            return configDefaults[section][key];
        }
    }

    public sealed class CustomConfig : Sections
    {
        private static CustomConfig _instance = null;
        private static readonly object _lock = new object();
        
        CustomConfig() { }

        public static CustomConfig SingleInstance
        {
            get
            {
                /*
                 * The lock will allow only one thread at a time to access 
                 * the block of code inside it.
                 * If there is already a thread accessing the block of code
                 * inside the lock, the other threads will hold at this point
                 * until the first thread finishes her business inside that block.
                 */
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new CustomConfig();
                    }
                    return _instance;
                }
            }
        }

        public static Dictionary<string, string> customConfig = new Dictionary<string, string>();

        public string LoadConfig(string filename)
        {
            string fullPath = @"/Users/souljourner/Dropbox/Coding/C#/HydraCommand/HydraCommand/" + filename;
            var iniFile = new IniFile(fullPath);
            string[] all = iniFile.GetAllValues("bot", "prompt");
            Console.WriteLine(all);
            if (iniFile.GetValue("bot", "prompt") != "default")
            {
                string prompt;
                return prompt = iniFile.GetValue("bot", "prompt");
            }

            return DefaultConfig.GetSettings("bot", "prompt");

        }
    }

    public static class CommandTree
    {
        private static Dictionary<string, Dictionary<string, Delegate>> subCommand =
            new Dictionary<string, Dictionary<string, Delegate>>();

        private static Dictionary<string, Delegate> subSubCommand =
            new Dictionary<string, Delegate>();


        private static List<string> validSubCommands = new List<string> { "set", "get" };
        private static List<string> validSubSubCommands = new List<string> { "defaults" };

        //// store
        //var dico = new Dictionary<int, Delegate>();
        //dico[1] = new Func<int, int, int>(Func1);
        //dico[2] = new Func<int, int, int, int>(Func2);

        //// and later invoke
        //var res = dico[1].DynamicInvoke(1, 2);
        //Console.WriteLine(res);
        //var res2 = dico[2].DynamicInvoke(1, 2, 3);
        //Console.WriteLine(res2);

        static CommandTree()
        {
            // Add methods to associated subSubCommand
            subSubCommand["set"] = new Func<string, string, string>(Set);
            subSubCommand["get"] = new Func<string, string, string>(Get);

            // Add subSubCommands to associated subCommand
            subCommand["default"] = subSubCommand;
            subCommand["user"] = subSubCommand;
        }

        public static void ParseCommand(string[] args)
        {
            
        }

        private static string Get(string cLevel, string arg)
        {
            if (cLevel == "default")
            {



                DefaultConfig.GetSettings("bot", "prompt");
                return "";
            }
            else
            {
                return "";
            }
        }

        private static string Set(string cLevel, string arg)
        {
            if (cLevel == "default")
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR: Unable to set default settings.");
                Console.ResetColor();
                return "";
            }
            else
            {
                return "";
            }
        }
    }
}   
    
