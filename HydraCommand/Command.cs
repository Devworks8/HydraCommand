//
//  Command.cs
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

namespace HydraCommand
{
    /// <summary>
    /// The class <c>Command</c> executes the action from a <c>Receiver</c> class.
    /// </summary>
    abstract public class Command
    {
        protected Receiver Receiver;
        public Command(Receiver receiver)
        {
            this.Receiver = receiver;
        }

        public abstract void Execute(string[] args);
    }

    /// <summary>
    /// The following classes constitute the commands.
    /// <list type="bullet">
    ///<item>
    ///<description>
    ///<c>quit</c> => Exit the application
    ///</description>
    ///</item>
    /// </list>
    /// </summary>
    public class quit : Command
    {
        public quit(Receiver receiver) : base(receiver) { }
        public override void Execute(string[] args) => Receiver.quit(args);
    }

    public class help : Command
    {
        public help(Receiver receiver) : base(receiver) { }
        public override void Execute(string[] args) => Receiver.help(args);
    }

    public class config : Command
    {
        public config(Receiver receiver) : base(receiver) { }
        public override void Execute(string[] args) => Receiver.config(args);
    }

    public class network : Command
    {
        public network(Receiver receiver) : base(receiver) { }
        public override void Execute(string[] args) => Receiver.network(args);
    }

}
