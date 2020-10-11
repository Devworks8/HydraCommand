using System;

namespace HydraCommand
{
    class HCommand
    {
        static void Main(string[] args)
        {
            Receiver r = new Receiver();
            Command c = new ConcreteCommand(r);
            Invoker invoker = new Invoker(c);
            invoker.ExecuteCommand();
        }
    }
}
