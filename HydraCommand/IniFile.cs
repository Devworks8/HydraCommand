﻿//
//  IniFile.cs
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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;

namespace HydraCommand
{
    /// <summary>
    /// A class for reading values by section and key from a standard ".ini" initialization file.
    /// </summary>
    /// <remarks>
    /// Section and key names are not case-sensitive. Values are loaded into a hash table for fast access.
    /// Use <see cref="GetAllValues"/> to read multiple values that share the same section and key.
    /// Sections in the initialization file must have the following form:
    /// <code>
    ///     ; comment line
    ///     [section]
    ///     key=value
    /// </code>
    /// </remarks>
    public class IniFile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IniFile"/> class.
        /// </summary>
        /// <param name="file">The initialization file path.</param>
        /// <param name="commentDelimiter">The comment delimiter string (default value is ";").
        /// </param>
        public IniFile(string file, string commentDelimiter = ";")
        {
            CommentDelimiter = commentDelimiter;
            TheFile = file;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IniFile"/> class.
        /// </summary>
        public IniFile()
        {
            CommentDelimiter = ";";
        }

        /// <summary>
        /// The comment delimiter string (default value is ";").
        /// </summary>
        public string CommentDelimiter { get; set; }

        private string theFile = null;

        /// <summary>
        /// The initialization file path.
        /// </summary>
        public string TheFile
        {
            get
            {
                return theFile;
            }
            set
            {
                theFile = null;
                dictionary.Clear();
                if (File.Exists(value))
                {
                    theFile = value;
                    using (StreamReader sr = new StreamReader(theFile))
                    {
                        string line, section = "";
                        while ((line = sr.ReadLine()) != null)
                        {
                            line = line.Trim();
                            if (line.Length == 0) continue;  // empty line
                            if (!String.IsNullOrEmpty(CommentDelimiter) && line.StartsWith(CommentDelimiter))
                                continue;  // comment

                            if (line.StartsWith("[") && line.Contains("]"))  // [section]
                            {
                                int index = line.IndexOf(']');
                                section = line.Substring(1, index - 1).Trim();
                                continue;
                            }

                            if (line.Contains("="))  // key=value
                            {
                                int index = line.IndexOf('=');
                                string key = line.Substring(0, index).Trim();
                                string val = line.Substring(index + 1).Trim();
                                string key2 = String.Format("[{0}]{1}", section, key).ToLower();

                                if (val.StartsWith("\"") && val.EndsWith("\""))  // strip quotes
                                    val = val.Substring(1, val.Length - 2);

                                if (dictionary.ContainsKey(key2))  // multiple values can share the same key
                                {
                                    index = 1;
                                    string key3;
                                    while (true)
                                    {
                                        key3 = String.Format("{0}~{1}", key2, ++index);
                                        if (!dictionary.ContainsKey(key3))
                                        {
                                            dictionary.Add(key3, val);
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    dictionary.Add(key2, val);
                                }
                            }
                        }
                    }
                }
            }
        }

        // "[section]key"   -> "value1"
        // "[section]key~2" -> "value2"
        // "[section]key~3" -> "value3"
        private Dictionary<string, string> dictionary = new Dictionary<string, string>();

        private bool TryGetValue(string section, string key, out string value)
        {
            string key2;
            if (section.StartsWith("["))
                key2 = String.Format("{0}{1}", section, key);
            else
                key2 = String.Format("[{0}]{1}", section, key);

            return dictionary.TryGetValue(key2.ToLower(), out value);
        }

        /// <summary>
        /// Get a keys and values of a section.
        /// </summary>
        /// <param name="section">The section.</param>
        /// <returns>The dictionary of keys and values by section</returns>
        public Dictionary<string, string> GetValue(string section)
        {
            Dictionary<string, string> value = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> kvp in dictionary)
            {
                string _section = "[" + section + "]";
                if (kvp.Key.Contains(_section))
                {
                    value.Add(kvp.Key.Substring(_section.Length), kvp.Value);
                }
            }
            return value;
        }

        /// <summary>
        /// Gets a string value by section and key.
        /// </summary>
        /// <param name="section">The section.</param>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The value.</returns>
        /// <seealso cref="GetAllValues"/>
        public string GetValue(string section, string key, string defaultValue = "")
        {
            string value;
            if (!TryGetValue(section, key, out value))
                return defaultValue;

            return value;
        }

        /// <summary>
        /// Gets a string value by section and key.
        /// </summary>
        /// <param name="section">The section.</param>
        /// <param name="key">The key.</param>
        /// <returns>The value.</returns>
        /// <seealso cref="GetValue"/>
        public string this[string section, string key]
        {
            get
            {
                return GetValue(section, key);
            }
        }

        /// <summary>
        /// Gets an integer value by section and key.
        /// </summary>
        /// <param name="section">The section.</param>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="minValue">Optional minimum value to be enforced.</param>
        /// <param name="maxValue">Optional maximum value to be enforced.</param>
        /// <returns>The value.</returns>
        public int GetInteger(string section, string key, int defaultValue = 0,
            int minValue = int.MinValue, int maxValue = int.MaxValue)
        {
            string stringValue;
            if (!TryGetValue(section, key, out stringValue))
                return defaultValue;

            int value;
            if (!int.TryParse(stringValue, out value))
            {
                double dvalue;
                if (!double.TryParse(stringValue, out dvalue))
                    return defaultValue;
                value = (int)dvalue;
            }

            if (value < minValue)
                value = minValue;
            if (value > maxValue)
                value = maxValue;
            return value;
        }

        /// <summary>
        /// Gets a double floating-point value by section and key.
        /// </summary>
        /// <param name="section">The section.</param>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="minValue">Optional minimum value to be enforced.</param>
        /// <param name="maxValue">Optional maximum value to be enforced.</param>
        /// <returns>The value.</returns>
        public double GetDouble(string section, string key, double defaultValue = 0,
            double minValue = double.MinValue, double maxValue = double.MaxValue)
        {
            string stringValue;
            if (!TryGetValue(section, key, out stringValue))
                return defaultValue;

            double value;
            if (!double.TryParse(stringValue, out value))
                return defaultValue;

            if (value < minValue)
                value = minValue;
            if (value > maxValue)
                value = maxValue;
            return value;
        }

        /// <summary>
        /// Gets a boolean value by section and key.
        /// </summary>
        /// <param name="section">The section.</param>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The value.</returns>
        public bool GetBoolean(string section, string key, bool defaultValue = false)
        {
            string stringValue;
            if (!TryGetValue(section, key, out stringValue))
                return defaultValue;

            return (stringValue != "0" && !stringValue.StartsWith("f", true, null));
        }

        /// <summary>
        /// Gets an array of string values by section and key.
        /// </summary>
        /// <param name="section">The section.</param>
        /// <param name="key">The key.</param>
        /// <returns>The array of values, or null if none found.</returns>
        /// <seealso cref="GetValue"/>
        public string[] GetAllValues(string section, string key)
        {
            string key2, key3, value;
            if (section.StartsWith("["))
                key2 = String.Format("{0}{1}", section, key).ToLower();
            else
                key2 = String.Format("[{0}]{1}", section, key).ToLower();

            if (!dictionary.TryGetValue(key2, out value))
                return null;

            List<string> values = new List<string>();
            values.Add(value);
            int index = 1;
            while (true)
            {
                key3 = String.Format("{0}~{1}", key2, ++index);
                if (!dictionary.TryGetValue(key3, out value))
                    break;
                values.Add(value);
            }

            return values.ToArray();
        }
    }
}