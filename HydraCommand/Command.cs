using System;
namespace HydraCommand
{
    // The Command executes the action from a Receiver.
    abstract class Command
    {
        protected Receiver Receiver;
        public Command(Receiver receiver)
        {
            this.Receiver = receiver;
        }

        public abstract void Execute();
    }

    class ConcreteCommand : Command
    {
        public ConcreteCommand(Receiver receiver) : base(receiver) { }
        public override void Execute() => Receiver.Action();
    }
}
