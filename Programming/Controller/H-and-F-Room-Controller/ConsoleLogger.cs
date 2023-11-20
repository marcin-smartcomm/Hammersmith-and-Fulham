using System;
using System.Collections.Generic;
using Crestron.SimplSharp;
using WebsocketServer;

namespace H_and_F_Room_Controller
{
    public class ConsoleLogger
    {
        ControlSystem _cs;
        private static WebsocketSrvr _server;
        private static bool _clientConnected;

        private static List<string> _backlog;

        public static void WriteLine(string msg, params object[] args)
        {
            msg = msg.Replace("{", "(").Replace("}", ")");
            var text = String.Format(msg, args) + "\n";

            if (_clientConnected)
            {
                _server.SetIndirectTextSignal(1, text);
            }
            else
            {
                if (_backlog.Count > 99)
                    _backlog.RemoveAt(0);

                _backlog.Add(text);
            }
        }

        public void ConsoleLoggerStart(int port, ControlSystem cs)
        {
            try
            {
                _cs = cs;

                _server = new WebsocketSrvr();
                _server.Initialize(port);
                _server.OnClientConnectedChange += OnClientConnected;
                _server.OnStringSignalChange += OnReceivingMessage;

                _backlog = new List<string>();

                _clientConnected = false;

                Start();
            }
            catch (Exception e)
            {
                WriteLine(e.ToString());
            }
        }

        public void Start()
        {
            _server.StartServer();
        }

        public void Stop()
        {
            _server.StopServer();
        }

        private void OnClientConnected(ushort state)
        {
            if (state == 0)
            {
                // Disconnected
                _clientConnected = false;
            }
            else
            {
                // Connected
                _clientConnected = true;
                _server.SetIndirectTextSignal(1, "\n-- CONNECTED --\n");

                if (_backlog.Count > 0)
                {
                    foreach (var msg in _backlog)
                    {
                        _server.SetIndirectTextSignal(1, msg);
                    }
                }

                _backlog.Clear();
            }
        }


        private void OnReceivingMessage(ushort state, SimplSharpString value)
        {
            WriteLine(value.ToString());
            string incomingMessage = value.ToString();

            try
            {
                if (incomingMessage == "__ping__")
                {
                    _server.SetIndirectTextSignal(1, "__pong__");
                }
                if (incomingMessage.Contains("IOCHange"))
                {
                    _cs._roomManager.WallStateChanged(int.Parse(incomingMessage.Split(':')[1]), bool.Parse(incomingMessage.Split(':')[2]));
                }
                if (incomingMessage == "debug:on") ControlSystem.debugEnabled = true;
                if (incomingMessage == "debug:off") ControlSystem.debugEnabled = false;
            }
            catch(Exception e)
            {
                WriteLine($"OnReceivingMessage: {e.Message}");
            }
        }
    }
}
