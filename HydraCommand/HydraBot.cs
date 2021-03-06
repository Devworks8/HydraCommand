﻿//
//  HydroBot.cs
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

namespace HydraCommand
{
    public static class HydraBot
    {
        static Receiver r = new Receiver();
        static Invoker invoker = new Invoker();


        // List of valid commands to be created dynamically
        public static List<string> commandOptions =
            new List<string> { "quit", "config", "help" };

        // Hold the valid command objects in a Dictionary
        public static Dictionary<string, Command> validCommands =
            new Dictionary<string, Command>();

        // Dynamically create a Command object className with the Receiver object argument
        public static Command CreateInstance(string className, Receiver r)
        {
            Type t = Type.GetType("HydraCommand." + className);

            Command o = (Command) Activator.CreateInstance(t, new Object[] { r });
            return o;
        }

        private static void GetUserInput(string prompt)
        {
            Console.Write(prompt);

            // Capture the cursor position just after the prompt
            var inputCursorLeft = Console.CursorLeft;
            var inputCursorTop = Console.CursorTop;

            // Now get user input
            string input = Console.ReadLine();

            string[] args = input.Split(' ');

            while (!String.IsNullOrEmpty(input))
            {
                if (commandOptions.Contains(args[0]))
                {
                    // Erase the last error message (if there was one)
                    Console.Write(new string(' ', Console.WindowWidth));
                    invoker.SetCommand(validCommands[args[0].ToLower()], args);
                    invoker.ExecuteCommand();
                    break;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    // PadRight ensures that this line extends the width
                    // of the console window so it erases itself each time
                    Console.Write($"Error! '{input}' is not a valid response".PadRight(Console.WindowWidth));
                    Console.ResetColor();

                    // Set cursor position to just after the promt again, write
                    // a blank line, and reset the cursor one more time
                    Console.SetCursorPosition(inputCursorLeft, inputCursorTop);
                    Console.Write(new string(' ', input.Length));
                    Console.SetCursorPosition(inputCursorLeft, inputCursorTop);

                    input = Console.ReadLine();
                    args = input.Split(':', StringSplitOptions.RemoveEmptyEntries);
                } 
            }

            // Erase the last error message (if there was one)
            Console.Write(new string(' ', Console.WindowWidth));

        }

        public static void run()
        {
            DefaultConfig.ParseDefaults();
            CustomConfig.LoadConfig(DefaultConfig.GetSettings("cfg", "path"));
            //CustomConfig.ParseCustom();

            // Create command objects and assign to dictionary
            foreach (string command in commandOptions)
            {
                validCommands.Add(command, CreateInstance(command, r));
            }
            
            while (true)
            {
                GetUserInput(CustomConfig.GetSettings("bot", "prompt"));
            }
        }
    }
}
