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
    /// Network classes interface
    /// </summary>
    public interface State
    {
        abstract void Status();
        abstract void Start();
        abstract void Stop();
        abstract void Restart();
    }


    /// <summary>
    /// Receives requests from users and sends to Reactor
    /// </summary>
    public class Proxy : State
    {
        private bool isRunning { get; set; } = false;
        private bool isRestarting { get; set; } = false;
        private static ZContext backend_ctx { get; set; }
        private static ZContext frontend_ctx { get; set; }
        private ZSocket backend { get; set; }
        private ZSocket frontend { get; set; }

        public void Status()
        {
            if (isRunning) Console.WriteLine("The proxy is: Running");
            else Console.WriteLine("The proxy is: Stopped");
        }

        public void Start()
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

        public void Stop()
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

        public void Restart()
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
    public class Messenger
    {
        private bool isRunning { get; set; } = false;

        public void Status()
        {

        }

        public void Start()
        {

        }

        public void Stop()
        {

        }

        public void Restart()
        {

        }

    }

    /// <summary>
    /// Receives requests from proxy and sends to Messenger
    /// </summary>
    public class Reactor
    {
        private bool isRunning { get; set; } = false;

        public void Status()
        {

        }

        public void Start()
        {

        }

        public void Stop()
        {

        }

        public void Restart()
        {

        }

    }

    /// <summary>
    /// Service Node
    /// </summary>
    public class Node
    {
        private bool isRunning { get; set; } = false;

        public void Status()
        {

        }

        public void Start()
        {

        }

        public void Stop()
        {

        }

        public void Restart()
        {

        }

    }
}
