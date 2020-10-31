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
using ZeroMQ;
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
        private static ZContext backend_ctx;
        private static ZContext frontend_ctx;
        private static ZSocket backend;
        private static ZSocket frontend;

        static Proxy() { }

        public static void Status()
        {
            if (isRunning) Console.WriteLine("The proxy is: Running");
            else Console.WriteLine("The proxy is: Stopped");
        }

        public static void Start()
        {
            if (!isRunning)
            {
                backend_ctx = new ZContext();
                frontend_ctx = new ZContext();
                backend = new ZSocket(backend_ctx, ZSocketType.ROUTER);
                frontend = new ZSocket(frontend_ctx, ZSocketType.DEALER);
                // Bind both sockets to TCP ports
                frontend.Bind("tcp://" + CustomConfig.GetSettings("proxy", "frontend")
                    + ":" + CustomConfig.GetSettings("proxy", "frontend_port"));
                backend.Bind("tcp://" + CustomConfig.GetSettings("proxy", "backend")
                    + ":" + CustomConfig.GetSettings("proxy", "backend_port"));
                if (!isRestarting) Console.WriteLine("Proxy started");
            }
            else Helper.DisplayError("Proxy is already running...");
        }

        public static void Stop()
        {
            if (isRunning)
            {
                backend.Close();
                backend_ctx.Terminate();
                frontend.Close();
                frontend_ctx.Terminate();
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
        private static ZContext reactor_ctx { get; set; }
        private static ZContext node_ctx { get; set; }
        private static ZSocket reactor { get; set; }
        private static ZSocket node { get; set; }

        public static void Status()
        {
            if (isRunning) Console.WriteLine("The messenger is: Running");
            else Console.WriteLine("The messenger is: Stopped");
        }

        public static void Start()
        {
            if (!isRunning)
            {
                reactor_ctx = new ZContext();
                node_ctx = new ZContext();
                node = new ZSocket(node_ctx, ZSocketType.ROUTER);
                reactor = new ZSocket(reactor_ctx, ZSocketType.DEALER);
                // Bind both sockets to TCP ports
                node.Bind("tcp://" + CustomConfig.GetSettings("messenger", "node")
                    + ":" + CustomConfig.GetSettings("messenger", "node_port"));
                reactor.Bind("tcp://" + CustomConfig.GetSettings("messenger", "reactor")
                    + ":" + CustomConfig.GetSettings("messenger", "reactor_port"));
                if (!isRestarting) Console.WriteLine("Messenger started");
            }
            else Helper.DisplayError("Messenger is already running...");
        }

        public static void Stop()
        {
            if (isRunning)
            {
                node.Close();
                node_ctx.Terminate();
                reactor.Close();
                reactor_ctx.Terminate();
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
        private static ZContext proxy_ctx { get; set; }
        private static ZContext messenger_ctx { get; set; }
        private static ZSocket proxy { get; set; }
        private static ZSocket messenger { get; set; }

        public static void Status()
        {
            if (isRunning) Console.WriteLine("The reactor is: Running");
            else Console.WriteLine("The reactor is: Stopped");
        }

        public static void Start()
        {
            if (!isRunning)
            {
                proxy_ctx = new ZContext();
                messenger_ctx = new ZContext();
                proxy = new ZSocket(proxy_ctx, ZSocketType.DEALER);
                messenger = new ZSocket(messenger_ctx, ZSocketType.ROUTER);
                // Bind both sockets to TCP ports
                proxy.Bind("tcp://" + CustomConfig.GetSettings("reactor", "proxy")
                    + ":" + CustomConfig.GetSettings("reactor", "proxy_port"));
                messenger.Bind("tcp://" + CustomConfig.GetSettings("reactor", "messenger")
                    + ":" + CustomConfig.GetSettings("reactor", "messenger_port"));
                if (!isRestarting) Console.WriteLine("Reactor started");
            }
            else Helper.DisplayError("Reactor is already running...");
        }

        public static void Stop()
        {
            if (isRunning)
            {
                proxy.Close();
                proxy_ctx.Terminate();
                messenger.Close();
                messenger_ctx.Terminate();
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
