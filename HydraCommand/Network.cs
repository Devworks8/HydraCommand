//
//  Network.cs
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
using System.Threading.Tasks;
using NetMQ.Sockets;
using MessageRouter.NetMQ;
using HydraCommand;

namespace HydraCommand.Network
{
    /// <summary>
    /// Receives requests from users and sends to Reactor
    /// </summary>
    public class Proxy
    {
        private bool isRunning { get; set; } = false;
        private bool isRestarting { get; set; } = false;
        private Task task;
        private static string frontendAddress
        {
            get
            {
                return "tcp://" + CustomConfig.GetSettings("proxy", "frontend")
                    + ":" + CustomConfig.GetSettings("proxy", "frontend_port");
            }
        }    
        private static string backendAddress
        {
            get
            {
                return "tcp://" + CustomConfig.GetSettings("proxy", "backend")
                    + ":" + CustomConfig.GetSettings("proxy", "backend_port");
            }
        }
        public static XSubscriberSocket frontend = new XSubscriberSocket(frontendAddress);
        private static XPublisherSocket backend = new XPublisherSocket(backendAddress);
        private NetMQ.Proxy proxy = new NetMQ.Proxy(frontend, backend);

        public void Status()
        {
            if (isRunning) Console.WriteLine("The proxy is: Running");
            else Console.WriteLine("The proxy is: Stopped");
        }

        public void Start()
        {
            if (!isRunning)
            {
                //TODO: Need to keep working on the async implementation
                Task.Run (() => proxy.Start(), TaskCreationOptions.LongRunning);

                if (!isRestarting)
                {
                    Console.WriteLine("Proxy started");
                    isRunning = true;
                }
            }
            else Helper.DisplayError("Proxy is already running...");

        }

        public void Stop()
        {
            if (isRunning)
            {
                proxy.Stop();

                if (!isRestarting) Console.WriteLine("Proxy has been terminated");
                isRunning = false;
            }
            else Helper.DisplayError("Proxy is not running...");
        }

        public void Restart()
        {
            if (isRunning)
            {
                isRestarting = true;
                Stop();
                Start();
                Console.WriteLine("Proxy restarted");
                isRestarting = false;
                isRunning = true;
            }
            else Helper.DisplayError("Proxy is not running...");
        }
    }

    /// <summary>
    /// Receives requests from Reactor and sends to Nodes
    /// </summary>
    public class Messenger
    {
        private bool isRunning { get; set; } = false;
        private bool isRestarting { get; set; } = false;
        private DealerSocket reactor;
        private RouterSocket service;
        private string serviceAddress
        {
            get
            {
                return "tcp://" + CustomConfig.GetSettings("messenger", "service")
                    + ":" + CustomConfig.GetSettings("messenger", "service_port");
            }
        }

        private string reactorAddress
        {
            get
            {
                return "tcp://" + CustomConfig.GetSettings("messenger", "reactor")
                    + ":" + CustomConfig.GetSettings("messenger", "reactor_port");
            }
        }

        public void Status()
        {
            if (isRunning) Console.WriteLine("The messenger is: Running");
            else Console.WriteLine("The messenger is: Stopped");
        }

        public void Start()
        {
            if (!isRunning)
            {
                service = new RouterSocket(serviceAddress);
                reactor = new DealerSocket();
                reactor.Connect(reactorAddress);
                isRunning = true;
                if (!isRestarting) Console.WriteLine("Messenger started");
            }
            else Helper.DisplayError("Messenger is already running...");
        }

        public void Stop()
        {
            if (isRunning)
            {
                service.Unbind(serviceAddress);
                service.Dispose();
                reactor.Disconnect(reactorAddress);
                reactor.Dispose();
                if (!isRestarting) Console.WriteLine("Messenger has been terminated");
                isRunning = false;
            }
            else Helper.DisplayError("Messenger is not running...");
        }

