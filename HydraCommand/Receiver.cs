//
//  Receiver.cs
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
    /// <summary>
    /// The class <c>Receiver</c> is the object that has an action
    /// to be executed from the <c>Command</c> class.
    /// </summary>
    public class Receiver
    {
        public static IEnumerable<ConsoleCommand> GetCommands()
        {
            return ConsoleCommandDispatcher.FindCommandsInSameAssemblyAs(typeof(Receiver));
        }

        public void quit(string[] args)
        {
            var commands = GetCommands();
            ConsoleCommandDispatcher.DispatchCommand(commands, args, Console.Out);
        }

        public void help(string[] args)
        {
            var commands = GetCommands();

            if (args.Length > 1)
            {
                foreach (ConsoleCommand cmd in commands)
                {
                    if (!(cmd.Aliases is null) && cmd.Aliases[0] == args[1])
                    {
                        ManyConsole.Internal.ConsoleHelp.ShowCommandHelp(cmd, Console.Out);
                        break;
                    }
                    else
                    {
                        ManyConsole.Internal.ConsoleHelp.ShowSummaryOfCommands(commands, Console.Out);
                        break;
                    }
                }
            }
            else
            {
                ManyConsole.Internal.ConsoleHelp.ShowSummaryOfCommands(commands, Console.Out);
            }
        }

        public void config(string[] args)
        {
            var commands = GetCommands();
            ConsoleCommandDispatcher.DispatchCommand(commands, args, Console.Out);
            

        }

    }
}
