using System;
namespace HydraCommand
{
    // The Receiver is the object that has an action to be executed from the Command.
    public class Receiver
    {
        public void Action() => Console.WriteLine("Called Receiver.Action()");
    }
}