        public void Restart()
        {
            if (isRunning)
            {
                isRestarting = true;
                Stop();
                Start();
                Console.WriteLine("Messenger restarted");
                isRestarting = false;
                isRunning = true;
            }
            else Helper.DisplayError("Messenger is not running...");
        }

    }

    /// <summary>
    /// Receives requests from proxy and sends to Messenger
    /// </summary>
    public class Reactor
    {
        private bool isRunning { get; set; } = false;
        private bool isRestarting { get; set; } = false;
        private DealerSocket proxy;
        private RouterSocket messenger;
        private string proxyAddress
        {
            get
            {
                return "tcp://" + CustomConfig.GetSettings("reactor", "proxy")
                    + ":" + CustomConfig.GetSettings("reactor", "proxy_port");
            }
        }

        private string messengerAddress
        {
            get
            {
                return "tcp://" + CustomConfig.GetSettings("reactor", "messenger")
                    + ":" + CustomConfig.GetSettings("reactor", "messenger_port");
            }
        }

        public void Status()
        {
            if (isRunning) Console.WriteLine("The reactor is: Running");
            else Console.WriteLine("The reactor is: Stopped");
        }

        public void Start()
        {
            if (!isRunning)
            {
                proxy = new DealerSocket(proxyAddress);
                messenger = new RouterSocket();
                messenger.Connect(messengerAddress);
                isRunning = true;
                if (!isRestarting) Console.WriteLine("Reactor started");
            }
            else Helper.DisplayError("Reactor is already running...");
        }

        public void Stop()
        {
            if (isRunning)
            {
                proxy.Unbind(proxyAddress);
                proxy.Dispose();
                messenger.Disconnect(messengerAddress);
                messenger.Dispose();
                if (!isRestarting) Console.WriteLine("Reactor has been terminated");
                isRunning = false;
            }
            else Helper.DisplayError("Reactor is not running...");
        }

        public void Restart()
        {
            if (isRunning)
            {
                isRestarting = true;
                Stop();
                Start();
                Console.WriteLine("Reactor restarted");
                isRestarting = false;
                isRunning = true;
            }
            else Helper.DisplayError("Reactor is not running...");
        }

    }



    /*
    /// <summary>
    /// Service Node
    /// </summary>
    public class Node
    {
        private bool isRunning { get; set; } = false;
        private bool isRestarting { get; set; } = false;
        private static ZContext node_ctx { get; set; }
        private static ZContext messenger_ctx { get; set; }
        private ZSocket node { get; set; }
        private ZSocket messenger { get; set; }

        public void Status()
        {
            if (isRunning) Console.WriteLine("The reactor is: Running");
            else Console.WriteLine("The reactor is: Stopped");
        }

        public void Start()
        {
            if (!isRunning)
            {
                node_ctx = new ZContext();
                messenger_ctx = new ZContext();
                node = new ZSocket(node_ctx, ZSocketType.DEALER);
                messenger = new ZSocket(messenger_ctx, ZSocketType.ROUTER);
                // Bind both sockets to TCP ports
                node.Bind("tcp://" + CustomConfig.GetSettings("reactor", "proxy")
                    + ":" + CustomConfig.GetSettings("reactor", "proxy_port"));
                messenger.Bind("tcp://" + CustomConfig.GetSettings("reactor", "messenger")
                    + ":" + CustomConfig.GetSettings("reactor", "messenger_port"));
                if (!isRestarting) Console.WriteLine("Reactor started");
            }
            else Helper.DisplayError("Reactor is already running...");
        }

        public void Stop()
        {
            if (isRunning)
            {
                node.Close();
                node_ctx.Terminate();
                messenger.Close();
                messenger_ctx.Terminate();
                if (!isRestarting) Console.WriteLine("Reactor has been terminated");
                isRunning = false;
            }
            else Helper.DisplayError("Reactor is not running...");
        }

        public void Restart()
        {
            if (isRunning)
            {
                isRestarting = true;
                Stop();
                Start();
                Console.WriteLine("Reactor restarted");
                isRestarting = false;
            }
            else Helper.DisplayError("Reactor is not running...");
        }

    }
    */
}
