//
//  HydroBot.cs
//
//  Author:
//       souljourner <lachapellec@gmail.com>
//
//  Copyright (c) 2020 ${CopyrightHolder}
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
    public class HydraBot
    {
        Receiver r = new Receiver();
        Invoker invoker = new Invoker();

        // List of valid commands to be created dynamically
        public List<string> commandOptions =
            new List<string> { "quit" };

        // Hold the valid command objects in a Dictionary
        public Dictionary<string, Command> validCommands =
            new Dictionary<string, Command>();

        // Dynamically create a Command object className with the Receiver object argument
        public Command CreateInstance(string className, Receiver r)
        {
            Type t = Type.GetType("HydraCommand." + className);

            Command o = (Command) Activator.CreateInstance(t, new Object[] { r });
            return o;
        }

    public void run()
        {
            // Create command objects and assign to dictionary
            foreach (string command in commandOptions)
            {
                validCommands.Add(command, CreateInstance(command, r));
            }

            string input = "";

            while (true)
            {
                Console.Write("> ");
                input = Console.ReadLine();
                string[] command = input.Split(':', StringSplitOptions.RemoveEmptyEntries);
                    
                invoker.SetCommand(validCommands[command[0].ToLower()]);
                invoker.ExecuteCommand();
            }
        }
    }
}
