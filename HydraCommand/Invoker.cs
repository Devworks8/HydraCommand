using System;
namespace HydraCommand
{
    // This class tells the Commands to execute their actions.
    class Invoker
    {
        private Command _command;
        public Invoker(Command command)
        {
            this._command = command;
        }

        public void ExecuteCommand() => _command.Execute();
    }
}
