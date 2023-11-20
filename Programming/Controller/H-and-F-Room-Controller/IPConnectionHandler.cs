using Crestron.SimplSharp.CrestronSockets;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace H_and_F_Room_Controller
{
    public class IPConnectionHandler
    {
        public event Action<string> newMessageEvent;

        ControlSystem _cs;
        TCPClient _comms;
        string _id;
        bool _keepConnectionAlive = false;

        public IPConnectionHandler(ControlSystem cs, TCPClient comms, string id)
        {
            _cs = cs;
            _comms = comms;
            _id = id;
        }
        public void Connect()
        {
            if (!_keepConnectionAlive)
            {
                ConsoleLogger.WriteLine("Tryign to connect to: " + _id + " (" + _comms.AddressClientConnectedTo + ")");
                _comms.ConnectToServerAsync(ClientConnectCallBackFunction);
                _comms.SocketStatusChange += _comms_SocketStatusChange;

                _keepConnectionAlive = true;
            }
        }
        public void Disconnect()
        {
            _keepConnectionAlive = false;
            _comms.DisconnectFromServer();
            _comms.SocketStatusChange -= _comms_SocketStatusChange;
        }
        public void SendMessage(string message)
        {
            if (_comms.ClientStatus == SocketStatus.SOCKET_STATUS_CONNECTED)
            {
                byte[] messageInBytes = Encoding.ASCII.GetBytes(message);

                _comms.SendData(messageInBytes, messageInBytes.Length);
            }
        }
        public void SendByteMessage(byte[] message)
        {
            if (_comms.ClientStatus == SocketStatus.SOCKET_STATUS_CONNECTED)
                _comms.SendData(message, message.Length);
        }
        private void ClientConnectCallBackFunction(TCPClient myTcpClient)
        {
            _comms.ReceiveDataAsync(SerialRecieveCallBack);
        }

        void KeepAlive()
        {
            Task.Run(() =>
            {
                while (_comms.ClientStatus == SocketStatus.SOCKET_STATUS_CONNECTED)
                {
                    byte[] bytes = { 0x0A };
                    _comms.SendData(bytes, bytes.Length);
                    Thread.Sleep(5000);
                }
            });
        }
        private void SerialRecieveCallBack(TCPClient myTcpClient, int numberOfBytesReceived)
        {
            var stringdataReceived = Encoding.ASCII.GetString(myTcpClient.IncomingDataBuffer, 0, numberOfBytesReceived);
            ConsoleLogger.WriteLine(_id + ": " + stringdataReceived);
            if (newMessageEvent != null)
                newMessageEvent(stringdataReceived);
            _comms.ReceiveDataAsync(SerialRecieveCallBack);
        }

        void _comms_SocketStatusChange(TCPClient myTCPClient, SocketStatus clientSocketStatus)
        {
            ConsoleLogger.WriteLine("SocketStatus: " + clientSocketStatus);
            if (clientSocketStatus == SocketStatus.SOCKET_STATUS_CONNECTED)
            {
                KeepAlive();
                ConsoleLogger.WriteLine("Connected to: " + _id + " (" + _comms.AddressClientConnectedTo + ")");
            }
            if (clientSocketStatus == SocketStatus.SOCKET_STATUS_NO_CONNECT && _keepConnectionAlive)
            {
                Task.Run(() =>
                {
                    while (_comms.ClientStatus == SocketStatus.SOCKET_STATUS_NO_CONNECT)
                    {
                        Disconnect();
                        Thread.Sleep(2000);
                        Connect();
                        Thread.Sleep(2000);
                    }
                });
            }
        }
    }
}
