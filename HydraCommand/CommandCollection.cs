//
//  CommandCollection.cs
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
using System.Linq;
using System.Collections.Generic;
using ManyConsole;

namespace HydraCommand
{
    public class HelpCommand : ConsoleCommand
    {
        public HelpCommand()
        {
            IsCommand("help", "Display command list.");
        }

        public override int Run(string[] remainingArguments)
        {
            // locate any commands in the assembly (or use an IoC container, or whatever source)
            //var commands = GetCommands();
            //ManyConsole.Internal.ConsoleHelp.ShowSummaryOfCommands(commands, );
            return 0;
        }
    }



    public class ConfigCommand : ConsoleCommand
    {
        public string CLevel { get; set; }
        public string Operation { get; set; }
        public string Service { get; set; }
        public string Field { get; set; }

        // config default get [-s bot] [-f prompt]

        public ConfigCommand()
        {
            // Register the actual command with a simple (optional) description.
            IsCommand("config", "Access configuration.");
            HasAlias("config");
            HasLongDescription(@"
Get or Set config settings.
Config Level: The config you wish to access. [default|user]
   Operation: Read or write into a field in the config. [get|set]
              You can only read from the default config.
     Service: The name of the service to access.
       Field: The name of the field to access.");

            HasOption("s|service=", "The service name.", s => Service = s);
            HasOption("f|field=", "The field name.", f => Field = f);
            HasAdditionalArguments(2, "<Config Level> <Operation>");
        }

        public override int? OverrideAfterHandlingArgumentsBeforeRun(string[] remainingArguments)
        {
            CLevel = remainingArguments[0];
            Operation = remainingArguments[1];

            return base.OverrideAfterHandlingArgumentsBeforeRun(remainingArguments);
        }

        public override int Run(string[] remainingArguments)
        {
            if (CLevel.ToLower() == "default")
            {
                if (Operation.ToLower() == "get")
                {
                    if (String.IsNullOrEmpty(Service))
                    {

                    }
                    else
                    {
                        if (String.IsNullOrEmpty(Field))
                        {
                            
                        }
                        else
                        {
                            Console.WriteLine("Results:\n");
                            Console.WriteLine(DefaultConfig.GetSettings(Service, Field));
                        }
                    }
                    
                }
            }
            else
            {

            }
            return 0;
        }
    }
}