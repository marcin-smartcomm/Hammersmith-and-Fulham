using Crestron.SimplSharpPro.CrestronConnected;

namespace H_and_F_Room_Controller
{
    public enum DisplayType
    {
        TV,
        Projector,
        ProductionUnit
    }

    public enum DisplayControlType
    {
        rs232,
        cec,
        roomView
    }

    public enum DisplayCommand
    {
        PowerOn,
        PowerOff,
        HDMI1,
        HDMI2
    }

    public class Display
    {
        public DisplayType displayType { get; set; }
        public DisplayControlType controlType { get; set; }
        public uint ipid { get; set; }
        public uint nvxRXConnected { get; set; }
        public string[] rs232Commands { get; set; }
        public int motorizedScreenID { get; set; }
        public RoomViewConnectedDisplay connectedDisplay { get; set; }
    }

    public enum MotorizedScreenControlType
    {
        relay,
        rs232
    }

    public class MotorizedScreen
    {
        public int id { set; get; }
        public MotorizedScreenControlType controlType { get; set; }
        public uint relay { set; get; }
        public bool screenUpRelayState { set; get; }
        public bool screenDownRelayState { set; get; }
        public uint rs232Port { set; get; }
        public string[] rs232Commands { get; set; }
    }

    public enum CameraControlType
    {
        http,
        viscaIP
    }

    public class Camera
    {
        public string name { get; set; }
        public string ip { get; set; }
        public CameraControlType controlType { get; set; }
        public IPConnectionHandler ipComms { get; set; }
    }

    public enum AudioProcessorControlType
    {
        rs232,
        ip
    }

    public class AudioProcessor
    {
        public AudioProcessorControlType controlType { get; set; }
        public string ip { set; get; }
        public int TCPport { set; get; }
        public uint rs232Port { set; get; }
        public string[][] controlComponents { set; get; }
    }

    public class RoomAVData
    {
        public uint[] NVXRxs { get; set; }
        public uint[] NVXTxs { get; set; }
        public AudioProcessor AudioProcessor { get; set; }
        public Display[] Displays { get; set; }
        public MotorizedScreen[] motorizedScreens { get; set; }
        public Camera[] Cameras { get; set; }
    }
}
