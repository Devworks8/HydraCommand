//
//  Command.cs
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
namespace HydraCommand
{
    // The Command executes the action from a Receiver.
    abstract public class Command
    {
        protected Receiver Receiver;
        public Command(Receiver receiver)
        {
            this.Receiver = receiver;
        }

        public abstract void Execute(string[] args);
    }

    public class quit : Command
    {
        public quit(Receiver receiver) : base(receiver) { }
        public override void Execute(string[] args) => Receiver.quit(args);
    }

    
}
