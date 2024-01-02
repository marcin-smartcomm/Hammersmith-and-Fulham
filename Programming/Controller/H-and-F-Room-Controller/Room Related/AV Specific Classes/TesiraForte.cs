using Crestron.SimplSharp.CrestronSockets;

namespace H_and_F_Room_Controller
{
    public class TesiraForte
    {
        string _id;
        ControlSystem _cs;

        //IP Control
        bool _ipControlled = false;
        TCPClient _comms;
        IPConnectionHandler _commsHandler;

        //RS232 Control
        bool _rs232Controlled = false;
        uint _rs232Port;

        public TesiraForte(ControlSystem cs, TCPClient comms, string id)
        {
            _ipControlled= true;
            _cs = cs;
            _id = id;
            _comms = comms;
        }

        public TesiraForte(ControlSystem cs, AudioProcessor ap, string id)
        {
            _rs232Controlled = true;
            _cs = cs;
            _id = id;
            _rs232Port = ap.rs232Port;
            _cs.serialDataReceived += _cs_serialDataReceived;
        }

        public void Connect()
        {
            if (!_ipControlled) return;

            _commsHandler = new IPConnectionHandler(_cs, _comms, _id);
            _commsHandler.newMessageEvent += _commsHandler_newMessageEvent;
            _commsHandler.Connect();
        }

        public void Disconnect()
        {
            if (!_ipControlled) return;

            _commsHandler.Disconnect();
            _commsHandler = null;
        }

        private void _commsHandler_newMessageEvent(string obj)
        {

        }

        private void _cs_serialDataReceived(int port, string data)
        {
            if (port == _rs232Port)
                ConsoleLogger.WriteLine($"Received From Com Port {port}: " + data);
        }

        private void SendMessage(string msg)
        {
            if (_ipControlled) _commsHandler.SendMessage(msg + "\x0A");
            if (_rs232Controlled) _cs.SendSerialData(_rs232Port, msg + "\x0A");
        }

        public void SubscribeToComponent(string instanceTag, string blockChannel, string blockAttribute, string subscriptionID)
        => SendMessage($"{instanceTag} subscribe {blockAttribute} {blockChannel} {subscriptionID} 100");

        public void UnsbscribeFromComponent(string instanceTag, string blockChannel, string blockAttribute, string subscriptionID)
        => SendMessage($"{instanceTag} unsubscribe {blockAttribute} {blockChannel} {subscriptionID}");

        public void ChangeMatrixConnection(string instanceTag, int newState, int inputIndex, int outputIndex)
        => SendMessage($"{instanceTag} set crosspointLevel {inputIndex} {outputIndex} {newState}");

        public void MuteOn(string instanceTag, string blockChannel) 
        => SendMessage($"{instanceTag} set mute {blockChannel} true");

        public void MuteOff(string instanceTag, string blockChannel)
        => SendMessage($"{instanceTag} set mute {blockChannel} false");

        public void MuteToggle(string instanceTag, string blockChannel)
        => SendMessage($"{instanceTag} toggle mute {blockChannel}");

        public void SliderLevelUp(string instanceTag, string blockChannel)
        => SendMessage($"{instanceTag} increment level {blockChannel} 1");

        public void SliderLevelDown(string instanceTag, string blockChannel)
        => SendMessage($"{instanceTag} decrement level {blockChannel} 1");

        public void SliderLevelChange(string instanceTag, int newLevel, string blockChannel)
        => SendMessage($"{instanceTag} set level {blockChannel} " + newLevel);
    }
}
