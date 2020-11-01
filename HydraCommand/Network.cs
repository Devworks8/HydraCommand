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
//using ZeroMQ;
using NetMQ.Sockets;
using MessageRouter.NetMQ;
using HydraCommand;

namespace HydraNetwork
{
    /// <summary>
    /// Receives requests from users and sends to Reactor
    /// </summary>
    public static class Proxy
    {
        private static bool isRunning { get; set; } = false;
        private static bool isRestarting { get; set; } = false;
        private static DealerSocket frontend;
        private static RouterSocket backend;
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
            
        public static void Status()
        {
            if (isRunning) Console.WriteLine("The proxy is: Running");
            else Console.WriteLine("The proxy is: Stopped");
        }

        public static void Start()
        {
            if (!isRunning)
            {
                backend = new RouterSocket(backendAddress);
                frontend = new DealerSocket();
                frontend.Connect(frontendAddress);

                if (!isRestarting)
                {
                    Console.WriteLine("Proxy started");
                    isRunning = true;
                }
            }
            else Helper.DisplayError("Proxy is already running...");
        }

        public static void Stop()
        {
            if (isRunning)
            {
                backend.Unbind(backendAddress);
                backend.Dispose();
                frontend.Disconnect(frontendAddress);
                frontend.Dispose();
                
                if (!isRestarting) Console.WriteLine("Proxy has been terminated");
                isRunning = false;
            }
            else Helper.DisplayError("Proxy is not running...");
        }

        public static void Restart()
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
    public static class Messenger
    {
        private static bool isRunning { get; set; } = false;
        private static bool isRestarting { get; set; } = false;
        private static DealerSocket reactor;
        private static RouterSocket service;
        private static string serviceAddress
        {
            get
            {
                return "tcp://" + CustomConfig.GetSettings("messenger", "service")
                    + ":" + CustomConfig.GetSettings("messenger", "service_port");
            }
        }

        private static string reactorAddress
        {
            get
            {
                return "tcp://" + CustomConfig.GetSettings("messenger", "reactor")
                    + ":" + CustomConfig.GetSettings("messenger", "reactor_port");
            }
        }

        public static void Status()
        {
            if (isRunning) Console.WriteLine("The messenger is: Running");
            else Console.WriteLine("The messenger is: Stopped");
        }

        public static void Start()
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

        public static void Stop()
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

        public static void Restart()
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
    public static class Reactor
    {
        private static bool isRunning { get; set; } = false;
        private static bool isRestarting { get; set; } = false;
        private static DealerSocket proxy;
        private static RouterSocket messenger;
        private static string proxyAddress
        {
            get
            {
                return "tcp://" + CustomConfig.GetSettings("reactor", "proxy")
                    + ":" + CustomConfig.GetSettings("reactor", "proxy_port");
            }
        }

        private static string messengerAddress
        {
            get
            {
                return "tcp://" + CustomConfig.GetSettings("reactor", "messenger")
                    + ":" + CustomConfig.GetSettings("reactor", "messenger_port");
            }
        }

        public static void Status()
        {
            if (isRunning) Console.WriteLine("The reactor is: Running");
            else Console.WriteLine("The reactor is: Stopped");
        }

        public static void Start()
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

        public static void Stop()
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

        public static void Restart()
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
