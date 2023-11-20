using Crestron.SimplSharp.CrestronSockets;

namespace H_and_F_Room_Controller
{
    public class TesiraForte
    {
        ControlSystem _cs;
        TCPClient _comms;
        string _id;
        IPConnectionHandler _commsHandler;

        public TesiraForte(ControlSystem cs, TCPClient comms, string id)
        {
            _cs = cs;
            _id = id;
            _comms = comms;
        }

        public void Conncet()
        {
            _commsHandler = new IPConnectionHandler(_cs, _comms, _id);
            _commsHandler.newMessageEvent += _commsHandler_newMessageEvent;
            _commsHandler.Connect();
        }

        public void Disconnect()
        {
            _commsHandler.Disconnect();
            _commsHandler = null;
        }

        private void _commsHandler_newMessageEvent(string obj)
        {

        }

        private void SendMessage(string msg)
        {
            _commsHandler.SendMessage(msg);
        }

        public void ChangeMatrixConnection(string componentName, int newState, int inputIndex, int outputIndex)
        {
            SendMessage(componentName + " set crosspointLevel " + inputIndex + " " + outputIndex + " " + newState);
        }

        public void MuteOn(string componentName)
        {
            SendMessage(componentName + " set mute 1 true");
        }

        public void MuteOff(string componentName)
        {
            SendMessage(componentName + " set mute 1 false");
        }

        public void MuteToggle(string componentName)
        {
            SendMessage(componentName + " toggle mute 1");
        }

        public void VolUp(string componentName)
        {
            SendMessage(componentName + " increment level 1 2");
        }

        public void VolDown(string componentName)
        {
            SendMessage(componentName + " decrement level 1 2");
        }

        public void VolLevelChange(string componentName, int newLevel)
        {
            SendMessage(componentName + " set level 1 " + newLevel);
        }
    }
}
