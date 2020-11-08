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
            return 0;
        }
    }

    public class QuitCommand : ConsoleCommand
    {
        public QuitCommand()
        {
            IsCommand("quit", "Quit the program.");
        }

        public override int Run(string[] remainingArguments)
        {
            Environment.Exit(0);
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
            AllowsAnyAdditionalArguments("<arg>");
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
                        Console.WriteLine("Showing results for -> default:all\n\n{0}", DefaultConfig.GetSettings("all"));
                    }
                    else
                    {
                        if (String.IsNullOrEmpty(Field))
                        {
                            Console.WriteLine("Showing results for -> defaults:{0}\n\n{1}", Service, DefaultConfig.GetSettings(Service));
                        }
                        else
                        {
                            Console.WriteLine("Showing results for -> default:{0}= {1}", Field, DefaultConfig.GetSettings(Service, Field));
                        }
                    }   
                }
                else if (Operation.ToLower() == "set")
                {
                    Helper.DisplayError("Unable to set values in the default config");
                }
            }
            //TODO: Need to process Set operation.
            else if (CLevel.ToLower() == "user")
            {
                if (Operation.ToLower() == "get")
                {
                    if (String.IsNullOrEmpty(Service))
                    {
                        Console.WriteLine("Showing results for -> user:all\n\n{0}",  CustomConfig.GetSettings("all"));
                    }
                    else
                    {
                        if (String.IsNullOrEmpty(Field))
                        {
                            Console.WriteLine("Showing results for -> user:{0}\n\n{1}", Service, CustomConfig.GetSettings(Service));
                        }
                        else
                        {
                            Console.WriteLine("Showing results for -> user:{0}= {1}", Field, CustomConfig.GetSettings(Service, Field));
                        }
                    }
                }
                else if (Operation.ToLower() == "set")
                {
                    if (string.IsNullOrEmpty(Service))
                    {
                        Helper.DisplayError("Service name required.");
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(Field))
                        {
                            Helper.DisplayError("Field Name required.");
                        }
                        else
                        {
                            if (!CustomConfig.SetSetting(Service, Field, remainingArguments[2]))
                            {
                                Helper.DisplayError("Failed to make changes.");
                            }
                            Console.WriteLine("{0}:{1} has been updated.", Service, Field);
                        }
                    }
                }
            }
            return 0;
        }
    }

    public class NetworkCommand : ConsoleCommand
    {
        public string service { get; set; }
        public string command { get; set; }

        public NetworkCommand()
        {
            IsCommand("network", "Access network commands");
            HasAdditionalArguments(2, " <Service> <Command>");
        }

        public override int? OverrideAfterHandlingArgumentsBeforeRun(string[] remainingArguments)
        {
            service = remainingArguments[0];
            command = remainingArguments[1];

            return base.OverrideAfterHandlingArgumentsBeforeRun(remainingArguments);
        }

        public override int Run(string[] remainingArguments)
        {
            if (service == "all")
            {
                if (command == "start")
                {
                    HydraBot.proxy.Start();
                    HydraBot.reactor.Start();
                    HydraBot.messenger.Start();
                }
                else if (command == "stop")
                {
                    HydraBot.proxy.Stop();
                    HydraBot.reactor.Stop();
                    HydraBot.messenger.Stop();
                }
                else if (command == "restart")
                {
                    HydraBot.proxy.Restart();
                    HydraBot.reactor.Restart();
                    HydraBot.messenger.Restart();
                }
                else if (command == "status")
                {
                    HydraBot.proxy.Status();
                    HydraBot.reactor.Status();
                    HydraBot.messenger.Status();
                }
                else
                {
                    Helper.DisplayError("Invalid command");
                    return 1;
                }
            }
            else if (service == "proxy")
            {
                if (command == "start") HydraBot.proxy.Start();
                else if (command == "stop") HydraBot.proxy.Stop();
                else if (command == "restart") HydraBot.proxy.Restart();
                else if (command == "status") HydraBot.proxy.Status();
                else
                {
                    Helper.DisplayError("Invalid command");
                    return 1;
                }
            }
            else if (service == "reactor")
            {
                if (command == "start") HydraBot.reactor.Start();
                else if (command == "stop") HydraBot.reactor.Stop();
                else if (command == "restart") HydraBot.reactor.Restart();
                else if (command == "status") HydraBot.reactor.Status();
                else
                {
                    Helper.DisplayError("Invalid command");
                    return 1;
                }
            }
            else if (service == "messenger")
            {
                if (command == "start") HydraBot.messenger.Start();
                else if (command == "stop") HydraBot.messenger.Stop();
                else if (command == "restart") HydraBot.messenger.Restart();
                else if (command == "status") HydraBot.messenger.Status();
                else
                {
                    Helper.DisplayError("Invalid command");
                    return 1;
                }
            }
            return 0;
        }
    }
}